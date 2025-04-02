using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public ParticleSystem healEffect;
    public SkinnedMeshRenderer playerRenderer;
    public UIHealthBar healthBar;
    public float blinkIntensity;
    public float blinkDuration;
    private Color originalColor;
    private Coroutine flashCoroutine;

    private void Start()
    {
        currentHealth = maxHealth;
        healEffect.Stop();
        if (playerRenderer != null)
        {
            originalColor = playerRenderer.material.color;
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Debug.Log("Vie restaurée : " + currentHealth);

        if (healEffect != null)
        {
            healEffect.Play();
        }

        if (healthBar != null)
        {
            healthBar.SetHealthBarPercentage((float)currentHealth / maxHealth);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        Debug.Log("Vie perdue : " + currentHealth);

        if (playerRenderer != null)
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
            }
            flashCoroutine = StartCoroutine(FlashWhite());
        }

        if (healthBar != null)
        {
            healthBar.SetHealthBarPercentage((float)currentHealth / maxHealth);
        }
    }

    private IEnumerator FlashWhite()
    {
        float elapsedTime = 0f;
        while (elapsedTime < blinkDuration)
        {
            float lerp = elapsedTime / blinkDuration;
            playerRenderer.material.color = Color.Lerp(Color.white, originalColor, lerp);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        playerRenderer.material.color = originalColor;
    }
}

