using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 16f;
    private bool isFacingRight = true;
    private bool inAir;

    private float airTime;
    private float chargingTime;
    private float chargingPower = 32f;

    private float coyoteTime = 0.2f;
    private float coyoteTimecounter;

    private float jumpBufferTime = 0.2f;
    private float jumpBufferCounter;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (IsGrounded())
        {
            coyoteTimecounter = coyoteTime;
        }
        else
        {
            coyoteTimecounter -= Time.deltaTime;
        }

        if (Input.GetButtonDown("Jump"))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        anim.SetFloat("Walking", Mathf.Abs(horizontal));

        if (jumpBufferCounter > 0f && coyoteTimecounter > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);

            jumpBufferCounter = 0f;

            anim.SetTrigger("Jumping");
            inAir = true;
        }

        if (inAir == true) 
        {
            airTime += Time.deltaTime;
            if (IsGrounded() && airTime > 0.1f)
            {
                inAir = false;
                anim.SetTrigger("Landing");
                airTime = 0;
            }
        }

        if (Input.GetButtonUp ("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);

            coyoteTimecounter = 0f;
        }

        Flip();

        if (Input.GetKey(KeyCode.LeftControl) && IsGrounded())
        {
            chargingTime += Time.deltaTime;
        }
        
        if (chargingTime > 0f && horizontal != 0)
        {
            chargingTime = 0f;
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            if (chargingTime > 2)
            {
                rb.velocity = new Vector2(rb.velocity.x, chargingPower);
                anim.SetTrigger("Jumping");
                inAir = true;
            }
            chargingTime = 0;
        }

        anim.SetFloat("ChargingTime", chargingTime);
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2 (horizontal * speed, rb.velocity.y);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
}
