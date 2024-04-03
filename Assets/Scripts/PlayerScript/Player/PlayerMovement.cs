using System.Collections.Generic;
using UnityEngine;

public partial class Player : MonoBehaviour, IDamagable
{
    private void UpdateCameraFollowPosition()
    {
        // Have some delay between each time to prevent camera shaking
        if (Time.time - lastSetCameraFollowTime < cameraFollowDelay)
            return;
        // Set position to current rigidbody (current form) position
        cameraFollowTransform.position = rb.transform.position;
    }

    private void UpdateSprite()
    {
        if (horizontalInput != 0)
        {
            FlipSprite(horizontalInput > 0);
        }
        Vector3 newScale = sprite.transform.localScale;
        newScale.y = (1 - 0.25f / 100 * chargePercent) * initialScaleY;
        sprite.transform.localScale = newScale;

        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        animator.SetBool("is_grounded", isGrounded);
        animator.SetBool("is_charging", isChargeJumping);
        animator.SetBool("is_running", isRunning);
        animator.SetBool("is_walled", isWalled);
        animator.SetBool("is_sliding", isWallSliding);
        animator.SetBool("is_climbing", isClimbingWall);
        animator.SetBool("is_walking", !isRunning && horizontalInput != 0 && !isChargeJumping);
        animator.SetFloat("speed_y", rb.velocity.y);
    }

    private void FlipSprite(bool isFlip)
    {
        sprite.flipX = !isFlip;
        isFacingRight = isFlip;
        wallCheckPivot.transform.localScale = new(isFlip ? -1 : 1, 1, 1);
    }

    private void RotateSprite(float angle)
    {
        sprite.transform.localEulerAngles = new Vector3(0, 0, angle);
    }

