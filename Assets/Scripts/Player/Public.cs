using UnityEngine;

public partial class Player : MonoBehaviour, IDamagable
{
  public EntityType GetEntityType()
  {
    return EntityType.Player;
  }

  public void RecieveDamage(DamageInfo damageInfo, Vector2 attackerPos)
  {
    if (isFreeze) return;
    if (Time.time - lastDamagedTime > immortalDuration)
    {
      health -= damageInfo.damage;
      if (health < 0)
      {
        health = 0;
      }
      SoundManager.TryPlayNew("TestCatHurt");
      StatusUIManager.Instance.UpdateHealthBar(health, maxHealth);
      lastDamagedTime = Time.time;
    }
    if (damageInfo.isInterrupt)
    {
      isInterrupted = true;
      lastInterruptedTime = Time.time;
      interruptedDuration = damageInfo.interruptDuration;
    }
    if (damageInfo.isBounce)
    {
      Vector2 direction = ((Vector2)transform.position - attackerPos).normalized;
      if (isLiquid)
      {
        rb.AddForce(2f * damageInfo.bounceSpeed * direction, ForceMode2D.Impulse);
        return;
      }
      isBouncing = true;
      lastBounceTime = Time.time;
      bounceDuration = damageInfo.bounceDuration;
      bounceVelocity = damageInfo.bounceSpeed * direction;
    }
  }

  public void Heal(float amount)
  {
    health = Mathf.Min(health + amount, maxHealth);
    StatusUIManager.Instance.UpdateHealthBar(health, maxHealth);
  }
}
