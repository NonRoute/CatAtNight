using UnityEngine;

public partial class Player : MonoBehaviour, IDamagable
{
  private void SwitchMode(bool toLiquid)
  {
    if (isLiquid == toLiquid) return;
    isLiquid = toLiquid;
    Vector2 position = rb.position;
    Vector2 velocity = rb.velocity;
    if (isLiquid)
    {
      normal_rb.bodyType = RigidbodyType2D.Kinematic;
      sprite.gameObject.SetActive(false);
      normalColliders.SetActive(false);
      liquid_rb.transform.position = position;
      for (int i = 0; i < bone_rigidbodies.Length; i++)
      {
        bone_rigidbodies[i].transform.localPosition = bone_localPositions[i];
        bone_rigidbodies[i].transform.localRotation = bone_localRotations[i];
      }
      liquidForm.SetActive(true);
      //liquid_rb.velocity = velocity;
      liquid_rb.AddForce(5f * velocity, ForceMode2D.Impulse);
      rb = liquid_rb;
    }
    else
    {
      liquidForm.SetActive(false);
      transform.position = position;
      sprite.gameObject.SetActive(true);
      normalColliders.SetActive(true);
      normal_rb.bodyType = RigidbodyType2D.Dynamic;
      normal_rb.velocity = Vector2.zero;
      rb = normal_rb;
    }
    lastSetPlayerPosTime = Time.time;
    playerPosition.position = position;
  }

  private void UpdateLiquidMode()
  {
    bool onPlatform = Physics2D.OverlapCircle(l_platformCheck.position, 0.3f, platformLayer);
    SetOnPlatform(onPlatform || platformCount > 0);
    UpdateGravity();

    // Update Interrupted
    if (isInterrupted)
    {
      if (Time.time - lastInterruptedTime > interruptedDuration)
      {
        isInterrupted = false;
      }
      return;
    }

    // Update Jump
    if (isOnPlatform && Input.GetKeyDown(KeyCode.Space))
    {
      rb.AddForce(l_jumpPower * Vector2.up, ForceMode2D.Impulse);
    }

    // Update Movement
    if (horizontalSmooth != 0)
    {
      RaycastHit2D hit = Physics2D.Raycast(origin: rb.transform.position, direction: Vector2.down
          , distance: 1f, layerMask: platformLayer);

      if (hit.collider != null)
      {
        Vector2 perpendicular = Vector2.Perpendicular(hit.normal);
        //print(perpendicular);
        Debug.DrawLine((Vector2)rb.transform.position, (Vector2)rb.transform.position + perpendicular, Color.yellow, 2.0f);
        rb.AddForce(l_moveSpeed * horizontalSmooth * -perpendicular.normalized);
      }
      else
      {
        rb.AddForce(l_moveSpeed * horizontalSmooth * Vector2.right);
      }
    }

    bool isDashing = Time.time < l_dashEndTime;
    Vector2 newVelocity = rb.velocity;

    if (isDashing)
    {
      //float lerpRate = (dashEndTime - Time.time) / dashDuration;
      newVelocity = l_dashVelocity;
    }

    rb.velocity = newVelocity;

    // Update Dashing
    if (!isOnPlatform && Input.GetKeyDown(KeyCode.Space))
    {
      if (dashCount >= maxDashCount) return;
      Vector2 direction = (new Vector2(horizontalInput, verticalInput)).normalized;
      if (direction == Vector2.zero) return;
      l_dashVelocity = l_dashSpeed * direction;
      l_dashEndTime = Time.time + l_dashDuration;
      dashCount++;
    }
  }
}