using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Medkit : MonoBehaviour
{
    public int healAmount = 25;  // Quantité de vie récupérée

    private void OnTriggerEnter(Collider other)
    {
        // Vérifie si c'est le joueur qui touche le medkit
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);

                // Détruit le medkit après utilisation
                Destroy(gameObject);
            }
        }
    }
}
