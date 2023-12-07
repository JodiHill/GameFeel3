using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Movement : MonoBehaviour
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

    public ParticleSystem jumpps, landps, chargejumpps;

    private Animator anim;

    public AnimationCurve jumpForce;
    public float chargeTimeCheck = 2;

    public GameObject canvas, canvasparticle;
    public RawImage loader;
    public AudioSource audioSource, dead;
    private Vector3 spawnPosition;

    private void Start()
    {
        canvas.SetActive(false);
        canvasparticle.SetActive(false);
        loader.rectTransform.sizeDelta = new Vector2(0, 1);
        anim = GetComponent<Animator>();
        spawnPosition = transform.position;
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

            jumpps.Stop();
            jumpps.Play();
            anim.SetTrigger("Jumping");
            audioSource.Stop();
            audioSource.Play();
            inAir = true;
        }

        if (inAir == true) 
        {
            airTime += Time.deltaTime;
            if (IsGrounded() && airTime > 0.1f)
            {
                inAir = false;
                landps.Stop();
                landps.Play();
                anim.ResetTrigger("Jumping");
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
            float power = Mathf.Clamp01(jumpForce.Evaluate(chargingTime / 2f));
            loader.rectTransform.sizeDelta = new Vector2(power, 1);
        }
        
        if (chargingTime > 0f && horizontal != 0)
        {
            chargingTime = 0f;
        }

        if (chargingTime > chargeTimeCheck)
        {
            chargingTime = chargeTimeCheck;
            if (!canvasparticle.activeSelf)
            {
                canvasparticle.SetActive(true);
            }
        }
        else
        {
            if (canvasparticle.activeSelf)
            {
                canvasparticle.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            canvas.SetActive(true);
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            canvas.SetActive(false);
            rb.velocity = new Vector2(rb.velocity.x, chargingPower * jumpForce.Evaluate(chargingTime / 2f));
            anim.SetTrigger("Jumping");
            chargejumpps.Stop();
            chargejumpps.Play();
            audioSource.Stop();
            audioSource.Play();
            inAir = true;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Board"))
        {
            collision.gameObject.GetComponent<ParticlePlayer>().PlayParticle();
            spawnPosition = collision.transform.position + Vector3.up;
        }
    }

    public void RespawnPlayer()
    {
        transform.position = spawnPosition;
        dead.Stop();
        dead.Play();
    }
}
