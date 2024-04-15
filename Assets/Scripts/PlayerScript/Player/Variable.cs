using System.Collections.Generic;
using TMPro;
using UnityEngine;

public partial class Player : MonoBehaviour, IDamagable
{
    [Header("Status")]

    [SerializeField]
    private float health = 9f;

    [SerializeField]
    private float maxHealth = 9f;

    [SerializeField]
    private float stamina = 100f;

    [SerializeField]
    private float maxStamina = 100f;

    [SerializeField]
    private int skillProgression = 1;

    //[SerializeField]
    //private bool isLoadSave = true;

    [Header("Power Values")]

    [SerializeField]
    public float moveSpeed = 6f;

    [SerializeField]
    private float floatSpeed = 10f;

    [SerializeField]
    private float runMultiplier = 2f;

    [SerializeField]
    private float jumpPower = 8f;

    [SerializeField]
    private float chargeDuration = 0.75f;

    [SerializeField]
    private float dashDuration = 0.2f;

    [SerializeField]
    private float dashSpeed = 15f;

    [SerializeField]
    private int maxDashCount = 1;

    [SerializeField]
    private float baseStaminaRegenRate = 10f;

    [SerializeField]
    private float baseStaminaDrainRate = 2f;

    [SerializeField]
    private float baseStaminaUsage = 10f;

    [SerializeField]
    private float runningStaminaDrainMultiplier = 8f;

    [SerializeField]
    private float runJumpStaminaUsageMultiplier = 1f;

    [SerializeField]
    private float wallSlideSpeed = 3f;

    [SerializeField]
    private float wallClimbSpeed = 8f;

    [SerializeField]
    private float wallClimbStaminaDrainMultiplier = 3f;

    [SerializeField]
    private float wallJumpSpeed = 25f;

    [SerializeField]
    private float wallJumpDuration = 0.3f;

    [SerializeField]
    private float wallJumpStaminaUsageMultiplier = 1f;

    [Header("Static Values")]

    [SerializeField]
    public float maxVerticalVelocity = 15f;

    // Max Vertical Velocity when Floating
    // It limits jumping so It is currently disabled
    [SerializeField]
    public float maxFloatingVelocity = 150f;

    [SerializeField]
    private float gravityScale = 3f;

    [SerializeField]
    private float risingGravityMultiplier = 1f;

    [SerializeField]
    private float fallingGravityMultiplier = 1.5f;

    [SerializeField]
    private float groundedDelay = 0.1f;

    [SerializeField]
    private float minimumStamina = 30f;

    [SerializeField]
    private float staminaRegenDelay = 1f;

    // Duration that player will be immortal after receiving damage
    [SerializeField]
    private float immortalDuration = 1f;

    [Header("Collider Values")]

    [SerializeField]
    private LayerMask platformLayer;

    [SerializeField]
    private LayerMask wallLayer;

    [Header("Liquid Mode Values")]

    [SerializeField]
    private bool isLiquid = false;

    [SerializeField]
    private float liquidMoveForce = 25f;

    [SerializeField]
    private float liquidFloatingMultiplier = 0.05f;

    [SerializeField]
    private float liquidJumpForce = 75f;

    [SerializeField]
    private float liquidDashDuration = 0.2f;

    [SerializeField]
    private float liquidDashSpeed = 15f;

    [SerializeField]
    public float liquidMaxVerticalVelocity = 30f;

    [SerializeField]
    public float liquidMaxHorizontalVelocity = 30f;

    [Header("Companion Skill Values")]

    [SerializeField]
    private float startDelayDistance = 8f;

    [SerializeField]
    private float companionTriggerDistance = 5f;

    [SerializeField]
    private float companionTriggerTime = 2f;

    [SerializeField]
    private float delayDistance = 3f;

    [SerializeField]
    private float delayPositionTime = 1f;

    [Header("Zone 1")]

    [SerializeField]
    private GameObject yarnBall;

    [SerializeField]
    private Vector3 facingRightOffset;

    [SerializeField]
    private Vector3 facingLeftOffset;

    private YarnBall pickedUpYarnBall;

    private Vector2 yarnBallVel;

    [SerializeField]
    private GameObject zone1;

    [SerializeField]
    private float yarnBallSmoothTime;

    [Header("References")]

    [SerializeField]
    private GameObject textBox;

    [SerializeField]
    private TMP_Text textBoxText;

    [SerializeField]
    private GameObject companionPrefab;

    private Companion companion;

    [SerializeField]
    private Transform playerFeetTransform;

    [SerializeField]
    private Transform liquidPlayerBottomTransform;

    [SerializeField]
    private Transform wallCheckPivot;

    [SerializeField]
    private Transform wallCheckTransform;

