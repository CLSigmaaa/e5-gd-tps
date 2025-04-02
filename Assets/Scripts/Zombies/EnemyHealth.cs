using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    public SkinnedMeshRenderer enemyRenderer;
    public float blinkIntensity;
    public float blinkDuration;
    private Color originalColor;
    private Coroutine flashCoroutine;

    public WaveSystem waveSystem;
    UIHealthBar healthBar;

    // Prefabs for drops
    public GameObject ammoPrefab;
    public GameObject medkitPrefab;

    private void Start()
    {
        // find game object with tag "WaveSystem"
        waveSystem = GameObject.FindGameObjectWithTag("WaveSystem").GetComponent<WaveSystem>();
        currentHealth = maxHealth;
        healthBar = GetComponentInChildren<UIHealthBar>();
        if (enemyRenderer != null)
        {
            originalColor = enemyRenderer.material.color;
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBar.SetHealthBarPercentage(currentHealth / maxHealth);
        Debug.Log("Enemy took " + damage + " damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }

        if (enemyRenderer != null)
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            flashCoroutine = StartCoroutine(FlashWhite());
        }
    }

    // set health
    public void SetHealth(float health)
    {
        maxHealth = health;
        currentHealth = health;
        healthBar.SetHealthBarPercentage(currentHealth / maxHealth);
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " has died!");
        healthBar.gameObject.SetActive(false);
        waveSystem.OnZombieKilled(gameObject);
        DropItem();
        Destroy(gameObject); // Destroy the enemy (or play a death animation)
    }

    private void DropItem()
    {
        float dropChance = Random.Range(0f, 1f);
        if (dropChance <= 0.5f) // 50% chance to drop an item
        {
            GameObject drop = null;
            if (Random.Range(0f, 1f) <= 0.5f)
            {
                drop = ammoPrefab; // 50% chance to drop ammo
            }
            else
            {
                drop = medkitPrefab; // 50% chance to drop medkit
            }

            if (drop != null)
            {
                Instantiate(drop, transform.position, Quaternion.identity);
            }
        }
    }

    private IEnumerator FlashWhite()
    {
        float elapsedTime = 0f;
        while (elapsedTime < blinkDuration)
        {
            float lerp = elapsedTime / blinkDuration;
            enemyRenderer.material.color = Color.Lerp(Color.white, originalColor, lerp);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        enemyRenderer.material.color = originalColor;
    }
}
