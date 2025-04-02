using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    public RaycastWeapon raycastWeapon;
    public float interactionDistance = 3f; // Distance à partir de laquelle l'arme peut être ramassée
    private Weapon currentWeapon = null; // L'arme actuellement équipée

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            raycastWeapon.StartFiring();
        }
        else if (Input.GetButtonUp("Fire1"))
        {
            raycastWeapon.StopFiring();
        }
    }
}
