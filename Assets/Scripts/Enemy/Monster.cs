using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]
public class Monster : MonoBehaviour, IDamagable
{
    protected const int MOVE_LEFT = -1;
    protected const int MOVE_RIGHT = 1;

    [SerializeField] protected int monsterScore = 1;
    [SerializeField] protected float health = 2f;
    [SerializeField] protected float moveSpeed = 1f;
    [SerializeField] protected float moveDuration = 1f;
    [SerializeField] protected float jumpForce = 1.5f;
    [SerializeField] protected float jumpCooldown = 5f;
    [SerializeField] protected DamageInfo damageInfo = new();
    [SerializeField] private ScoreManager scoreManager;

    [Header("Debug")]
    [SerializeField] protected int moveDirection = MOVE_RIGHT;
    [SerializeField] protected float moveTimeElapsed;
    [SerializeField] protected float jumpTimeElapsed = 0f;
    [SerializeField] protected bool isInterrupted = false;
    [SerializeField] protected float lastInterruptedTime = 0f;
    [SerializeField] protected float interruptedDuration = 0f;
    [SerializeField] protected bool isBouncing = false;
    [SerializeField] protected float lastBounceTime = 0f;
    [SerializeField] protected float bounceDuration = 0f;
    [SerializeField] protected Vector2 bounceVelocity;


    private Rigidbody2D rb;

    protected virtual void Start()
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
    public virtual EntityType GetEntityType()
    {
        return EntityType.SmallEnemy;
    }

    public virtual void RecieveDamage(DamageInfo damageInfo, Vector2 attackerPos)
    {
        health -= damageInfo.damage;
        if (health <= 0)
        {
            scoreManager.monsterScore += monsterScore;
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

        SoundManager.TryPlay("RatHit");
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
        }
    }
}
