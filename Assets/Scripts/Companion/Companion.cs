using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum CompanionAction
{
    AttackBossZone3,
    HoldItem,
    Boost,
}

public class Companion : MonoBehaviour
{
    [SerializeField] bool isEnabled = false;
    public bool IsEnabled => isEnabled;
    [SerializeField] bool isWalkingOut = false;
    [SerializeField] bool isStayStill = false;
    // Walk
    [SerializeField] Vector2 destination;
    [SerializeField] bool isGoingToNextPos = false;
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float runSpeed = 9f;
    [SerializeField] float runThreshold = 6f;
    // Jump
    [SerializeField] bool isJumping = false;
    [SerializeField] float timeToJump = 1f;
    [SerializeField] float gravity = -10f;

    // Talk
    [SerializeField] private float textBoxEndTime;
    [SerializeField] private bool isTalking;

    // Choice
    public bool hasChoice3 = false;
    public string responseText;
    public CompanionAction companionAction;
    [SerializeField] GameObject platform;

    // may use in the future
    // but it is really hard to implement right now
    [SerializeField] Queue<Vector2> jumpPositions;

    // References
    [SerializeField] private GameObject textBox;
    [SerializeField] private TMP_Text textBoxText;
    [SerializeField] Transform spriteTransform;
    private SpriteRenderer sprite;
    private Animator anim;
    private float outDirection = 1f;
    private Vector2 startPosition;
    private float jumpEndTime;
    private float velocityX;
    private float velocityY;
    [SerializeField] float currentSpeed = 5f;

    private Rigidbody2D rb;

    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private LayerMask blockLayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpPositions = new Queue<Vector2>();
        sprite = spriteTransform.gameObject.GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateTextBox();
        UpdateSprite();

        if (isJumping)
        {
            UpdateJumping();
            return;
        }

        if (isWalkingOut)
        {
            UpdateWalkOut();
        }

        if (!isEnabled) return;

