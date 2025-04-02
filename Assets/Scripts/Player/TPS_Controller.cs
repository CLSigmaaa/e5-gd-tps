using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class TPS_Controller : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 7.5f;
    public float gravity = 20.0f;
    public float forwardPlayerSpeed = 2.0f;
    public float backwardPlayerSpeed = 1.0f;
    public float lateralPlayerSpeed = 1.0f;
    public float runSpeed = 10.0f;
    public float sprintSpeed = 15.0f;
    public float jumpCooldown = 1.0f;
    public float jumpHeight = 2.0f; // Add this line to define the jump height
    public float sprintCooldown = 1.0f;
    public float crouchSpeed = 3.0f;
    public float crouchCooldown = 1.0f;

    [Header("Camera Settings")]
    public Transform playerCameraParent;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 60.0f;
    public float normalFOV = 60.0f;
    public float aimFOV = 40.0f;
    public float aimSpeedMultiplier = 0.5f;
    public float aimTransitionSpeed = 5.0f;

    [Header("UI Settings")]
    public RectTransform crosshair;
    public Vector2 normalCrosshairSize = new Vector2(50, 50);
    public Vector2 aimCrosshairSize = new Vector2(25, 25);
    public float crosshairTransitionSpeed = 5.0f;
    public Text grenadeText;

    [Header("Grenade Settings")]
    public GameObject grenadePrefab;
    public float throwForce = 10.0f;
    public Transform grenadeSlot; // Add this line to define the grenade slot
    public int maxGrenades = 3; // Add this line to define the maximum number of grenades

    public int currentGrenades; // Add this line to store the current number of grenades

    private Vector3 moveDirection = Vector3.zero;
    private CharacterController characterController;
    private TPS_Animation_Controller tpsAnimationController;
    private Vector2 rotation = Vector2.zero;
    private Camera playerCamera;

    [Header("Rig Settings")]
    public Rig weaponAimingRig;

    private PlayerNoise playerNoise;

    [HideInInspector]
    public bool canMove = true;

    private float lastJumpTime = 0f;
    private float lastSprintTime = 0f;
    private float lastCrouchTime = 0f;
    private bool isCrouching = false;
    private bool isAiming = false;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        tpsAnimationController = GetComponent<TPS_Animation_Controller>();
        playerCamera = playerCameraParent.GetComponentInChildren<Camera>();
        playerNoise = GetComponent<PlayerNoise>();
        rotation.y = transform.eulerAngles.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        currentGrenades = maxGrenades; // Initialize the current number of grenades
        grenadeText.text = currentGrenades + " / " + maxGrenades; // Update the grenade text
    }

    void Update()
    {
        HandleMovement();
        HandleAiming();
        HandleCrouching();
        HandleGrenadeThrow();
        //HandleCrosshair();
    }

    private void HandleMovement()
    {
        if (characterController.isGrounded)
        {
            Vector3 inputDirection = GetInputDirection();

            float currentSpeed = speed;
            bool isSprinting = Input.GetKey(KeyCode.LeftShift) && inputDirection.z > 0 && Time.time - lastSprintTime > sprintCooldown;

            // Vérifiez si le joueur essaie de tirer en courant
            if (isSprinting && Input.GetMouseButton(0))
            {
                isSprinting = false; // Désactivez le sprint
                currentSpeed = speed; // Réglez la vitesse sur la vitesse de marche normale
            }
            else if (isSprinting)
            {
                currentSpeed = sprintSpeed;
                playerNoise.Sprint();
            }
            else if (isCrouching)
            {
                currentSpeed = crouchSpeed;
                playerNoise.Crouch();
            }
            else if (isAiming)
            {
                currentSpeed *= aimSpeedMultiplier;
                playerNoise.Idle();
            }
            else
            {
                playerNoise.Walk();
            }

            moveDirection = CalculateMoveDirection(inputDirection, currentSpeed);

            if (inputDirection.magnitude > 0)
            {
                tpsAnimationController.SetIsWalking(true);
                tpsAnimationController.SetIsSprinting(currentSpeed == sprintSpeed);
                tpsAnimationController.SetIsCrouching(isCrouching);
            }
            else
            {
                tpsAnimationController.SetIsWalking(false);
                playerNoise.Idle();
            }

            if (Input.GetButton("Jump") && canMove && Time.time - lastJumpTime > jumpCooldown)
            {
                tpsAnimationController.SetIsJumping(true);
                lastJumpTime = Time.time;
                moveDirection.y = Mathf.Sqrt(2 * gravity * jumpHeight); // Add this line to set the vertical velocity for jumping
                playerNoise.Shoot();
            }
            else
            {
                tpsAnimationController.SetIsJumping(false);
            }
        }
        else
        {
            tpsAnimationController.SetIsJumping(false);
        }

        ApplyGravity();
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleAiming()
    {
        bool isAimingOrShooting = Input.GetMouseButton(1) || Input.GetMouseButton(0); // Vérifie si on vise ou tire
        float targetWeight = isAimingOrShooting ? 1f : 0f;
        weaponAimingRig.weight = Mathf.Lerp(weaponAimingRig.weight, targetWeight, aimTransitionSpeed * Time.deltaTime);
        isAiming = targetWeight > 0.5f;

        float targetFOV = isAiming ? aimFOV : normalFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, aimTransitionSpeed * Time.deltaTime);
    }

    private void HandleCrouching()
    {
        if (Input.GetKey(KeyCode.X))
        {
            isCrouching = true;
        }
        else if (isCrouching && !Input.GetKey(KeyCode.X))
        {
            isCrouching = false;
        }

        tpsAnimationController.SetIsCrouching(isCrouching);
    }

    private void HandleGrenadeThrow()
    {
        if (Input.GetKeyDown(KeyCode.G) && currentGrenades > 0) // Check if there are grenades left
        {
            ThrowGrenade();
            currentGrenades--; // Decrease the number of grenades
        }
    }

    private void ThrowGrenade()
    {
        grenadeText.text = currentGrenades + " / " + maxGrenades; // Update the grenade text
        GameObject grenade = Instantiate(grenadePrefab, grenadeSlot.position, grenadeSlot.rotation); // Use grenadeSlot for position and rotation
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        rb.AddForce(playerCamera.transform.forward * throwForce, ForceMode.VelocityChange);
    }

    private void HandleCrosshair()
    {
        Vector2 targetSize = isAiming ? aimCrosshairSize : normalCrosshairSize;
        crosshair.sizeDelta = Vector2.Lerp(crosshair.sizeDelta, targetSize, crosshairTransitionSpeed * Time.deltaTime);
    }

    public Vector3 GetMoveDirection()
    {
        return moveDirection;
    }

    public void ApplyRecoil(float recoilAmount)
    {
        rotation.x -= recoilAmount;
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
        Vector3 direction = transform.right * inputDirection.x + transform.forward * inputDirection.z;
        direction *= currentSpeed;
        return direction;
    }

    private void ApplyGravity()
    {
        moveDirection.y -= gravity * Time.deltaTime;
    }
}
