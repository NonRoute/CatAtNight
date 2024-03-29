using System;
using UnityEngine;

// For sending player status to other scripts
// Used in GetPlayerData()
public struct PlayerData
{
    public float health;
    public float maxHealth;
    public float staminaDrainRate;
    public float staminaRegenRate;
    public int skillUnlockedCount;
}

public partial class Player : MonoBehaviour, IDamagable
{
    // Change to FALSE when build
    private const bool IS_DEBUG = true;

    void Start()
    {
        InitVariables();
        InitInputs();
        // Load Save need to be use somewhere else, not here
        // Because we shouldn't load save each time player is loaded
        if (isLoadSave)
        {
            RestoreFromSave();
        }

        StatusUIManager.Instance.UpdateHealthBar(health, maxHealth);
    }

    private void InitVariables()
    {
        // Initialize References
        normal_rb = GetComponent<Rigidbody2D>();
        rb = normal_rb;
        sprite = GetComponentInChildren<SpriteRenderer>();

        // Initialize Status
        health = maxHealth;
        stamina = maxStamina;

        // Initialize Scale of Player (used in charging jump)
        initialScaleY = sprite.transform.localScale.y;

        // Initialize Bone Positions for Liquid Form
        bone_rigidbodies = liquid_rb.gameObject.GetComponentsInChildren<Rigidbody2D>();
        bone_localPositions = new Vector3[bone_rigidbodies.Length];
        bone_localRotations = new Quaternion[bone_rigidbodies.Length];
        initialSpritePos = sprite.transform.localPosition;
        for (int i = 0; i < bone_rigidbodies.Length; i++)
        {
            Transform t = bone_rigidbodies[i].transform;
            bone_localPositions[i] = t.localPosition;
            bone_localRotations[i] = t.localRotation;
        }
    }

    private void InitInputs()
    {
        // Enable PlayerInput
        // **May need to touch this later when scene changing happens
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();
    }

    void Update()
    {
        // Update Position That Camera will Follow
        UpdateCameraFollowPosition();

        // Update State of the Game
        // Currently there is only freezing state
        // Freeze state is only used in DialogueManager right now
        UpdateGameState();
        if (isFreeze)
        {
            UpdateAnimationState();
            animator.SetBool("is_running", false);
            animator.SetBool("is_walking", false);
            rb.velocity = new Vector2(0,rb.velocity.y);
            //rb.gravityScale = 0;
            return;
        }

        // Read Input for movement
        ReadInput();
        UpdateStamina();
        if (isLiquid)
        {
            UpdateLiquidMode();
            return;
        }
        UpdateSprite();
        UpdatePhysicsCondition();
        if (IS_DEBUG && noClip)
        {
            transform.Translate(
                3f * moveSpeed * Time.deltaTime * new Vector3(horizontalSmooth, verticalSmooth, 0f)
            );
            return;
        }
        if (isInterrupted)
        {
            UpdateWhenInterrupted();
            return;
        }
        // Update Player Movement
        UpdateMovement();
        UpdateJumping();
        // Update Yarn Ball Position and Throwing
        UpdateYarnBall();
    }

    private void ReadInput()
    {
        horizontalInput = playerInputActions.Player.Horizontal.ReadValue<float>();
        // Deadzone for Controller Input
        horizontalInput = (Math.Abs(horizontalInput) > 0.1f) ? Math.Sign(horizontalInput) : 0f;

        verticalInput = playerInputActions.Player.Vertical.ReadValue<float>();
        // Deadzone for Controller Input
        verticalInput = (Math.Abs(verticalInput) > 0.1f) ? Math.Sign(verticalInput) : 0f;

        // Lerping to make Smooth Input (4f is the magic number right now)
        horizontalSmooth = Mathf.MoveTowards(horizontalSmooth, horizontalInput, Time.deltaTime * 4f);
        verticalSmooth = Mathf.MoveTowards(verticalSmooth, verticalInput, Time.deltaTime * 4f);

        isTryingToRun = playerInputActions.Player.UseStamina.IsPressed() && horizontalInput != 0;

        // Switch Mode Right Here (Before We Update between Solid and Liquid Form)
        if (playerInputActions.Player.SwitchForm.WasPressedThisFrame())
        {
            SwitchMode(!isLiquid);
        }

        // Noclip for DEBUG only. No need to change anything here
        if (IS_DEBUG && Input.GetKeyDown(KeyCode.C))
        {
            noClip = !noClip;
            rb.bodyType = (noClip) ? RigidbodyType2D.Kinematic : RigidbodyType2D.Dynamic;
            Color newColor = sprite.color;
            newColor.a = (noClip) ? 0.5f : 1f;
            sprite.color = newColor;
            rb.velocity = Vector2.zero;
        }
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //    SaveGame();
        //}
    }

    private void UpdateGameState()
    {
        bool willFreeze = false;
        if (DialogueManager.Instance != null)
        {
            willFreeze |= DialogueManager.Instance.isDialogueActive;
        }
        isFreeze = willFreeze;

        if (playerInputActions.Player.OpenMenu.WasPressedThisFrame())
        {
            if (PauseUIManager.Instance != null)
            {
                PauseUIManager.Instance.TogglePauseMenu();
            }
        }
    }

    private void UpdateYarnBall()
    {
        if (pickedUpYarnBall != null)
        {
            Vector3 offset = isFacingRight ? facingRightOffset : facingLeftOffset;
            pickedUpYarnBall.transform.position = transform.position + offset;
            if (playerInputActions.Player.AtackAction.WasPressedThisFrame())
            {
                ThrowYarnBall();
            }
        }
    }

    private void CreateYarnBall()
    {
        Vector3 offset = isFacingRight ? facingRightOffset : facingLeftOffset;
        GameObject newYarnBall = Instantiate(
            yarnBall,
            (gameObject.transform.position + offset),
            Quaternion.identity
        );
        pickedUpYarnBall = newYarnBall.GetComponent<YarnBall>();
    }

    private void ThrowYarnBall()
    {
        pickedUpYarnBall.GetComponent<YarnBall>().Throw(isFacingRight);
        pickedUpYarnBall = null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Pick Up Yarn Ball
        if (isInterrupted) return;
        if ((collision.gameObject.CompareTag("YarnBallBox") || collision.gameObject.CompareTag("BossRoomYarnBallBox"))
            && pickedUpYarnBall == null)
        {
            CreateYarnBall();
            if (collision.gameObject.CompareTag("BossRoomYarnBallBox"))
            {
                zone1.GetComponent<Zone1>().ChangeYarnBallBoxPosition();
            }
        }
    }

}
