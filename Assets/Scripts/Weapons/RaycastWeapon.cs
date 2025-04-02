using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class RaycastWeapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float damage = 10f;
    public float fireRate = 0.1f;
    public float recoil = 1f;               // Force du recul
    public float spread = 0.05f;
    public float maxDistance = 100f;
    public float recoilReturnSpeed = 5f;    // Vitesse du retour à la position d'origine
    public int bulletsPerMag = 30;          // Nombre de balles par chargeur
    public int totalBullets = 90;           // Nombre total de balles

    [Header("Weapon FX")]
    public ParticleSystem muzzleFlash;
    public ParticleSystem impactEffect;
    public LineRenderer bulletTracer;
    public Transform raycastOrigin;
    public Camera playerCamera;

    private float nextFireTime = 0f;
    private bool isFiring = false;
    private bool isReloading = false;       // Variable pour suivre l'état de rechargement
    private Vector3 recoilOffset = Vector3.zero;  // Offset temporaire du recul
    public int currentBullets;                   // Balles actuelles dans le chargeur

    private Ray ray;
    private RaycastHit hitInfo;

    public Text ammoText;

    //public Rig 

    public Animator playerAnim;

    private void Awake()
    {
        muzzleFlash.Stop();
        bulletTracer.positionCount = 2;
        bulletTracer.enabled = false;
        currentBullets = bulletsPerMag;  // Initialiser les balles actuelles
        UpdateAmmoText(); // Mettre à jour le texte des munitions au démarrage
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
        recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, Time.deltaTime * recoilReturnSpeed);
        playerCamera.transform.localRotation = Quaternion.Euler(recoilOffset);
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
        ray.direction = playerCamera.transform.forward;
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

            if (hitInfo.collider.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = hitInfo.collider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(damage);
                }
                // get shooting locomotion 
                ShootingZombieLocomotion shootingLocomotion = hitInfo.collider.GetComponent<ShootingZombieLocomotion>();
                if (shootingLocomotion != null)
                {
                    // aggro the player
                    shootingLocomotion.AggroPlayer();
                }

                BasicZombieLocomotion basicLocomotion = hitInfo.collider.GetComponent<BasicZombieLocomotion>();
                if (basicLocomotion != null)
                {
                    // aggro the player
                    basicLocomotion.AggroPlayer();
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
        ApplyRecoil();

        // Décrémenter le nombre de balles actuelles
        currentBullets--;
        UpdateAmmoText(); // Mettre à jour le texte des munitions après chaque tir
    }

    private IEnumerator ShowTracer()
    {
        bulletTracer.enabled = true;
        yield return new WaitForSeconds(0.1f);
        bulletTracer.enabled = false;
    }

    private void ApplyRecoil()
    {
        // Ajout d'un offset temporaire sur l'axe vertical (pour simuler le recul)
        recoilOffset += new Vector3(-recoil, Random.Range(-recoil / 2, recoil / 2), 0);
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        muzzleFlash.Stop();
        //Debug.Log("Reloading...");
        //playerAnim.SetTrigger("Reload");
        yield return new WaitForSeconds(2f); // Temps de rechargement
        if (totalBullets > 0)
        {
            int bulletsToReload = Mathf.Min(bulletsPerMag, totalBullets);
            currentBullets = bulletsToReload;
            totalBullets -= bulletsToReload;
        }
        else
        {
            Debug.Log("Out of bullets!");
        }
        isReloading = false;
        UpdateAmmoText(); // Mettre à jour le texte des munitions après le rechargement
    }

    private void UpdateAmmoText()
    {
        if (ammoText != null)
        {
            ammoText.text = $"{currentBullets} / {totalBullets}";
        }
    }
}
