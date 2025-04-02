using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUI : MonoBehaviour
{
    public Transform target;
    public float yOffset = 0.5f;
    public float xOffset = 0.5f;
    public float zOffset = 0.0f;

    private void LateUpdate()
    {
        if (target != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(target.position + new Vector3(xOffset, yOffset, zOffset));
        }
    }
}
