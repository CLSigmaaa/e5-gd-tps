using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TPS_Animation_Controller : MonoBehaviour
{

    private Animator animator;
    private bool isWalking = false;
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
