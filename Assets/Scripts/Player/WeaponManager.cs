using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UIElements;

public class WeaponManager : MonoBehaviour
{
    public Transform weaponHolder;
    public RaycastWeapon currentWeapon;
    PlayerNoise playerNoise;

    public Rig weaponAimingRig;

    private void Start()
    {
        playerNoise = GetComponent<PlayerNoise>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E key pressed");
            TryPickupWeapon();
        }

        if (currentWeapon != null && Input.GetMouseButtonDown(0))
        {
            //weaponAimingRig.weight = 1;
            currentWeapon.StartFiring();
            playerNoise.Shoot();
        }
        else if (currentWeapon != null && Input.GetMouseButtonUp(0))
        {
            //weaponAimingRig.weight = 0;
            currentWeapon.StopFiring();
        }
    }

    private void TryPickupWeapon()
    {
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 20f))
        {
            if (hit.collider.CompareTag("Weapon"))
            {
                PickupWeapon(hit.collider.gameObject);
            }
        }
    }

    private void PickupWeapon(GameObject weaponObject)
    {
        RaycastWeapon newWeapon = weaponObject.GetComponent<RaycastWeapon>();

        if (newWeapon != null)
        {
            if (currentWeapon != null)
            {
                Destroy(currentWeapon.gameObject); // Déséquiper l'ancienne arme
            }

            currentWeapon = newWeapon;
            currentWeapon.transform.SetParent(weaponHolder);
            // local positon should be 0, -0.08800115, 0.2789989
            currentWeapon.transform.localPosition = new Vector3(0, -0.08800115f, 0.2789989f);
            // local rotation of -90 degrees on the x axis
            // and 90 degrees on the z axis
            currentWeapon.transform.localRotation = Quaternion.Euler(0, 0, 0);
            currentWeapon.gameObject.SetActive(true);
        }
    }




}
