using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Companion : MonoBehaviour
{
    [SerializeField] bool isEnabled = false;
    [SerializeField] bool isJumping = false;
    [SerializeField] bool isGoingToNextPos = false;
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float runSpeed = 8f;
    [SerializeField] float timeToJump = 1f;
    [SerializeField] float gravity = -10f;
    [SerializeField] Vector2 destination;
    [SerializeField] float verticalThreshold = 1f;
    [SerializeField] Queue<Vector2> jumpPositions;
    [SerializeField] Transform spriteTransform;
    private Vector2 startPosition;
    private float direction = 1;
    private float walkEndTime = 0f;
    private float jumpEndTime;
    private float velocityX;
    private float velocityY;

    private Rigidbody2D rb;

    [SerializeField]
    private LayerMask platformLayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        jumpPositions = new Queue<Vector2>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            GoToPlayerPos();
        }

        if (!isEnabled) return;

        if (isGoingToNextPos)
        {

            if (Mathf.Abs(transform.position.x - destination.x) < Mathf.Abs(transform.position.y - destination.y))
            {
                isGoingToNextPos = false;
                Jump(transform.position, destination);
                return;
            }

            RaycastHit2D hit = Physics2D.Raycast(origin: transform.position + 0.2f * Vector3.up, direction: Vector2.down
            , distance: 1f, layerMask: platformLayer);

            if (hit.collider != null)
            {
                Vector2 perpendicular = Vector2.Perpendicular(hit.normal);
                RotateSprite(hit.normal);
                float direction = Mathf.Sign(destination.x - transform.position.x);
                Vector2 velocity = direction * walkSpeed * -perpendicular.normalized;
                transform.Translate(velocity * Time.deltaTime);
            }
            else
            {
                isGoingToNextPos = false;
                Jump(transform.position, destination);
            }

            if(Vector2.Distance(transform.position, destination) < 0.1f)
            {
                transform.position = destination;
                isGoingToNextPos = false;
            }
        }

        if (isJumping)
        {
            Vector2 displacement = new Vector2(velocityX, velocityY) * Time.deltaTime;
            transform.Translate(displacement);
            if (Time.time > jumpEndTime)
            {
                transform.position = destination;
                isJumping = false;
            }
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
        spriteTransform.rotation = Quaternion.Euler(0f, 0f, ((rightAngle < leftAngle) ? -1 : 1) * rotateAngle);
    }

    public void ToggleCompanion(Vector2 posToJump)
    {
        isEnabled = !isEnabled;
        if (isEnabled)
        {
            Vector2 playPos = GameplayStateManager.Instance.Player.GetCameraFollow().position;
            float direction = Mathf.Sign(posToJump.x - playPos.x);
            transform.position = posToJump + new Vector2(direction * 20f, 9f);
            isGoingToNextPos = false;
            Jump(transform.position, posToJump);
        }
    }

    [ContextMenu("Simulate Travel")]
    public void GoToNextPos()
    {
        GoToNextPos(destination);
    }

    [ContextMenu("Simulate Travel to Player")]
    public void GoToPlayerPos()
    {
        GoToNextPos(GameplayStateManager.Instance.Player.GetCameraFollow().position - 0.55f * Vector3.up);
    }

    public void GoToNextPos(Vector2 destination)
    {
        if (!isEnabled) return;
        if (isJumping) return;

        RaycastHit2D hit = Physics2D.Raycast(origin: destination, direction: Vector2.down
          , distance: 0.5f, layerMask: platformLayer);
        if (hit.collider != null)
        {
            destination.y = hit.point.y;
        }

        WalkToNextPos(destination);
    }

    private void WalkToNextPos(Vector2 destination)
    {
        startPosition = transform.position;
        this.destination = destination;

        // Set feet to ground
        RaycastHit2D hit = Physics2D.Raycast(origin: startPosition, direction: Vector2.down
          , distance: 3f, layerMask: platformLayer);
        if (hit.collider != null)
        {
            startPosition.y = hit.point.y;
        }
        transform.position = startPosition;
        direction = Mathf.Sign(destination.x - startPosition.x);

        walkEndTime = Time.time + Mathf.Abs(destination.x - startPosition.x) / walkSpeed;

        isGoingToNextPos = true;
    }


    private void Jump(Vector2 startPosition, Vector2 destination)
    {
        transform.position = startPosition;
        this.startPosition = startPosition;
        this.destination = destination;

        float deltaX = destination.x - startPosition.x;
        velocityX = deltaX / timeToJump;
        float deltaY = destination.y - startPosition.y;
        velocityY = deltaY / timeToJump - 0.5f * gravity * timeToJump;

        jumpEndTime = Time.time + timeToJump;
        isJumping = true;
    }
}
