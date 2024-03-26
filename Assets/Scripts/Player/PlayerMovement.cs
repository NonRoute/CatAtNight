using System;
using UnityEngine;

public partial class Player : MonoBehaviour, IDamagable
{
    private void UpdatePlayerPosition()
    {
        if (Time.time - lastSetPlayerPosTime < playerPosDelay)
            return;
        playerPosition.position = rb.transform.position;
        //playerPosition.position = Vector3.Lerp(playerPosition.position, rb.transform.position,0.8f);
    }

    private void UpdateSprite()
    {
        if (horizontalInput != 0)
        {
            FlipSprite(horizontalInput > 0);
            if (horizontalInput > 0)
            {
                sprite.flipX = false;
                isFacingRight = true;
                wallCheckPivot.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                sprite.flipX = true;
                isFacingRight = false;
                wallCheckPivot.transform.localScale = new Vector3(1, 1, 1);
            }
        }
        Vector3 newScale = sprite.transform.localScale;
        newScale.y = (1 - 0.25f / 100 * chargePercent) * initialScaleY;
        sprite.transform.localScale = newScale;
    }

    private void FlipSprite(bool isFlip)
    {
        sprite.flipX = !isFlip;
        isFacingRight = isFlip;
        wallCheckPivot.transform.localScale = new Vector3(isFlip ? -1 : 1, 1, 1);
    }

    /// <summary>
    /// Interrupt is Uncontrollable State ex. being damaged, being bounced
    /// </summary>
    private void UpdateInterrupted()
    {
        UpdateGravity();
        float currentTime = Time.time;
        if (isBouncing)
        {
            if (currentTime - lastBounceTime > bounceDuration)
            {
                isBouncing = false;
            }
            else
            {
                float lerpRate = (lastBounceTime + bounceDuration - Time.time) / bounceDuration;
                rb.velocity = bounceVelocity * lerpRate;
            }
        }
        if (currentTime - lastInterruptedTime > interruptedDuration)
        {
            isInterrupted = false;
        }
    }

    private void UpdateStamina()
    {
        if (isRunning && horizontalInput != 0)
        {
            UseStamina(staminaDrainRate * Time.deltaTime);
        }
        else if (Time.time - lastRunningTime > staminaRegenDelay)
        {
            stamina += staminaRegenRate * Time.deltaTime;
            stamina = Mathf.Min(maxStamina, stamina);
        }
        StatusUIManager.Instance.UpdateStaminaBar(stamina / maxStamina, isStaminaOut);

        if (stamina >= minimumStamina)
        {
            isStaminaOut = false;
        }

        bool wasRunning = isRunning;

        if (isTryingToRun)
        {
            isRunning = !isStaminaOut;
        }
        else
        {
            isRunning = false;
        }

        if (wasRunning && !isRunning)
        {
            lastRunningTime = Time.time;
        }
    }

    private void UseStamina(float usage)
    {
        stamina = Mathf.Max(stamina - usage, 0f);
        if (stamina <= 0)
        {
            isStaminaOut = true;
        }
    }