    [SerializeField]
    private Transform cameraFollowTransform;

    [SerializeField]
    private GameObject normalColliders;

    private Rigidbody2D rb;

    private Rigidbody2D normal_rb;

    [SerializeField]
    private Rigidbody2D liquid_rb;

    [SerializeField]
    private GameObject liquidForm;

    [SerializeField]
    private GameObject staticLiquidSprite;

    [SerializeField]
    private GameObject spriteObject;

    private SpriteRenderer sprite;

    private Animator animator;

    // Initial liquid mode values
    private Rigidbody2D[] boneRigidbodies;

    private Vector3[] boneLocalPositions;

    private Quaternion[] bone_localRotations;

    // Use to Set Scale y when Charging Jump
    private float initialScaleY;

    // Use to restore sprite position after rotated
    private Vector3 initialSpritePos;

    private PlayerInputActions playerInputActions;

    [Header("Debug Values")]

    [Header("--Input")]

    [SerializeField]
    private float horizontalInput = 0f;

    [SerializeField]
    private float verticalInput = 0f;

    [SerializeField]
    private float horizontalSmooth = 0f;

    [SerializeField]
    private float verticalSmooth = 0f;

    [Header("--States")]

    [SerializeField]
    private bool isFacingRight = false;

    [SerializeField]
    private bool isFreeze = false;

    // Damage-related variables
    [Header("--Damage-related")]

    [SerializeField]
    private bool isInterrupted = false;

    [SerializeField]
    private float lastInterruptedTime = 0f;

    [SerializeField]
    private float interruptedDuration = 0f;

    [SerializeField]
    private float lastDamagedTime = 0f;

    [SerializeField]
    private bool isBouncing = false;

    [SerializeField]
    private float lastBounceTime = 0f;

    [SerializeField]
    private float bounceDuration = 0f;

    [SerializeField]
    private Vector2 bounceVelocity;

    // Jumping-related variables
    [Header("--Jump-related")]

    [SerializeField]
    List<Platform> passThroughPlatformList;

    [SerializeField]
    private bool isGrounded = true;

    [SerializeField]
    private float lastGroundedTime = 0f;

    [SerializeField]
    private bool isFloating = false;

    [SerializeField]
    private bool isChargeJumping = false;

    [SerializeField]
    private float chargePercent = 0f;

    [SerializeField]
    private int dashCount = 0;

    [SerializeField]
    private float dashEndTime = 0f;

    [SerializeField]
    private Vector2 dashVelocity = Vector2.zero;

    [SerializeField]
    private Vector2 wallJumpVelocity = Vector2.zero;

    [SerializeField]
    private float wallJumpEndTime = 0f;

    [SerializeField]
    private bool isWalled = false;

    [SerializeField]
    private bool isWallSliding = false;

    // Stamina-related variables
    [Header("--Stamina-related")]

    [SerializeField]
    private bool isTryingToRun = false;

    [SerializeField]
    private bool isRunning = false;

    [SerializeField]
    private float lastRunningTime = 0f;

    [SerializeField]
    private bool isClimbingWall = false;

    [SerializeField]
    private bool isStaminaOut = false;

    // Liquid Mode-related variables
    [Header("--Liquid Mode-related")]

    [SerializeField]
    private Vector2 liquidDashVelocity = Vector2.zero;

    // Need to seperate this because dashing should stop when player transform from liquid to solid form
    [SerializeField]
    private float liquidDashEndTime = 0f;

    [SerializeField]
    private float timeBeforeEnter;

    [SerializeField]
    private Vector3 positionBeforeEnter;

    [SerializeField]
    private bool isInPipe = false;

    [SerializeField]
    private bool isPipeForward = false;

    [SerializeField]
    private int pipeIndex = 0;

    [SerializeField]
    private float travelStartTime = 0f;

    [SerializeField]
    private float travelEndTime = 0f;

    [SerializeField]
    private Pipe currentPipe;

    // Companion-related variables
    [Header("--Companion-related")]

    [SerializeField]
    private Vector2 lastGroundPosition;

    [SerializeField]
    private Vector2 lastLandingPosition;

    [SerializeField]
    private Vector2 startDelayedPosition = Vector2.zero;

    [SerializeField]
    private Vector2 delayedPosition = Vector2.zero;

    [SerializeField]
    private float lastSetDelayedPositionTime = 0f;

    [SerializeField]
    private float lastSetCompanionPositionTime = 0f;

    [SerializeField]
    private float textBoxEndTime;

    [SerializeField]
    private bool isTalking;

    [SerializeField]
    private string choice3TalkText;

    [Header("--Others")]

    [SerializeField]
    private bool noClip = false;

}
