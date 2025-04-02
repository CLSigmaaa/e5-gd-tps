using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    public RaycastWeapon weapon;
    public TPS_Controller player;

    private void Update()
    {
       
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            weapon = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<RaycastWeapon>();
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<TPS_Controller>();


            if (weapon != null)
            {
                weapon.totalBullets += weapon.bulletsPerMag * 5;
                weapon.ammoText.text = weapon.currentBullets + " / " + weapon.totalBullets;
                Destroy(gameObject);
            }

            if (player != null)
            {
                player.currentGrenades += 1;
                player.grenadeText.text = (player.currentGrenades + 1) + " / " + player.maxGrenades;
            }
        }
    }
}
