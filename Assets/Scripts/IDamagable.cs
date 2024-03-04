using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum EntityType
{
    Player = 1 << 0,
    SmallEnemy = 1 << 1,
    BigEnemy = 1 << 2,
    Boss = 1 << 3
}

[System.Serializable]
public struct DamageInfo
{
    public EntityType targetEntityType;
    public float damage;
    public bool isInterrupt;
    public float interruptDuration;
    public bool isBounce;
    public float bounceDuration;
    public float bounceSpeed;
}

public interface IDamagable
{
    public EntityType GetEntityType();
    public void RecieveDamage(DamageInfo damageInfo, Vector2 attackerPos);
}
