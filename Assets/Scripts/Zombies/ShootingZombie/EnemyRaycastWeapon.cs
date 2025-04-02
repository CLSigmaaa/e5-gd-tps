using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRaycastWeapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float damage = 10f;
    public float fireRate = 0.1f;
    public float recoil = 1f;               // Force du recul
    public float spread = 0.05f;
    public float maxDistance = 100f;
    public float recoilReturnSpeed = 5f;    // Vitesse du retour à la position d'origine
    public int bulletsPerMag = 30;          // Nombre de balles par chargeur

    [Header("Weapon FX")]
    public ParticleSystem muzzleFlash;
    public ParticleSystem impactEffect;
    public LineRenderer bulletTracer;
    public Transform raycastOrigin;
    public Transform rayCastDirection;

    private float nextFireTime = 0f;
    private bool isFiring = false;
    private bool isReloading = false;       // Variable pour suivre l'état de rechargement
    private int currentBullets;                   // Balles actuelles dans le chargeur

    private Ray ray;
    private RaycastHit hitInfo;

    private void Awake()
    {
        muzzleFlash.Stop();
        bulletTracer.positionCount = 2;
        bulletTracer.enabled = false;
        currentBullets = bulletsPerMag;  // Initialiser les balles actuelles
    }

    private void Start()
    {
        // find component with tag bullteTracer
        bulletTracer = GameObject.FindGameObjectWithTag("bulletTracer").GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (isFiring && Time.time >= nextFireTime && !isReloading)
        {
            if (currentBullets > 0)
            {
                nextFireTime = Time.time + fireRate;
                FireWeapon();
            }
            else
            {
                StartCoroutine(Reload());
            }
        }

        // Retour progressif de la caméra à sa position d'origine
        //recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, Time.deltaTime * recoilReturnSpeed);
        //playerCamera.transform.localRotation = Quaternion.Euler(recoilOffset);
    }

    public void StartFiring()
    {
        isFiring = true;
    }

    public void StopFiring()
    {
        isFiring = false;
        muzzleFlash.Stop();
    }

    private void FireWeapon()
    {
        if (!muzzleFlash.isPlaying)
        {
            muzzleFlash.Play();
        }

        // Calcul du raycast avec dispersion aléatoire
        ray.origin = raycastOrigin.position;

        // direction is behind origin
        ray.direction = raycastOrigin.position - rayCastDirection.position;
        ray.direction += new Vector3(
            Random.Range(-spread, spread),
            Random.Range(-spread, spread),
            0
        );

        Vector3 endPosition = ray.origin + ray.direction * maxDistance;

        if (Physics.Raycast(ray, out hitInfo, maxDistance))
        {
            endPosition = hitInfo.point;
            Debug.Log("Hit " + hitInfo.collider.gameObject.name);

            if (hitInfo.collider.CompareTag("Player"))
            {
                PlayerHealth playerHealth = hitInfo.collider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage((int)damage);
                }

                impactEffect.transform.position = hitInfo.point;
                impactEffect.transform.forward = hitInfo.normal;
                impactEffect.Play();
            }
        }

        bulletTracer.SetPosition(0, ray.origin);
        bulletTracer.SetPosition(1, endPosition);
        StartCoroutine(ShowTracer());

        // Appliquer le recul à la caméra
        //ApplyRecoil();

        // Décrémenter le nombre de balles actuelles
        currentBullets--;
    }

    private IEnumerator ShowTracer()
    {
        bulletTracer.enabled = true;
        yield return new WaitForSeconds(0.1f);
        bulletTracer.enabled = false;
    }

    //private void ApplyRecoil()
    //{
    //    // Ajout d'un offset temporaire sur l'axe vertical (pour simuler le recul)
    //    recoilOffset += new Vector3(-recoil, Random.Range(-recoil / 2, recoil / 2), 0);
    //}

    private IEnumerator Reload()
    {
        isReloading = true;
        muzzleFlash.Stop();
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(2f); // Temps de rechargement
        currentBullets = bulletsPerMag;  // Recharger le chargeur avec le nombre de balles par chargeur
        isReloading = false;
    }
}
