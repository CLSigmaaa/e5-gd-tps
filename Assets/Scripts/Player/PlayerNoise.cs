using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNoise : MonoBehaviour
{
    private float currentNoiseLevel = 1f;
    private LineRenderer noiseCircle;

    void Start()
    {
        noiseCircle = gameObject.AddComponent<LineRenderer>();
        noiseCircle.positionCount = 50; // Nombre de segments pour dessiner le cercle
        noiseCircle.startWidth = 0.1f;
        noiseCircle.endWidth = 0.1f;
        noiseCircle.useWorldSpace = false;
        noiseCircle.material = new Material(Shader.Find("Sprites/Default"));
        noiseCircle.startColor = new Color(1, 0, 0, 0.5f); // Couleur rouge semi-transparente
        noiseCircle.endColor = new Color(1, 0, 0, 0.5f);

        UpdateNoiseCircle(); // Dessine le cercle au démarrage
    }

    void Update()
    {
        BroadCastNoiseToEnnemies();
    }

    public void SetNoiseLevel(float noise)
    {
        currentNoiseLevel = noise;
        UpdateNoiseCircle();
    }

    public float GetCurrentNoiseLevel()
    {
        return currentNoiseLevel;
    }

    public void Idle()
    {
        SetNoiseLevel(5);
    }
    public void Crouch()
    {
        SetNoiseLevel(2.5f);
    }

    public void Walk()
    {
        SetNoiseLevel(10f);
    }

    public void Sprint()
    {
        SetNoiseLevel(15f);
    }

    public void Shoot()
    {
        SetNoiseLevel(30f);
    }

    private void UpdateNoiseCircle()
    {
        if (noiseCircle != null)
        {
            DrawCircle(currentNoiseLevel);
        }
    }

    private void DrawCircle(float radius)
    {
        float angle = 20f;
        for (int i = 0; i < noiseCircle.positionCount; i++)
        {
            float x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            noiseCircle.SetPosition(i, new Vector3(x, 0, z));
            angle += (360f / noiseCircle.positionCount);
        }
    }

    private void BroadCastNoiseToEnnemies()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, currentNoiseLevel);
        //Debug.Log("Noise broadcasted to " + colliders.Length + " enemies");
        foreach (Collider col in colliders)
        {
            if (col.CompareTag("Enemy") || col.CompareTag("Mutant"))
            {
                IEnemy enemy = col.GetComponent<IEnemy>();
                if (enemy != null)
                {
                    //Debug.Log("Noise broadcasted to enemy");
                    enemy.OnPlayerNoise();
                }
            }
        }
    }
}