    private void UpdateCollideCondition()
    {
        bool onPlatform = Physics2D.OverlapCircle(platformCheck.position, 0.3f, platformLayer);
        SetOnPlatform(onPlatform || platformCount > 0);
        isGroundedDelay = (Time.time - lastGroundedTime > 0.5f) && isOnPlatform;
        //isFloatingDelay = isGroundedDelay && (Time.time - lastGroundedTime > 0.2f);
        isWalled = Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void UpdateMovement()
    {
        if (isChargeJumping)
        {
            //rb.velocity = Vector2.zero;
            //.velocity = new Vector2(horizontalSmooth * moveSpeed * Math.Max(0.1f, 1 - chargePercent / 100), 0);
            rb.velocity = new Vector2(horizontalSmooth * moveSpeed * 0.1f, 0);
            rb.gravityScale = 0;
            return;
        }
        UpdateGravity();

        Vector2 newVelocity = rb.velocity;

        //float angle = 0f;
        //RaycastHit2D hit = Physics2D.Raycast(origin: l_platformCheck.position, direction: Vector2.down
        //, distance: 1f, layerMask: platformLayer);
        //if (hit.collider != null)
        //{
        //    angle = Vector2.Angle(Vector2.up, hit.normal);
        //}
        //bool isGrounded = isOnPlatform && (Mathf.Abs(rb.velocity.y) <= 0.1f || angle > 0f);

        //bool isGrounded = isOnPlatform && Mathf.Abs(rb.velocity.y) <= 0.1f
        bool isGrounded = isOnPlatform && isGroundedDelay;
        float speed = (isGrounded) ? moveSpeed : floatSpeed;
        if (isRunning)
        {
            speed *= runMultiplier;
        }

        bool isDashing = Time.time < dashEndTime;
        bool isWallJumping = Time.time < wallJumpEndTime;

        if (isDashing)
        {
            float lerpRate = (dashEndTime - Time.time) / dashDuration;
            newVelocity = dashVelocity * lerpRate;
            newVelocity.x += horizontalSmooth * speed;
            FlipSprite(dashVelocity.x > 0);
        }
        else if (isWallJumping)
        {
            float lerpRate = (wallJumpEndTime - Time.time) / wallJumpDuration;
            newVelocity = wallJumpVelocity * lerpRate;
            newVelocity.x += 0.5f * horizontalSmooth * moveSpeed;
            FlipSprite(wallJumpVelocity.x > 0);
        }
        else
        {
            newVelocity.x = horizontalSmooth * speed;
        }

        isClimbingWall = false;
        if (isWalled && !isGrounded && horizontalInput != 0)
        {
            // Wall Slide
            float newVelY = Mathf.Max(newVelocity.y, -wallSlideSpeed);
            // Wall Climb
            if (isRunning && horizontalInput * (isFacingRight ? 1 : -1) > 0 && !isStaminaOut)
            {
                newVelY = wallClimbSpeed;
                UseStamina(wallClimbStaminaDrain * Time.deltaTime);
                isClimbingWall = true;
            }
            newVelocity.y = newVelY;
        }

        rb.velocity = newVelocity;
    }

    private void UpdateGravity()
    {
        rb.gravityScale =
            gravityScale
            * ((rb.velocity.y < 0) ? fallingGravityMultiplier : risingGravityMultiplier);
    }

    private void UpdateJumping()
    {
        //bool pressedJump = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0);
        //bool releasedJump = Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.JoystickButton0);
        bool pressedJump = playerInputActions.Player.Jump.WasPressedThisFrame();
        bool releasedJump = playerInputActions.Player.Jump.WasReleasedThisFrame();

        if (isClimbingWall)
        {
            if (pressedJump)
            {
                WallJump();
            }
            return;
        }

        if (pressedJump)
        {
            // RealJump
            if (isOnPlatform)
            {
                if (isRunning)
                {
                    Jump(false);
                    UseStamina(runJumpStaminaUsage);
                }
                else
                {
                    isChargeJumping = true;
                }
            }
            else
            {
                if (Time.time - lastGroundedTime < groundedDelay)
                    return;
                Dash();
            }
        }
        if (isChargeJumping && releasedJump)
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

    private void WallJump()
    {
        Vector2 oppositeDirection = Vector2.left * (isFacingRight ? 1 : -1);
        Vector2 jumpDirection = (2f * oppositeDirection + Vector2.up).normalized;
        wallJumpVelocity = wallJumpSpeed * jumpDirection;
        wallJumpEndTime = Time.time + wallJumpDuration;
        FlipSprite(!isFacingRight);
        UseStamina(wallJumpStaminaUsage);
    }

    private void Dash()
    {
        if (dashCount >= maxDashCount)
            return;
        Vector2 direction = (new Vector2(horizontalInput, verticalInput)).normalized;
        if (direction == Vector2.zero)
            return;
        dashVelocity = dashSpeed * direction;
        dashEndTime = Time.time + dashDuration;
        wallJumpEndTime = Time.time;
        dashCount++;
    }

    private void IncreaseChargeJumpPercent()
    {
        if (!isChargeJumping)
            return;

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
        if (isOnPlatform == onPlatform)
            return;
        isOnPlatform = onPlatform;
        if (isOnPlatform)
        {
            //lastFloatingTime = Time.time;
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

    private void UpdateAnimation()
    {
        animator.SetBool("is_grounded", isOnPlatform);
        //animator.SetBool("is_grounded", isOnPlatform && (Time.time-lastGroundedTime > 0.2f));
        animator.SetBool("is_charging", isChargeJumping);
        animator.SetBool("is_running", isRunning);
        animator.SetBool("is_walled", isWalled);
        animator.SetBool("is_climbing", isClimbingWall);
        animator.SetBool("is_walking", !isRunning && horizontalInput != 0 && !isChargeJumping);
        animator.SetFloat("speed_y", rb.velocity.y);
    }
}
