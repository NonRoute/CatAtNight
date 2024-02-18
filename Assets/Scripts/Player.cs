using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float jumpPower = 250f;
    [SerializeField] private float chargeDuration = 1f;
    [SerializeField] private float dashPower = 200f;
    [SerializeField] private int maxDashCount = 1;
    [SerializeField] private float additionalRisingMultiplier = 1.5f;
    [SerializeField] private float additionalFallingMultiplier = 1.5f;

    [Header("Debug")]
    [SerializeField] private float horizontalInput = 0f;
    [SerializeField] private float verticalInput = 0f;
    [SerializeField] private float chargePercent = 0f;
    [SerializeField] private int dashCount = 0;
    [SerializeField] private int platformCount = 0;
    [SerializeField] private bool isOnPlatform = false;
    [SerializeField] private bool isChargeJumping = false;
    [SerializeField] private bool isControlling = true;

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
        horizontalInput = Input.GetAxis("Horizontal");
        if(isOnPlatform && horizontalInput != 0)
        {
            isControlling = true;
        }
        verticalInput = Input.GetAxis("Vertical");
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
        if(isChargeJumping)
        {
            Vector2 newVelocity = rb.velocity;
            newVelocity.x = 0;
            rb.velocity = newVelocity;
            return;
        }

        if(rb.velocity.y < 0)
        {
            Vector3 newPos = transform.position;
            newPos.y += rb.velocity.y * additionalFallingMultiplier * Time.deltaTime;
            transform.position = newPos;
        }
        else
        {
            Vector3 newPos = transform.position;
            newPos.y += rb.velocity.y * additionalRisingMultiplier * Time.deltaTime;
            transform.position = newPos;
        }

        if(isOnPlatform && isControlling)
        {
            Vector2 newVelocity = rb.velocity;
            newVelocity.x = horizontalInput * moveSpeed;
            rb.velocity = newVelocity;
        }
        else
        {
            Vector3 newPos = transform.position;
            newPos.x += horizontalInput * moveSpeed * Time.deltaTime;
            transform.position = newPos;
        }
    }

    private void UpdateJumping()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Try to Go down
            if (isOnPlatform && verticalInput < 0) return;

            // RealJump
            if (isOnPlatform)
            {
                isChargeJumping = true;
            }
            else
            {
                Dash();
            }
        }
        if (isChargeJumping && Input.GetKeyUp(KeyCode.Space))
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
        rb.AddForce(jumpForce * Vector2.up);
        isControlling = false;
    }

    private void Dash()
    {
        if (dashCount >= maxDashCount) return;
        rb.velocity = 2f * Vector2.up;
        rb.AddForce(dashPower * (new Vector2(horizontalInput, verticalInput)).normalized);
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
        if(isOnPlatform)
        {
            dashCount = 0;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent<Platform>(out _))
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
