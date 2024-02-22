using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float jumpPower = 5f;
    [SerializeField] private float chargeDuration = 1f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashSpeed = 5f;
    [SerializeField] private int maxDashCount = 1;
    [SerializeField] private float gravityScale = 1.5f;
    [SerializeField] private float groundedDelay = 0.1f;
    [SerializeField] private float risingGravityMultiplier = 1f;
    [SerializeField] private float fallingGravityMultiplier = 1.5f;

    [Header("Debug")]
    [SerializeField] private float horizontalInput = 0f;
    [SerializeField] private float verticalInput = 0f;
    [SerializeField] private float horizontalSmooth = 0f;
    [SerializeField] private float verticalSmooth = 0f;
    [SerializeField] private float chargePercent = 0f;
    [SerializeField] private Vector2 dashVelocity = Vector2.zero;
    [SerializeField] private float dashEndTime = 0f;
    [SerializeField] private int dashCount = 0;
    [SerializeField] private float lastGroundedTime = 0f;
    [SerializeField] private int platformCount = 0;
    [SerializeField] private bool isOnPlatform = false;
    [SerializeField] private bool isChargeJumping = false;

    private SpriteRenderer sprite;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        ReadInput();
        UpdateSprite();
        UpdateMovement();
        UpdateJumping();
        CountChargePercent();
    }

    private void ReadInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalSmooth = Input.GetAxis("Horizontal");
        verticalSmooth = Input.GetAxis("Vertical");
    }

    private void UpdateSprite()
    {
        if (horizontalInput != 0)
        {
            if (horizontalInput > 0)
            {
                sprite.flipX = true;
            }
            else
            {
                sprite.flipX = false;
            }
        }

        Vector3 newScale = sprite.transform.localScale;
        newScale.y = 0.25f * (1 - 0.25f / 100 * chargePercent);
        sprite.transform.localScale = newScale;
    }

    private void UpdateMovement()
    {
        if (isChargeJumping)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            return;
        }
        rb.gravityScale = gravityScale * ((rb.velocity.y < 0) ? fallingGravityMultiplier : risingGravityMultiplier);

        Vector2 newVelocity = rb.velocity;

        if (Time.time < dashEndTime)
        {
            newVelocity = dashVelocity;
            newVelocity.x += horizontalSmooth * moveSpeed;
        }
        else
        {
            newVelocity.x = horizontalSmooth * moveSpeed;
        }

        rb.velocity = newVelocity;
    }

    private void UpdateJumping()
    {

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
        {

            // RealJump
            if (isOnPlatform)
            {
                isChargeJumping = true;
            }
            else
            {
                if (Time.time - lastGroundedTime < groundedDelay) return;
                Dash();
            }
        }
        if (isChargeJumping && (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.JoystickButton0)))
        {
            Jump(true);
            chargePercent = 0;
            isChargeJumping = false;
        }
    }

    private void Jump(bool isChargeJump)
    {
        float jumpForce = jumpPower;
        if (isChargeJump)
        {
            jumpForce += chargePercent / 100 * jumpPower;
        }
        //Vector2 direction = (Vector2.up + Vector2.right * horizontalInput + Vector2.up * verticalInput).normalized;
        Vector2 direction = Vector2.up;
        rb.AddForce(jumpForce * direction, ForceMode2D.Impulse);
    }

    private void Dash()
    {
        if (dashCount >= maxDashCount) return;
        Vector2 direction = (new Vector2(horizontalInput, verticalInput)).normalized;
        dashVelocity = dashSpeed * direction;
        dashEndTime = Time.time + dashDuration;
        dashCount++;
    }

    private void CountChargePercent()
    {
        if (!isChargeJumping) return;

        chargePercent += (100f / chargeDuration) * Time.deltaTime;
        if (chargePercent > 100)
        {
            chargePercent = 100;
        }
    }

    private void AddPlatformCount(int count)
    {
        platformCount += count;
        SetOnPlatform(platformCount > 0);
    }

    private void SetOnPlatform(bool onPlatform)
    {
        if (isOnPlatform == onPlatform) return;
        isOnPlatform = onPlatform;
        if (isOnPlatform)
        {
            dashCount = 0;
        }
        else
        {
            lastGroundedTime = Time.time;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Platform>(out _))
        {
            AddPlatformCount(1);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Platform>(out _))
        {
            AddPlatformCount(-1);
        }
    }
}
