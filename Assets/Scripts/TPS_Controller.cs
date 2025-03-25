using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPS_Controller : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 7.5f;
    //public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float forwardPlayerSpeed = 2.0f;
    public float backwardPlayerSpeed = 1.0f;
    public float lateralPlayerSpeed = 1.0f;
    public float runSpeed = 10.0f;
    //public float jumpForce = 0.5f;
    public float sprintSpeed = 15.0f;
    public float jumpCooldown = 1.0f; // Temps d'attente entre les sauts
    public float sprintCooldown = 1.0f; // Temps d'attente entre les sprints
    public float crouchSpeed = 3.0f;
    public float crouchCooldown = 1.0f; // Temps d'attente entre les accroupissements

    [Header("Camera Settings")]
    public Transform playerCameraParent;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 60.0f;

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;
    private TPS_Animation_Controller tpsAnimationController;
    private Vector2 rotation = Vector2.zero;

    [HideInInspector]
    public bool canMove = true;

    private float lastJumpTime = 0f;
    private float lastSprintTime = 0f;
    private float lastCrouchTime = 0f;
    private bool isCrouching = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        tpsAnimationController = GetComponent<TPS_Animation_Controller>();
        rotation.y = transform.eulerAngles.y;

        // Verrouiller et cacher le curseur
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
        //HandleAnimations();
    }

    private void HandleMovement()
    {
        if (characterController.isGrounded)
        {
            Vector3 inputDirection = GetInputDirection();

            // Calculer la vitesse en fonction du sprint et de l'accroupissement
            float currentSpeed = speed;
            if (Input.GetKey(KeyCode.LeftShift) && inputDirection.z > 0 && Time.time - lastSprintTime > sprintCooldown)
            {
                currentSpeed = sprintSpeed;
            }
            else if (isCrouching)
            {
                currentSpeed = crouchSpeed;
            }

            // Appliquer la direction avec la vitesse
            moveDirection = CalculateMoveDirection(inputDirection, currentSpeed);

            // Gestion des animations (marche, sprint ou accroupissement)
            if (inputDirection.magnitude > 0)
            {
                tpsAnimationController.SetIsWalking(true);
                tpsAnimationController.SetIsSprinting(currentSpeed == sprintSpeed); // Gérer l'animation sprint
                tpsAnimationController.SetIsCrouching(isCrouching); // Gérer l'animation accroupissement
            }
            else
            {
                tpsAnimationController.SetIsWalking(false);
            }

            if (Input.GetButton("Jump") && canMove && Time.time - lastJumpTime > jumpCooldown)
            {
                tpsAnimationController.SetIsJumping(true);
                lastJumpTime = Time.time; // Mettre à jour le temps du dernier saut
            }
            else
            {
                tpsAnimationController.SetIsJumping(false);
            }

            // Gestion de l'accroupissement
            if (Input.GetKey(KeyCode.X))
            {
                isCrouching = true;
            }
            else
            {
                isCrouching = false;
            }
        }
        else
        {
            tpsAnimationController.SetIsJumping(false);
        }

        ApplyGravity();
        characterController.Move(moveDirection * Time.deltaTime);
    }



    private Vector3 GetInputDirection()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (z < 0)
        {
            z *= backwardPlayerSpeed;
        }
        else
        {
            z *= forwardPlayerSpeed;
        }
        x *= lateralPlayerSpeed;

        tpsAnimationController.SetX(x);
        tpsAnimationController.SetY(z);

        return new Vector3(x, 0, z);
    }

    private Vector3 CalculateMoveDirection(Vector3 inputDirection, float currentSpeed)
    {
        // Calcul de la direction et normalisation si nécessaire
        Vector3 direction = transform.right * inputDirection.x + transform.forward * inputDirection.z;

        // Appliquer la vitesse calculée (normale, sprint ou accroupissement)
        direction *= currentSpeed;

        return direction;
    }

    private void ApplyGravity()
    {
        moveDirection.y -= gravity * Time.deltaTime;
    }

    private void HandleRotation()
    {
        if (canMove)
        {
            rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
            rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);

            playerCameraParent.localRotation = Quaternion.Euler(rotation.x, 0, 0);
            transform.eulerAngles = new Vector2(0, rotation.y);
        }
    }
}
