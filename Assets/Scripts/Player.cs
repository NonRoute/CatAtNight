using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamagable
{
    private const bool IS_DEBUG = true;

    [Header("Status")]
    [SerializeField] private float health = 9f;
    [SerializeField] private float maxHealth = 9f;
    [SerializeField] private float stamina = 100f;
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float immortalDuration = 1f;
    [Header("Power Values")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float floatSpeed = 10f;
    [SerializeField] private float runMultiplier = 2f;
    [SerializeField] private float staminaDrainRate = 20f;
    [SerializeField] private float staminaRegenRate = 10f;
    [SerializeField] private float staminaRegenDelay = 1f;
    [SerializeField] private float wallSlideSpeed = 3f;
    [SerializeField] private float wallClimbSpeed = 8f;
    [SerializeField] private float wallClimbStaminaDrain = 2f;
    [SerializeField] private float wallJumpSpeed = 12f;
    [SerializeField] private float wallJumpDuration = 0.3f;
    [SerializeField] private float wallJumpStaminaUsage = 10f;
    [SerializeField] private float runJumpStaminaUsage = 10f;
    [SerializeField] private float minimumStamina = 30f;
    [SerializeField] private float jumpPower = 8f;
    [SerializeField] private float chargeDuration = 0.75f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private int maxDashCount = 1;
    [SerializeField] private float groundedDelay = 0.1f;
    [Header("Gravity Values")]
    [SerializeField] private float gravityScale = 3f;
    [SerializeField] private float risingGravityMultiplier = 1f;
    [SerializeField] private float fallingGravityMultiplier = 1.5f;
    [Header("Collider Values")]
    [SerializeField] private Transform platformCheck;
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private Transform wallCheckPivot;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;
    [Header("Zone 1")]
    [SerializeField] private GameObject yarnBall;
    [SerializeField] private Vector3 facingRightOffset;
    [SerializeField] private Vector3 facingLeftOffset;
    private GameObject pickedUpYarnBall;
    private Vector2 yarnBallVel;
    [SerializeField] private float yarnBallSmoothTime;

    [Header("Liquid Mode Values")]
    [SerializeField] private bool isLiquid = false;
    [SerializeField] private float l_moveSpeed = 25f;
    [SerializeField] private float l_jumpPower = 75f;
    [SerializeField] private Rigidbody2D[] bone_rigidbodies;
    [SerializeField] private Vector3[] bone_localPositions;
    [SerializeField] private Quaternion[] bone_localRotations;
    [SerializeField] private Transform l_platformCheck;
    [SerializeField] private float lastSetPlayerPosTime = 0f;
    [SerializeField] private float playerPosDelay = 0.15f;
    [SerializeField] private float l_dashDuration = 0.2f;
    [SerializeField] private float l_dashSpeed = 15f;
    [SerializeField] private Vector2 l_dashVelocity = Vector2.zero;
    [SerializeField] private float l_dashEndTime = 0f;

    [Header("Debug Values")]
    [SerializeField] private bool isFreeze = false;
    [SerializeField] private bool isInterrupted = false;
    [SerializeField] private float lastInterruptedTime = 0f;
    [SerializeField] private float interruptedDuration = 0f;
    [SerializeField] private bool isBouncing = false;
    [SerializeField] private float lastBounceTime = 0f;
    [SerializeField] private float bounceDuration = 0f;
    [SerializeField] private Vector2 bounceVelocity;
    [SerializeField] private float horizontalInput = 0f;
    [SerializeField] private float verticalInput = 0f;
    [SerializeField] private float horizontalSmooth = 0f;
    [SerializeField] private float verticalSmooth = 0f;
    [SerializeField] private bool isFacingRight = false;
    [SerializeField] private float chargePercent = 0f;
    [SerializeField] private Vector2 dashVelocity = Vector2.zero;
    [SerializeField] private float dashEndTime = 0f;
    [SerializeField] private int dashCount = 0;
    [SerializeField] private Vector2 wallJumpVelocity = Vector2.zero;
    [SerializeField] private float wallJumpEndTime = 0f;
    [SerializeField] private float lastGroundedTime = 0f;
    [SerializeField] private int platformCount = 0;
    [SerializeField] private bool isOnPlatform = false;
    [SerializeField] private bool isWalled = false;
    [SerializeField] private bool isChargeJumping = false;
    [SerializeField] private float lastRunningTime = 0f;
    [SerializeField] private bool isTryingToRun = false;
    [SerializeField] private bool isRunning = false;
    [SerializeField] private bool isClimbingWall = false;
    [SerializeField] private bool isStaminaOut = false;
    [SerializeField] private bool noClip = false;
    [SerializeField] private float lastDamagedTime = 0f;

    [Header("References")]
    [SerializeField] private Transform playerPosition;
    [SerializeField] private GameObject normalColliders;
    [SerializeField] private GameObject liquidForm;
    [SerializeField] private Rigidbody2D liquid_rb;
    private SpriteRenderer sprite;
    private Rigidbody2D normal_rb;
    private Rigidbody2D rb;

    void Start()
    {
        normal_rb = GetComponent<Rigidbody2D>();
        rb = normal_rb;
        sprite = GetComponentInChildren<SpriteRenderer>();
        health = maxHealth;
        stamina = maxStamina;
        bone_rigidbodies = liquid_rb.gameObject.GetComponentsInChildren<Rigidbody2D>();
        bone_localPositions = new Vector3[bone_rigidbodies.Length];
        bone_localRotations = new Quaternion[bone_rigidbodies.Length];
        for (int i = 0; i < bone_rigidbodies.Length; i++)
        {
            Transform t = bone_rigidbodies[i].transform;
            bone_localPositions[i] = t.localPosition;
            bone_localRotations[i] = t.localRotation;
        }
        StatusUIManager.Instance.UpdateHealthBar(health, maxHealth);
    }

    void Update()
    {
        UpdatePlayerPosition();
        UpdateGameStatus();
        if (isFreeze)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            return;
        }
        ReadInput();
        if (isLiquid)
        {
            UpdateLiquidMode();
            return;
        }
        UpdateSprite();
        UpdateStamina();
        UpdateCollideCondition();
        if (IS_DEBUG && noClip)
        {
            transform.Translate(3f * moveSpeed * Time.deltaTime * new Vector3(horizontalSmooth, verticalSmooth, 0f));
            return;
        }
        if (isInterrupted)
        {
            UpdateInterrupted();
            return;
        }
        UpdateMovement();
        UpdateJumping();
        CountChargePercent();
        UpdateYarnBall();
    }

    private void UpdatePlayerPosition()
    {
        if (Time.time - lastSetPlayerPosTime < playerPosDelay) return;
        playerPosition.position = rb.transform.position;
    }

    private void UpdateGameStatus()
    {
        bool willFreeze = false;
        if (DialogueManager.Instance != null)
        {
            willFreeze |= DialogueManager.Instance.isDialogueActive;
        }
        isFreeze = willFreeze;
    }

    private void ReadInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalSmooth = Input.GetAxis("Horizontal");
        verticalSmooth = Input.GetAxis("Vertical");
        isTryingToRun = Input.GetKey(KeyCode.LeftControl) && horizontalInput != 0;
        if (Input.GetKeyDown(KeyCode.X))
        {
            SwitchMode(!isLiquid);
        }
        if (IS_DEBUG && Input.GetKeyDown(KeyCode.C))
        {
            noClip = !noClip;
            rb.bodyType = (noClip) ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
            Color newColor = sprite.color;
            newColor.a = (noClip) ? 0.5f : 1f;
            sprite.color = newColor;
            rb.velocity = Vector2.zero;
        }
    }

    private void UpdateSprite()
    {
        if (horizontalInput != 0)
        {
            FlipSprite(horizontalInput > 0);
            if (horizontalInput > 0)
            {
                sprite.flipX = true;
                isFacingRight = true;
                wallCheckPivot.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                sprite.flipX = false;
                isFacingRight = false;
                wallCheckPivot.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        Vector3 newScale = sprite.transform.localScale;
        newScale.y = 0.25f * (1 - 0.25f / 100 * chargePercent);
        sprite.transform.localScale = newScale;
    }

    private void FlipSprite(bool isFlip)
    {
        sprite.flipX = isFlip;
        isFacingRight = isFlip;
        wallCheckPivot.transform.localScale = new Vector3(isFlip ? -1 : 1, 1, 1);
    }

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
                rb.velocity = bounceVelocity;
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
        }

        if (stamina > maxStamina)
        {
            stamina = maxStamina;
        }

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
        isWalled = Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void UpdateMovement()
    {
        if (isChargeJumping)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            return;
        }
        UpdateGravity();

        Vector2 newVelocity = rb.velocity;

        bool isGrounded = isOnPlatform && Mathf.Abs(rb.velocity.y) <= 0.1f;
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
        rb.gravityScale = gravityScale * ((rb.velocity.y < 0) ? fallingGravityMultiplier : risingGravityMultiplier);
    }

    private void UpdateJumping()
    {
        bool pressedJump = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0);
        bool releasedJump = Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.JoystickButton0);

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
                if (Time.time - lastGroundedTime < groundedDelay) return;
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

    private void UpdateYarnBall()
    {
        if (pickedUpYarnBall != null)
        {
            Vector3 offset = isFacingRight ? facingRightOffset : facingLeftOffset;
            pickedUpYarnBall.transform.position = transform.position + offset;
            if (Input.GetMouseButtonDown(0))
            {
                pickedUpYarnBall.GetComponent<YarnBall>().Throw(isFacingRight);
                pickedUpYarnBall = null;
            }
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
        if (dashCount >= maxDashCount) return;
        Vector2 direction = (new Vector2(horizontalInput, verticalInput)).normalized;
        if (direction == Vector2.zero) return;
        dashVelocity = dashSpeed * direction;
        dashEndTime = Time.time + dashDuration;
        wallJumpEndTime = Time.time;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "YarnBallBox" && pickedUpYarnBall == null)
        {
            Vector3 offset = isFacingRight ? facingRightOffset : facingLeftOffset;
            pickedUpYarnBall = Instantiate(yarnBall, (gameObject.transform.position + offset), Quaternion.identity);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Platform>(out _))
        {
            AddPlatformCount(-1);
        }
    }

    public EntityType GetEntityType()
    {
        return EntityType.Player;
    }

    public void RecieveDamage(DamageInfo damageInfo, Vector2 attackerPos)
    {
        if (Time.time - lastDamagedTime > immortalDuration)
        {
            health -= damageInfo.damage;
            if (health < 0)
            {
                health = 0;
            }
            SoundManager.TryPlayNew("TestCatHurt");
            StatusUIManager.Instance.UpdateHealthBar(health, maxHealth);
            lastDamagedTime = Time.time;
        }
        if (damageInfo.isInterrupt)
        {
            isInterrupted = true;
            lastInterruptedTime = Time.time;
            interruptedDuration = damageInfo.interruptDuration;
        }
        if (damageInfo.isBounce)
        {
            Vector2 direction = ((Vector2)transform.position - attackerPos).normalized;
            if (isLiquid)
            {
                rb.AddForce(5f * damageInfo.bounceSpeed * direction, ForceMode2D.Impulse);
                return;
            }
            isBouncing = true;
            lastBounceTime = Time.time;
            bounceDuration = damageInfo.bounceDuration;
            bounceVelocity = damageInfo.bounceSpeed * direction;
        }
    }

    public void Heal(float amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        StatusUIManager.Instance.UpdateHealthBar(health, maxHealth);
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
