using System;
using UnityEngine;

public partial class Player : MonoBehaviour, IDamagable
{
    private const bool IS_DEBUG = true;

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
        initialScaleY = sprite.transform.localScale.y;
        for (int i = 0; i < bone_rigidbodies.Length; i++)
        {
            Transform t = bone_rigidbodies[i].transform;
            bone_localPositions[i] = t.localPosition;
            bone_localRotations[i] = t.localRotation;
        }

        if (isLoadSave)
        {
            RestoreFromSave();
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
        UpdateAnimation();
        UpdateStamina();
        UpdateCollideCondition();
        if (IS_DEBUG && noClip)
        {
            transform.Translate(
                3f * moveSpeed * Time.deltaTime * new Vector3(horizontalSmooth, verticalSmooth, 0f)
            );
            return;
        }
        if (isInterrupted)
        {
            UpdateInterrupted();
            return;
        }
        UpdateMovement();
        UpdateJumping();
        IncreaseChargeJumpPercent();
        UpdateYarnBall();
    }

    private void ReadInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        horizontalSmooth = Input.GetAxis("Horizontal");
        verticalSmooth = Input.GetAxis("Vertical");
        isTryingToRun = Input.GetKey(KeyCode.LeftShift) && horizontalInput != 0;
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

        if (Input.GetKeyDown(KeyCode.S))
        {
            SaveGame();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseUIManager.Instance != null)
            {
                PauseUIManager.Instance.TogglePauseMenu();
            }
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (
            (
                collision.gameObject.CompareTag("YarnBallBox")
                || collision.gameObject.CompareTag("BossRoomYarnBallBox")
            )
            && pickedUpYarnBall == null
        )
        {
            Vector3 offset = isFacingRight ? facingRightOffset : facingLeftOffset;
            pickedUpYarnBall = Instantiate(
                yarnBall,
                (gameObject.transform.position + offset),
                Quaternion.identity
            );
            if (collision.gameObject.CompareTag("BossRoomYarnBallBox"))
            {
                zone1.GetComponent<Zone1>().ChangeYarnBallBoxPosition();
            }
        }
    }

    private void UpdateAnimation()
    {
        animator.SetBool("is_grounded", isOnPlatform);
        animator.SetBool("is_charging", isChargeJumping);
        animator.SetBool("is_running", isRunning);
        animator.SetBool("is_walking", !isRunning && Math.Abs(rb.velocity.x) > 1);
        animator.SetFloat("speed_y", rb.velocity.y);
    }
}