        if (isGoingToNextPos)
        {
            UpdateWalking();
        }
    }

    private void UpdateSprite()
    {
        anim.SetBool("isWalking", isGoingToNextPos || isWalkingOut);
        anim.SetBool("isJumping", isJumping);
        anim.SetFloat("walkSpeed", currentSpeed / walkSpeed);
    }

    private void FlipSprite(bool isFacingRight)
    {
        sprite.flipX = isFacingRight;
    }
    private void UpdateWalking()
    {
        bool willJump = false;

        Vector2 currentPos = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(origin: currentPos + 0.2f * Vector2.up, direction: Vector2.down
        , distance: 1f, layerMask: platformLayer);

        Vector2 walkDirection = Vector2.right;

        if (hit.collider != null)
        {
            Vector2 perpendicular = Vector2.Perpendicular(hit.normal);
            RotateSprite(hit.normal);
            float direction = Mathf.Sign(destination.x - currentPos.x);
            float speed = walkSpeed;
            float distanceX = Mathf.Abs(destination.x - currentPos.x);
            if (distanceX > runThreshold)
            {
                speed = runSpeed;
                // Smoothing
                if (GameplayStateManager.Instance == null || GameplayStateManager.Instance.Player == null) return;
                float realDistanceX = Mathf.Abs(GameplayStateManager.Instance.Player.GetCameraFollow().position.x - currentPos.x);
                if (realDistanceX < runThreshold + 6f)
                {
                    speed = Mathf.Lerp(walkSpeed, runSpeed, (realDistanceX - runThreshold) / 6f);
                }
                currentSpeed = speed;
            }
            walkDirection = direction * -perpendicular.normalized;
            Vector2 velocity = speed * walkDirection;
            transform.Translate(velocity * Time.deltaTime);
        }
        else
        {
            willJump = true;
        }

        FlipSprite(walkDirection.x > 0);

        RaycastHit2D hit_side = Physics2D.Raycast(origin: currentPos + 0.2f * Vector2.up, direction: walkDirection
        , distance: 0.3f, layerMask: blockLayer);
        if (hit_side.collider != null)
        {
            willJump = true;
        }

        willJump |= 1.5f * Mathf.Abs(currentPos.x - destination.x) < Mathf.Abs(currentPos.y - destination.y);

        if (willJump)
        {
            isGoingToNextPos = false;
            Jump(currentPos, destination);
            return;
        }

        if (Vector2.Distance(transform.position, destination) < 0.1f)
        {
            transform.position = destination;
            isGoingToNextPos = false;
        }
    }

    private void UpdateJumping()
    {
        Vector2 displacement = new Vector2(velocityX, velocityY) * Time.deltaTime;
        transform.Translate(displacement);
        if (Time.time > jumpEndTime)
        {
            transform.position = destination;
            RaycastHit2D hit = Physics2D.Raycast(origin: transform.position + 0.2f * Vector3.up, direction: Vector2.down
                , distance: 1f, layerMask: platformLayer);
            if (hit.collider != null)
            {
                RotateSprite(hit.normal);
            }
            isJumping = false;
        }
    }

    private void UpdateWalkOut()
    {
        bool willJump = false;

        Vector2 currentPos = transform.position;

        RaycastHit2D hit = Physics2D.Raycast(origin: currentPos + 0.2f * Vector2.up, direction: Vector2.down
        , distance: 1f, layerMask: platformLayer);

        Vector2 walkDirection = Vector2.right;

        if (hit.collider != null)
        {
            Vector2 perpendicular = Vector2.Perpendicular(hit.normal);
            RotateSprite(hit.normal);
            walkDirection = outDirection * -perpendicular.normalized;
            Vector2 velocity = runSpeed * walkDirection;
            currentSpeed = runSpeed;
            transform.Translate(velocity * Time.deltaTime);
        }
        else
        {
            willJump = true;
        }

        FlipSprite(walkDirection.x > 0);

        RaycastHit2D hit_side = Physics2D.Raycast(origin: currentPos + 0.2f * Vector2.up, direction: walkDirection
        , distance: 0.3f, layerMask: blockLayer);
        if (hit_side.collider != null)
        {
            willJump = true;
        }

        if (willJump)
        {
            Jump(currentPos, currentPos + new Vector2(outDirection * 20f, 9f));
            StartCoroutine(Hide());
            isWalkingOut = false;
            return;
        }

        Vector2 playerPos = GameplayStateManager.Instance.Player.GetCameraFollow().position;

        if (Mathf.Abs(playerPos.x - currentPos.x) > 20f)
        {
            spriteTransform.gameObject.SetActive(false);
            isWalkingOut = false;
        }
    }

    private void FixedUpdate()
    {
        if (!isEnabled) return;

        if (isJumping)
        {
            velocityY += gravity * Time.fixedDeltaTime;
        }
    }

    private void RotateSprite(Vector2 normal)
    {
        float rightAngle = Vector2.Angle(normal, Vector2.right);
        float leftAngle = Vector2.Angle(normal, Vector2.left);
        float rotateAngle = Vector2.Angle(normal, Vector2.up);
        RotateSprite(((rightAngle < leftAngle) ? -1 : 1) * rotateAngle);
        spriteTransform.rotation = Quaternion.Euler(0f, 0f, ((rightAngle < leftAngle) ? -1 : 1) * rotateAngle);
    }

    private void RotateSprite(float angle)
    {
        spriteTransform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    public void ToggleCompanion(Vector2 posToJump)
    {
        isEnabled = !isEnabled;
        Vector2 playerPos = GameplayStateManager.Instance.Player.GetCameraFollow().position;
        float direction = Mathf.Sign(posToJump.x - playerPos.x);
        isGoingToNextPos = false;
        if (isEnabled)
        {
            // Jump in
            isJumping = false;
            isStayStill = false;
            CompanionUIManager.Instance.SetStatus(1);
            spriteTransform.gameObject.SetActive(true);
            StartCoroutine(StartTextBox("Hi", 2f));
            if (isWalkingOut)
            {
                isWalkingOut = false;
            }
            else
            {
                Vector2 startPos = posToJump + new Vector2(direction * 20f, 9f);
                //posToJump += direction * 0.5f * Vector2.right;
                Jump(startPos, posToJump);
            }
        }
        else
        {
            // Jump out
            StartCoroutine(StartTextBox("Bye bye", 2f));
            isWalkingOut = true;
            outDirection = direction;
            //StartCoroutine(Hide());
        }
    }

    IEnumerator Hide()
    {
        yield return new WaitForSeconds(timeToJump);
        if (!isEnabled)
        {
            spriteTransform.gameObject.SetActive(false);
        }
    }

    public void GoTo(Vector2 destination)
    {
        if (!isEnabled || isJumping || isStayStill) return;

        RaycastHit2D hit = Physics2D.Raycast(origin: destination + 0.1f * Vector2.up, direction: Vector2.down
          , distance: 0.5f, layerMask: platformLayer);
        if (hit.collider != null)
        {
            destination.y = hit.point.y;
        }

        WalkTo(destination);
    }

    private void WalkTo(Vector2 destination)
    {
        startPosition = transform.position;
        this.destination = destination;

        // Set feet to ground
        RaycastHit2D hit = Physics2D.Raycast(origin: startPosition + 0.2f * Vector2.up, direction: Vector2.down
          , distance: 0.5f, layerMask: platformLayer);
        if (hit.collider != null)
        {
            startPosition.y = hit.point.y;
        }
        transform.position = startPosition;

        isGoingToNextPos = true;
    }

    private void Jump(Vector2 startPosition, Vector2 destination)
    {
        transform.position = startPosition;
        RotateSprite(0f);
        this.startPosition = startPosition;
        this.destination = destination;

        float deltaX = destination.x - startPosition.x;
        velocityX = deltaX / timeToJump;
        float deltaY = destination.y - startPosition.y;
        velocityY = deltaY / timeToJump - 0.5f * gravity * timeToJump;

        jumpEndTime = Time.time + timeToJump;
        isJumping = true;
        FlipSprite(deltaX > 0);
    }

    public void SetStayStill(Vector2 destination)
    {
        if (!isEnabled) return;
        ClearSomeAction();
        StartCoroutine(StartTextBox("Roger that", 2f));
        isStayStill = false;
        GoTo(destination);
        isStayStill = true;
    }

    private string GetFollowAcknowledgeMessage()
    {
        if (Random.Range(0f, 1f) < 0.05f)
        {
            return "Yes, chef!";
        }
        else
        {
            return "Yes, sir";
        }
    }

    public void SetFollow(Vector2 destination)
    {
        if (!isEnabled) return;
        ClearSomeAction();
        StartCoroutine(StartTextBox(GetFollowAcknowledgeMessage(), 2f));
        isStayStill = false;
        GoTo(destination);
    }

    public void ClearSomeAction()
    {
        platform.SetActive(false);
    }

    public void StartChoice3()
    {
        if (!isEnabled || !hasChoice3) return;
        StartCoroutine(StartTextBox(responseText, 2f));
        ClearSomeAction();
        if (companionAction == CompanionAction.AttackBossZone3)
        {
            Zone3Manager.Instance.AttackBoss();
            CompanionUIManager.Instance.SetStatus(1);
            SetFollow(GameplayStateManager.Instance.Player.GetLastGroundPosition());
        }
        else if(companionAction == CompanionAction.Boost)
        {
            SetStayStill(GameplayStateManager.Instance.Player.GetLastGroundPosition());
            platform.SetActive(true);
        }
    }

    // For Debug Only
    [ContextMenu("Simulate Travel to Player")]
    public void GoToPlayerPos()
    {
        GoTo(GameplayStateManager.Instance.Player.GetLastGroundPosition());
    }

    IEnumerator StartTextBox(string text, float duration)
    {
        yield return new WaitForSeconds(0.2f);
        isTalking = true;
        textBox.SetActive(true);
        textBoxText.text = text;
        textBoxEndTime = Time.time + duration;
    }

    public void StopTextBox()
    {
        isTalking = false;
        textBox.SetActive(false);
    }

    private void UpdateTextBox()
    {
        if (!isTalking) return;

        if (Time.time >= textBoxEndTime)
        {
            StopTextBox();
        }
    }
}
