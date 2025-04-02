using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName = "Pistol";    // Nom de l'arme
    public int damage = 10;                 // D�g�ts inflig�s par l'arme
    public float range = 50f;               // Port�e du tir
    public float fireRate = 0.5f;           // Temps entre deux tirs (cadence)
    public RaycastWeapon raycastWeapon;     // R�f�rence � l'arme de tir

    private float nextTimeToFire = 0f;      // Pour g�rer la cadence de tir

    private void Start()
    {
        // On r�cup�re la r�f�rence � l'arme de tir
        raycastWeapon = GetComponent<RaycastWeapon>();
    }

    public void TryFire()
    {
        // Si le cooldown est termin�, l'arme peut tirer
        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            raycastWeapon.StartFiring();
        }
    }
}
