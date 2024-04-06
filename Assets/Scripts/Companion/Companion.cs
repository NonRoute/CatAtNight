using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : MonoBehaviour
{
    [SerializeField] bool isEnabled = false;
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

    // may use in the future
    // but it is really hard to implement right now
    [SerializeField] Queue<Vector2> jumpPositions;

    // References
    [SerializeField] Transform spriteTransform;
    private float outDirection = 1f;
    private Vector2 startPosition;
    private float jumpEndTime;
    private float velocityX;
    private float velocityY;

    private Rigidbody2D rb;

    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private LayerMask blockLayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpPositions = new Queue<Vector2>();
    }

    private void Update()
    {

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
                if (distanceX < runThreshold + 4f)
                {
                    speed = Mathf.Lerp(walkSpeed, runSpeed, (distanceX - runThreshold) / 4f);
                }
            }
            walkDirection = direction * -perpendicular.normalized;
            Vector2 velocity = speed * walkDirection;
            transform.Translate(velocity * Time.deltaTime);
        }
        else
        {
            willJump = true;
        }

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
            transform.Translate(velocity * Time.deltaTime);
        }
        else
        {
            willJump = true;
        }

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
            spriteTransform.gameObject.SetActive(true);
            Vector2 startPos = posToJump + new Vector2(direction * 20f, 9f);
            //posToJump += direction * 0.5f * Vector2.right;
            Jump(startPos, posToJump);
        }
        else
        {
            // Jump out
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
    }

    public void SetStayStill(Vector2 destination)
    {
        if (!isEnabled) return;
        isStayStill = false;
        GoTo(destination);
        isStayStill = true;
    }
    public void SetFollow(Vector2 destination)
    {
        if (!isEnabled) return;
        isStayStill = false;
        GoTo(destination);
    }

    // For Debug Only
    [ContextMenu("Simulate Travel to Player")]
    public void GoToPlayerPos()
    {
        GoTo(GameplayStateManager.Instance.Player.GetCameraFollow().position - 0.55f * Vector3.up);
    }
}
