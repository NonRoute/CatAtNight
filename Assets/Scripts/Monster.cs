using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Monster : MonoBehaviour, IDamagable
{
    private const int MOVE_LEFT = -1;
    private const int MOVE_RIGHT = 1;

    [SerializeField] private float health = 2f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private float jumpForce = 1.5f;
    [SerializeField] private float jumpCooldown = 5f;
    [SerializeField] private DamageInfo damageInfo = new();

    [Header("Debug")]
    [SerializeField] private int moveDirection = MOVE_RIGHT;
    [SerializeField] private float moveTimeElapsed;
    [SerializeField] private float jumpTimeElapsed = 0f;
    [SerializeField] private bool isInterrupted = false;
    [SerializeField] private float lastInterruptedTime = 0f;
    [SerializeField] private float interruptedDuration = 0f;
    [SerializeField] private bool isBouncing = false;
    [SerializeField] private float lastBounceTime = 0f;
    [SerializeField] private float bounceDuration = 0f;
    [SerializeField] private Vector2 bounceVelocity;


    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        moveTimeElapsed = moveSpeed / 2;
    }

    private void FixedUpdate()
    {
        moveTimeElapsed += Time.deltaTime;
        jumpTimeElapsed += Time.deltaTime;

        if (moveTimeElapsed >= moveDuration)
        {
            ChangeDirection();
            moveTimeElapsed = 0f;
        }

        if (isInterrupted)
        {
            UpdateInterrupted();
            return;
        }
        Move();
        Jump();
    }

    private void ChangeDirection()
    {
        moveDirection *= -1;
    }

    private void Move()
    {
        rb.velocity = new Vector2(moveSpeed * moveDirection, rb.velocity.y);
    }

    private void Jump()
    {
        if (jumpTimeElapsed >= jumpCooldown)
        {
            jumpTimeElapsed = 0f;
            rb.AddForce(jumpForce * Vector2.up, ForceMode2D.Impulse);
        }
    }
    public EntityType GetEntityType()
    {
        return EntityType.SmallEnemy;
    }

    public void RecieveDamage(DamageInfo damageInfo, Vector2 attackerPos)
    {
        health -= damageInfo.damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        if (damageInfo.isInterrupt)
        {
            isInterrupted = true;
            lastInterruptedTime = Time.time;
            interruptedDuration = damageInfo.interruptDuration;
        }
        if (damageInfo.isBounce)
        {
            isBouncing = true;
            lastBounceTime = Time.time;
            bounceDuration = damageInfo.bounceDuration;
            bounceVelocity = damageInfo.bounceSpeed * ((Vector2)transform.position - attackerPos).normalized;
        }
    }

    private void UpdateInterrupted()
    {
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamagable damagable))
        {
            if (!damageInfo.targetEntityType.HasFlag(damagable.GetEntityType())) return;
            ContactPoint2D contact = collision.GetContact(0);
            damagable.RecieveDamage(damageInfo, contact.point);
            Destroy(gameObject);
        }
    }
}
