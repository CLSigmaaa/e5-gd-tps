using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName = "Pistol";    // Nom de l'arme
    public int damage = 10;                 // Dégâts infligés par l'arme
    public float range = 50f;               // Portée du tir
    public float fireRate = 0.5f;           // Temps entre deux tirs (cadence)
    public RaycastWeapon raycastWeapon;     // Référence à l'arme de tir

    private float nextTimeToFire = 0f;      // Pour gérer la cadence de tir

    private void Start()
    {
        // On récupère la référence à l'arme de tir
        raycastWeapon = GetComponent<RaycastWeapon>();
    }

    public void TryFire()
    {
        // Si le cooldown est terminé, l'arme peut tirer
        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + fireRate;
            raycastWeapon.StartFiring();
        }
    }
}
