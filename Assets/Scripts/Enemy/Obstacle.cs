using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Obstacle : MonoBehaviour
{
    [SerializeField] private DamageInfo damageInfo = new();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamagable damagable))
        {
            if (!damageInfo.targetEntityType.HasFlag(damagable.GetEntityType())) return;
            //print(collision.contacts.Length);
            ContactPoint2D contact = collision.GetContact(0);
            damagable.RecieveDamage(damageInfo, contact.point);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamagable damagable))
        {
            if (!damageInfo.targetEntityType.HasFlag(damagable.GetEntityType())) return;

            damagable.RecieveDamage(damageInfo, collision.transform.position);
        }
    }
}
