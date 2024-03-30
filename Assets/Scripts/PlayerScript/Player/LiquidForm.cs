using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class Player : MonoBehaviour, IDamagable
{
    private void FixedUpdate()
    {
        // Reserved for Liquid Form
        if (!isLiquid) return;
        if (isFreeze)
        {
            rb.velocity = new(0f,rb.velocity.y);
            foreach(Rigidbody2D rb in boneRigidbodies)
            {
                rb.velocity = new(0f, rb.velocity.y);
            }
        }
        if (isInterrupted) return;

        // Update Movement
        if (horizontalSmooth != 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin: rb.transform.position, direction: Vector2.down
                , distance: 1f, layerMask: platformLayer);

            if (hit.collider != null)
            {
                Vector2 perpendicular = Vector2.Perpendicular(hit.normal);
                //print(perpendicular);
                //Debug.DrawLine((Vector2)rb.transform.position, (Vector2)rb.transform.position + perpendicular, Color.yellow, 2.0f);
                float forcePower = liquidMoveForce * horizontalSmooth * (isFloating ? liquidFloatingMultiplier : 1);
                rb.AddForce(forcePower * -perpendicular.normalized);
            }
            else
            {
                rb.AddForce(liquidMoveForce * horizontalSmooth * Vector2.right);
            }
        }

        // Update Dashing Movement
        Vector2 newVelocity = rb.velocity;
        if (Time.time < liquidDashEndTime)
        {
            newVelocity = liquidDashVelocity;
        }

        // Limit Max Velocity
        newVelocity.x = Mathf.Clamp(newVelocity.x,-liquidMaxHorizontalVelocity,liquidMaxHorizontalVelocity);
        if(!isFloating)
        {
            newVelocity.y = Mathf.Min(newVelocity.y, liquidMaxVerticalVelocity);
        }

        rb.velocity = newVelocity;
        //if(newVelocity.sqrMagnitude > 400) // Debug Player Momentum
        //{
        //    print(newVelocity.sqrMagnitude);
        //}
    }

    private void SwitchMode(bool toLiquid)
    {
        if (isLiquid == toLiquid) return;
        isLiquid = toLiquid;
        Vector2 position = rb.position;
        Vector2 velocity = rb.velocity;
        if (isLiquid)
        {
            normal_rb.bodyType = RigidbodyType2D.Kinematic;
            spriteObject.SetActive(false);
            normalColliders.SetActive(false);
            liquid_rb.transform.position = position;
            for (int i = 0; i < boneRigidbodies.Length; i++)
            {
                boneRigidbodies[i].transform.SetLocalPositionAndRotation(boneLocalPositions[i], bone_localRotations[i]);
            }
            liquidForm.SetActive(true);
            //liquid_rb.velocity = velocity; // This doesn't work because player velocity is too low
            // Use add force instead of reserve velocity, Still use the magic number though
            liquid_rb.AddForce(5f * velocity, ForceMode2D.Impulse);
            rb = liquid_rb;
        }
        else
        {
            liquidForm.SetActive(false);
            transform.position = position;
            spriteObject.SetActive(true);
            normalColliders.SetActive(true);
            normal_rb.bodyType = RigidbodyType2D.Dynamic;
            normal_rb.velocity = Vector2.zero;
            rb = normal_rb;
        }
        lastSetCameraFollowTime = Time.time;
        cameraFollowTransform.position = position;
        StatusUIManager.Instance.ToggleLiquidImage(isLiquid);
    }

    private void UpdateLiquidMode()
    {
        // Update Physics Condition
        UpdateLiquidPhysicsCondition();
        // Update Interrupted
        if (isInterrupted)
        {
            // Do nothing if Interrupted
            if (Time.time - lastInterruptedTime > interruptedDuration)
            {
                isInterrupted = false;
            }
            return;
        }

        // Update Jumping and Dashing
        UpdateLiquidJumping();

        // Movement Part is in Fixed Update
    }

    private void UpdateLiquidPhysicsCondition()
    {
        UpdateGravity();

        RaycastHit2D hit = Physics2D.Raycast(origin: liquidPlayerBottomTransform.position, direction: Vector2.down
          , distance: 0.6f, layerMask: platformLayer);

        bool grounded = false;

        if (hit.collider != null)
        {
            //Debug.DrawLine((Vector2)liquidPlayerBottomTransform.position, (Vector2)liquidPlayerBottomTransform.position - hit.normal * 0.4f, Color.yellow, 2.0f);
            grounded = true;
        }
        SetGrounded(grounded);

        // Check Floating
        if (grounded && rb.velocity.y < 0)
        {
            isFloating = false;
        }
    }

    private void UpdateLiquidJumping()
    {
        bool pressedJump = playerInputActions.Player.Jump.WasPressedThisFrame();
        bool dropDown = playerInputActions.Player.DropDown.WasPressedThisFrame();

        bool isDropDown = false;
        if ((pressedJump && verticalInput < 0) || dropDown)
        {
            List<Platform> toBeDisable = new(passThroughPlatformList);
            foreach (Platform platform in toBeDisable)
            {
                isDropDown = true;
                platform.TemporaryDisableCollider();
            }
            if (isDropDown) return;
        }

        if(pressedJump)
        {
            if (isGrounded) // Jump
            {
                rb.AddForce(liquidJumpForce * Vector2.up, ForceMode2D.Impulse);
                isFloating = true;
            }
            else // Dash
            {
                if (dashCount >= maxDashCount) return;
                Vector2 direction = (new Vector2(horizontalInput, verticalInput)).normalized;
                if (direction == Vector2.zero) return;
                liquidDashVelocity = liquidDashSpeed * direction;
                liquidDashEndTime = Time.time + liquidDashDuration;
                dashCount++;
            }
        }
    }

    private void UpdateInPipe()
    {
        if(Time.time < travelEndTime)
        {
            Vector3 newPosition = (travelEndTime - Time.time) * GetPipePosition(pipeIndex-1) + (Time.time - travelStartTime) * GetPipePosition(pipeIndex);
            newPosition /= travelEndTime - travelStartTime;
            transform.position = newPosition;
            return;
        }

        ++pipeIndex;

        if(pipeIndex >= currentPipe.GetVertices(isPipeForward).Count)
        {
            EndMovePipe();
            return;
        }

        travelStartTime = travelEndTime;
        travelEndTime += GetPipeDuration(pipeIndex);
    }

    private void StartMovePipe(PipeEnd pipeEnd)
    {
        isPipeForward = pipeEnd.IsForward;
        currentPipe = pipeEnd.Pipe;
        isInPipe = true;
        pipeIndex = 0;
        positionBeforeEnter = rb.position;
        timeBeforeEnter = Time.time;
        travelStartTime = timeBeforeEnter;
        travelEndTime = timeBeforeEnter + GetPipeDuration(pipeIndex);

        SwitchMode(false);
        spriteObject.SetActive(false);
        staticLiquidSprite.SetActive(true);
        normalColliders.SetActive(false);
        normal_rb.bodyType = RigidbodyType2D.Kinematic;
        normal_rb.gravityScale = 0f;
        StatusUIManager.Instance.ToggleLiquidImage(true);
    }
    private void EndMovePipe()
    {
        staticLiquidSprite.SetActive(false);
        SwitchMode(true); 
        liquidDashVelocity = currentPipe.GetEndVelocity(isPipeForward);
        liquidDashEndTime = Time.time + 0.2f;
        rb.velocity = currentPipe.GetEndVelocity(isPipeForward);
        isInPipe = false;
    }

    private Vector3 GetPipePosition(int index)
    {
        if (index < 0) return positionBeforeEnter;
        return currentPipe.GetVertices(isPipeForward)[index].vertex.position;
    }
    private float GetPipeDuration(int index)
    {
        if (index < 0) return timeBeforeEnter;
        return currentPipe.GetVertices(isPipeForward)[index].timeToTravel;
    }
}