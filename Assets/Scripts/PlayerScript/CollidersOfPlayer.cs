using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidersOfPlayer : MonoBehaviour, IDamagable
{

    [SerializeField] private Player player;
    public EntityType GetEntityType()
    {
        return EntityType.Player;
    }

    public void RecieveDamage(DamageInfo damageInfo, Vector2 attackerPos)
    {
        player.RecieveDamage(damageInfo, attackerPos);
    }

}
