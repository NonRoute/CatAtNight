using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutDog : MonoBehaviour
{

    [SerializeField] protected float moveSpeed = 3f;
    [SerializeField] protected int moveDirection = 1;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] protected DamageInfo damageInfo = new();

    private Rigidbody2D rb;

    protected virtual void Start()
    {
        moveDirection = -1;
        rb = GetComponent<Rigidbody2D>();
        sprite = rb.GetComponentInChildren<SpriteRenderer>();
    }

    protected void FixedUpdate()
    {
        rb.velocity = new Vector2(moveSpeed * moveDirection, rb.velocity.y);
    }

    private void ChangeDirection()
    {
        moveDirection *= -1;
        sprite.flipX = !sprite.flipX;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamagable damagable))
        {
            if (!damageInfo.targetEntityType.HasFlag(damagable.GetEntityType())) return;
            ContactPoint2D contact = collision.GetContact(0);
            Vector2 attackerPos = contact.point + 3f * Vector2.down;
            damagable.RecieveDamage(damageInfo, attackerPos);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ForDog"))
        {
            ChangeDirection();
        }
    }
}
