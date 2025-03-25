using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPS_Animation_Controller : MonoBehaviour
{

    private Animator animator;
    private bool isWalking = false;
    private bool isSprinting = false;
    private bool isJumping = false;
    private bool isCrouching = false;
    private float x = 0.0f;
    private float y = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetIsWalking(bool isWalking)
    {
        this.isWalking = isWalking;
        animator.SetBool("isWalking", isWalking);
    }

    public void SetIsCrouching(bool isCrouching)
    {
        this.isCrouching = isCrouching;
        animator.SetBool("isCrouching", isCrouching);
    }

    public void SetIsSprinting(bool isSprinting)
    {
        this.isSprinting = isSprinting;
        animator.SetBool("isSprinting", isSprinting);
    }

    public void SetIsJumping(bool isJumping)
    {
        this.isJumping = isJumping;
        animator.SetBool("isJumping", isJumping);
    }

    // get isJumping
    public bool GetIsJumping()
    {
        return isJumping;
    }

    public void SetX(float x)
    {
        this.x = x;
        animator.SetFloat("x", x);
    }

    public void SetY(float y)
    {
        this.y = y;
        animator.SetFloat("y", y);
    }
}
