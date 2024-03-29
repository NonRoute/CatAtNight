using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Monster
{
    [SerializeField] private GameObject door;
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private Transform playerPos;

    public void OnDead()
    {
        cam.Follow = playerPos;
        Destroy(gameObject);
        Destroy(door);
    }

    public override EntityType GetEntityType()
    {
        return EntityType.Boss;
    }

    public override void RecieveDamage(DamageInfo damageInfo, Vector2 attackerPos)
    {
        health -= damageInfo.damage;
        if (health <= 0)
        {
            OnDead();
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