    /// <summary>
    /// Interrupt is Uncontrollable State ex. being damaged, being bounced
    /// </summary>
    private void UpdateWhenInterrupted()
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
            DrainStamina(runningStaminaDrainMultiplier);
        }
        else if (Time.time - lastRunningTime > staminaRegenDelay)
        {
            stamina += baseStaminaRegenRate * Time.deltaTime;
            stamina = Mathf.Min(maxStamina, stamina);
        }
        StatusUIManager.Instance.UpdateStaminaBar(stamina / maxStamina, isStaminaOut);

        if (stamina >= minimumStamina)
        {
            isStaminaOut = false;
        }

        if (isLiquid || isInPipe)
        {
            isRunning = false;
            return;
        }

        bool wasRunning = isRunning;

        isRunning = isTryingToRun && !isStaminaOut && !isChargeJumping;

        if (wasRunning && !isRunning)
        {
            lastRunningTime = Time.time;
        }
    }

    private void ReduceStamina(float amount)
    {
        stamina = Mathf.Max(stamina - amount, 0f);
        if (stamina <= 0)
        {
            isStaminaOut = true;
        }
    }

    private void UseStamina(float usageMultiplier)
    {
        ReduceStamina(baseStaminaUsage * usageMultiplier);
    }

    private void DrainStamina(float rateMultiplier)
    {
        ReduceStamina(baseStaminaDrainRate * rateMultiplier * Time.deltaTime);
    }

    private void UpdatePhysicsCondition()
    {
        isWalled = Physics2D.OverlapCircle(wallCheckTransform.position, 0.2f, wallLayer);

        bool grounded = false;

        RaycastHit2D hit = Physics2D.Raycast(origin: playerFeetTransform.position, direction: Vector2.down
          , distance: 1f, layerMask: platformLayer);

        bool isRotated = false;
        if (hit.collider != null)
        {
            //Debug.DrawLine((Vector2)platformCheck.position, (Vector2)platformCheck.position - hit.normal, Color.yellow, 2.0f);
            RaycastHit2D hit_rotated = Physics2D.Raycast(playerFeetTransform.position, -hit.normal, 0.6f, platformLayer);
            if (hit_rotated.collider != null)
            {
                float rightAngle = Vector2.Angle(hit_rotated.normal, Vector2.right);
                float leftAngle = Vector2.Angle(hit_rotated.normal, Vector2.left);
                float rotateAngle = Vector2.Angle(hit_rotated.normal, Vector2.up);
                if(Mathf.Abs(rotateAngle) > 30f)
                {
                    isRotated = true;
                    RotateSprite(((rightAngle < leftAngle) ? -1 : 1) * rotateAngle);
                    sprite.transform.localPosition = initialSpritePos - (Vector3)hit_rotated.normal * 0.2f;
                    grounded |= hit.distance <= 0.45f;
                }
                else
                {
                    grounded |= hit.distance <= 0.3f;
                }
            }
        }
        if(!isRotated)
        {
            sprite.transform.localPosition = initialSpritePos;
            RotateSprite(0);
        }
        SetGrounded(grounded);

        // Check FLoating
        if(grounded && rb.velocity.y <= 0)
        {
            isFloating = false;
        }

        // For One-way Slope
        RaycastHit2D hit_vel = Physics2D.Raycast(origin: playerFeetTransform.position, direction: rb.velocity.x * Vector2.right
          , distance: 0.5f, layerMask: platformLayer);
        if (hit_vel.collider != null)
        {
            float angle = Vector2.Angle(hit_vel.normal, Vector2.up);
            if (Mathf.Abs(angle) > 30f && hit_vel.normal.y > 0)
            {
                isFloating = false;
            }
        }
    }

    private void SetGrounded(bool grounded)
    {
        if (isGrounded == grounded)
            return;
        isGrounded = grounded;
        if (isGrounded)
        {
            dashCount = 0;
        }
        else
        {
            lastGroundedTime = Time.time;
        }
    }

    private void UpdateMovement()
    {
        if (isChargeJumping)
        {
            // Much Slower Movement when Charging Jump (10%)
            // May change direction to match the terrain in the future
            rb.velocity = new Vector2(horizontalSmooth * moveSpeed * 0.1f, 0);
            rb.gravityScale = 0;
            if (!isGrounded)
            {
                Jump(true);
            }
            return;
        }
        UpdateGravity();

        Vector2 newVelocity = rb.velocity;

        //newVelocity.y = Mathf.Clamp(newVelocity.y, -maxVelY, maxVelY);

        float baseSpeed = (isFloating) ? floatSpeed : moveSpeed;
        if (isRunning)
        {
            baseSpeed *= runMultiplier;
        }

        bool isDashing = Time.time < dashEndTime;
        bool isWallJumping = Time.time < wallJumpEndTime;

        if (isDashing)
        {
            float lerpRate = (dashEndTime - Time.time) / dashDuration;
            newVelocity = dashVelocity * lerpRate;
            float sign = Mathf.Sign(newVelocity.x);
            newVelocity.x = sign * Mathf.Max(sign * newVelocity.x, sign * horizontalSmooth * baseSpeed);
            FlipSprite(dashVelocity.x > 0);
        }
        else if (isWallJumping)
        {
            float lerpRate = (wallJumpEndTime - Time.time) / wallJumpDuration;
            newVelocity = wallJumpVelocity * lerpRate;
            newVelocity.x += horizontalSmooth * moveSpeed;
            newVelocity.x += wallJumpVelocity.x * 0.5f;
            FlipSprite(wallJumpVelocity.x > 0);
        }
        else
        {
            newVelocity.x = horizontalSmooth * baseSpeed;
        }

        isWallSliding = false;
        isClimbingWall = false;
        if (isWalled && !isGrounded && horizontalInput != 0)
        {
            // Wall Slide
            float newVelY = Mathf.Max(newVelocity.y, -wallSlideSpeed);
            isWallSliding = true;
            // Wall Climb
            if (isRunning && horizontalInput * (isFacingRight ? 1 : -1) > 0 && !isStaminaOut)
            {
                newVelY = wallClimbSpeed;
                DrainStamina(wallClimbStaminaDrainMultiplier);
                isClimbingWall = true;
            }
            newVelocity.y = newVelY;
        }
            
        // Only limit vertical velocity when !isFloating right now
        //newVelocity.y = Mathf.Min(newVelocity.y,isFloating ? maxFloatingVelocity : maxVerticalVelocity);
        if(!isFloating)
        {
            newVelocity.y = Mathf.Min(newVelocity.y, maxVerticalVelocity);
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
        bool pressedJump = playerInputActions.Player.Jump.WasPressedThisFrame();
        bool releasedJump = playerInputActions.Player.Jump.WasReleasedThisFrame();
        bool dropDown = playerInputActions.Player.DropDown.WasPressedThisFrame();

        bool isDropDown = false;
        if ((pressedJump && verticalInput < 0) || dropDown)
        {
            List<Platform> toBeDisable = new(passThroughPlatformList);
            foreach(Platform platform in toBeDisable)
            {
                isDropDown = true;
                platform.TemporaryDisableCollider();
            }
            if (isDropDown) return;
        }

        if (isWallSliding)
        {
            if (pressedJump && !isStaminaOut)
            {
                WallJump();
            }
            return;
        }

        if (isChargeJumping)
        {
            IncreaseChargeJumpPercent();
        }

        if (pressedJump)
        {
            if (isGrounded)
            {
                // Run Jump
                if (isRunning)
                {
                    Jump(false);
                    UseStamina(runJumpStaminaUsageMultiplier);
                }
                else
                {
                    isChargeJumping = true;
                }
            }
            else
            {
                // Prevent Dashing when Dropping Down
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
        isFloating = true;
        float jumpForce = jumpPower;
        if (isChargeJump)
        {
            jumpForce += chargePercent / 100 * jumpPower;
            chargePercent = 0;
            isChargeJumping = false;
        }
        rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
    }

    private void WallJump()
    {
        isFloating = true;
        Vector2 oppositeDirection = Vector2.left * (isFacingRight ? 1 : -1);
        Vector2 jumpDirection = (2f * oppositeDirection + Vector2.up).normalized;
        wallJumpVelocity = wallJumpSpeed * jumpDirection;
        wallJumpEndTime = Time.time + wallJumpDuration;
        animator.SetBool("is_sliding", isWallSliding);
        FlipSprite(!isFacingRight);
        UseStamina(wallJumpStaminaUsageMultiplier);
    }

    private void Dash()
    {
        isFloating = true;
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
        chargePercent += (100f / chargeDuration) * Time.deltaTime;
        if (chargePercent > 100)
        {
            chargePercent = 100;
        }
    }
}
