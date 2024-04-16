using UnityEngine;

public partial class Player : MonoBehaviour, IDamagable
{
    public void SetUnlockedSkill(int skillProgression)
    {
        this.skillProgression = skillProgression;
        GameEventsManager.instance.playerEvents.PlayerSkillProgressionChange(skillProgression);
    }

    public bool IsInUI()
    {
        return isFreeze;
    }

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
                Dead();
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
            if (pickedUpYarnBall != null)
            {
                ThrowYarnBall();
            }
        }
        if (damageInfo.isBounce)
        {
            Vector2 direction = ((Vector2)rb.position - attackerPos).normalized;
            if (isLiquid)
            {
                rb.AddForce(4f * damageInfo.bounceSpeed * direction, ForceMode2D.Impulse);
                return;
            }
            isBouncing = true;
            lastBounceTime = Time.time;
            bounceDuration = damageInfo.bounceDuration;
            bounceVelocity = damageInfo.bounceSpeed * direction;
        }
    }

    public void Dead()
    {
        GameOverUIManager.Instance.OpenGameOverUI();
        isDead = true;
    }

    public void Heal(float amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        StatusUIManager.Instance.UpdateHealthBar(health, maxHealth);
    }

    public void UpgradeHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
        health = maxHealth;
        StatusUIManager.Instance.UpdateHealthBar(health, maxHealth);
    }

    public Transform GetCameraFollow()
    {
        return cameraFollowTransform;
    }

    // Get player data to other scripts want to use
    public PlayerData GetPlayerData()
    {
        return new PlayerData()
        {
            health = health,
            maxHealth = maxHealth,
            staminaDrainRate = baseStaminaDrainRate,
            staminaRegenRate = baseStaminaRegenRate,
            skillProgression = skillProgression,
        };
    }

    public bool CheckPlayerMomentum(float sqrMagnitude)
    {
        // sqrMagnitude should be 400f
        return isLiquid && rb.velocity.sqrMagnitude > sqrMagnitude;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Platform Check
        if(collision.gameObject.TryGetComponent(out Platform platform))
        {
            if(platform.IsPassthrough)
            {
                passThroughPlatformList.Add(platform);
            }
            return;
        }

        // Check In Pipe
        if(isLiquid)
        {
            if (collision.gameObject.TryGetComponent(out PipeEnd pipeEnd))
            {
                StartMovePipe(pipeEnd);
                return;
            }
        }

        // Pick Up Yarn Ball
        if (isInterrupted) return;
        if ((collision.gameObject.CompareTag("YarnBallBox") || collision.gameObject.CompareTag("BossRoomYarnBallBox"))
            && pickedUpYarnBall == null)
        {
            CreateYarnBall();
            if (collision.gameObject.CompareTag("BossRoomYarnBallBox"))
            {
                zone1.GetComponent<Zone1>().ChangeYarnBallBoxPosition();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Platform Check
        if (collision.gameObject.TryGetComponent(out Platform platform))
        {
            if (platform.IsPassthrough)
            {
                passThroughPlatformList.Remove(platform);
            }
        }
    }

    public void StartTextBox(string text, float duration)
    {
        isTalking = true;
        textBox.SetActive(true);
        textBoxText.text = text;
        textBoxEndTime = Time.time + duration;
    }

    public void StopTextBox()
    {
        isTalking = false;
        textBox.SetActive(false);
    }

    public Companion GetCompanion()
    {
        return companion;
    }

    public void SetUpChoice3(string talkText, string responseText)
    {
        choice3TalkText = talkText;
        companion.responseText = responseText;
        companion.hasChoice3 = true;
    }

    public void ClearChoice3()
    {
        companion.hasChoice3 = false;
    }

    public void SetMainObjective(string mainObjective)
    {
        this.mainObjective = mainObjective;
        StatusUIManager.Instance.SetMainObjective(mainObjective);
        PauseUIManager.Instance.SetMainObjective(mainObjective);
    }

}
