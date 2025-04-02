using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : MonoBehaviour
{
    public int healAmount = 25;  // Quantit� de vie r�cup�r�e

    private void OnTriggerEnter(Collider other)
    {
        // V�rifie si c'est le joueur qui touche le medkit
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);

                // D�truit le medkit apr�s utilisation
                Destroy(gameObject);
            }
        }
    }
}
