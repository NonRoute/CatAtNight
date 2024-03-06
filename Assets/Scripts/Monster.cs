using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour, IDamagable
{
    private const int MOVE_LEFT = -1;
    private const int MOVE_RIGHT = 1;

    [SerializeField] private float health = 2f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private float jumpForce = 1.5f;
    [SerializeField] private float jumpCooldown = 5f;

    private int moveDirection = MOVE_RIGHT;
    private float moveTimeElapsed;
    private float jumpTimeElapsed = 0f;
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
        if (health < 0)
        {
            Destroy(gameObject);
        }
    }
}
