using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Monster
{
    [SerializeField] private GameObject door;
    public override EntityType GetEntityType()
    {
        return EntityType.Boss;
    }

    public override void RecieveDamage(DamageInfo damageInfo, Vector2 attackerPos)
    {
        health -= damageInfo.damage;
        if (health <= 0)
        {
            Destroy(gameObject);
            Destroy(door);
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
}
