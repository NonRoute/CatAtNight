using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZone2 : Monster
{
    [SerializeField] private GameObject door;
    [SerializeField] private CinemachineVirtualCamera cam;
    public bool isStarted;
    [SerializeField] private float baseSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float lastStopTime;
    [SerializeField] private float stopDuration = 2f;
    private Transform player;
    protected override void Start()
    {
        base.Start();
        CinemachineVirtualCamera cam = FindAnyObjectByType<CinemachineVirtualCamera>();
        baseSpeed = moveSpeed;
    }

    protected new void FixedUpdate()
    {
        if (isStarted && !GameplayStateManager.Instance.isInDialogue && Time.time - lastStopTime > stopDuration)
        {
            if (player == null)
            {
                player = GameplayStateManager.Instance.Player.transform;
                return;
            }
            Vector3 offset = new Vector3(0, 0, 0);
            //transform.position = Vector2.MoveTowards(transform.position, player.position + offset, moveSpeed);
            moveSpeed += acceleration * Time.fixedDeltaTime;
            if(moveSpeed > maxSpeed)
            {
                moveSpeed = maxSpeed;
            }
            transform.position = Vector2.MoveTowards(transform.position, player.position + offset, moveSpeed);
        }
    }
    public void OnDead()
    {
        cam.Follow = GameplayStateManager.Instance.Player.GetCameraFollow();
        DataManager.Instance.DestroyObject(gameObject);
        gameObject.SetActive(false);
        DataManager.Instance.DestroyObject(door);
        door.SetActive(false);
        GameplayStateManager.Instance.Player.SetUnlockedSkill(2);
        DataManager.Instance.tempData.position = GameplayStateManager.Instance.Player.GetCameraFollow().position;
        GameplayStateManager.Instance.AutoSave();
        //Destroy(door);
        SoundManager.TryPlay("Victory");
        SoundManager.TryPlayMusic("Zone 2 Music");
        Zone2Manager.Instance.AfterBossDead();
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
        SoundManager.TryPlay("Boss2Hit");
        moveSpeed = baseSpeed;
        lastStopTime = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out IDamagable damagable))
        {
            if (!damageInfo.targetEntityType.HasFlag(damagable.GetEntityType())) return;

            damagable.RecieveDamage(damageInfo, collision.transform.position);
            moveSpeed = baseSpeed;
            SoundManager.TryPlayNew("DemonLaugh");
            lastStopTime = Time.time;
        }
    }
}
