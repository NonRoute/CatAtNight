using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossZone2 : Monster
{
    [SerializeField] private GameObject door;
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] public bool isStarted;
    private Transform player;
    private Vector2 vel;
    public float smoothTime;
    protected override void Start()
    {
        base.Start();
        CinemachineVirtualCamera cam = FindAnyObjectByType<CinemachineVirtualCamera>();
    }

    protected new void FixedUpdate()
    {
        if (isStarted)
        {
            if (player == null)
            {
                player = GameplayStateManager.Instance.Player.transform;
                return;
            }
            Vector3 offset = new Vector3(0, 0, 0);
            transform.position = Vector2.SmoothDamp(transform.position, player.position + offset, ref vel, smoothTime);
        }
    }
    public void OnDead()
    {
        cam.Follow = GameplayStateManager.Instance.Player.GetCameraFollow();
        DataManager.Instance.DestroyObject(gameObject);
        gameObject.SetActive(false);
        DataManager.Instance.DestroyObject(door);
        door.SetActive(false);
        GameplayStateManager.Instance.Player.SetUnlockedSkill(1);
        DataManager.Instance.tempData.position = GameplayStateManager.Instance.Player.GetCameraFollow().position;
        GameplayStateManager.Instance.AutoSave();
        //Destroy(door);
        SoundManager.TryPlay("Victory");
        SoundManager.TryPlayMusic("Zone 2 Music");
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
    }
}
