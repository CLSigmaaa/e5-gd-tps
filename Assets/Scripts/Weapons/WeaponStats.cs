using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponStats", menuName = "Weapons/WeaponStats")]
public class WeaponStats : ScriptableObject
{
    public string weaponName;       // Nom de l'arme
    public int damage;              // Dégâts infligés par l'arme
    public float fireRate;          // Nombre de tirs par seconde
    public int magazineSize;        // Taille du chargeur
    public float recoilAmount;      // Intensité du recul
}
