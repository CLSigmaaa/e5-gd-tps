using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour
{
    public Transform item; // Un seul objet à faire tourner
    public float rotationSpeed = 50f;
    public float radius = 3f;

    void Start()
    {
        // Assigner automatiquement le premier enfant comme item
        if (transform.childCount > 0)
        {
            item = transform.GetChild(0);
        }
    }

    void Update()
    {
        if (item != null)
        {
            float angle = Time.time * rotationSpeed;
            float x = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            float z = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

            // Modifier la position locale pour garder la hiérarchie intacte
            item.localPosition = new Vector3(x, 0, z);

            // Faire tourner l'item autour de son axe propre
            item.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
            item.Rotate(Vector3.right * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}
