using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPS_Aim_Rig : MonoBehaviour
{
    private Vector2 rotation = Vector2.zero;
    private Camera playerCamera;
    public Transform target;

    public float lookSpeed = 2.0f;
    public float lookXLimit = 60.0f;
    public float normalFOV = 60.0f;
    public float targetFollowSpeed = 5.0f; // Vitesse de suivi de la cible
    public float playerRotationSpeed = 2.0f; // Vitesse de rotation du joueur
    public float cameraMoveSpeed = 2.0f; // Vitesse de déplacement de la caméra


    public float horizontalClamp = 80.0f;
    public Transform playerCameraParent;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // Verrouille le curseur
    }

    private void Update()
    {
        HandleRotation();
        HandleCameraMovement();
    }

    private void HandleRotation()
    {
        if (true)
        {
            rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
            rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);



            playerCameraParent.localRotation = Quaternion.Euler(rotation.x, 0, 0);
            transform.eulerAngles = new Vector2(0, rotation.y);

            // Calculer la nouvelle position de la cible
            Vector3 targetPosition = playerCameraParent.position + playerCameraParent.forward * 10;

            // Interpoler la position de la cible pour créer un effet de délai
            target.position = Vector3.Lerp(target.position, targetPosition, Time.deltaTime * targetFollowSpeed);

            // Interpoler la rotation du joueur pour ralentir la rotation
            Vector3 lookDirection = new Vector3(playerCameraParent.forward.x, 0, playerCameraParent.forward.z);
            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * playerRotationSpeed);
            }
        }
    }

    private void HandleCameraMovement()
    {
        //if (Input.GetKey(KeyCode.X))
        //{
        //    // set y position to 1.5

        //    cameraPosition.y = 1.5f;

        //    playerCameraParent.position = Vector3.Lerp(playerCameraParent.position, cameraPosition, Time.deltaTime * cameraMoveSpeed);
        //}
    }
}