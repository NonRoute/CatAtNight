using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone3Manager : MonoBehaviour
{
    private static Zone3Manager instance;
    public static Zone3Manager Instance => instance;

    [SerializeField] private Transform targetGroup;
    [SerializeField] private GameObject bossEntranceBlocker;
    [SerializeField] private GameObject doorToZone4;
    [SerializeField] private GameObject dog;
    [SerializeField] private SlimeBoss boss;
    [SerializeField] private GameObject fishStorage;
    [SerializeField] private GameObject triggerBeforeBoss;
    [SerializeField] private DialogueTree dialogue;
    [SerializeField] private GameObject tempPlatform;


    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    void Start()
    {
        SoundManager.TryPlayMusic("Zone 3 Music");
    }

    [ContextMenu("RemoveDog")]
    public void RemoveDog()
    {
        SoundManager.TryPlayNew("DogDead");
        DataManager.Instance.DestroyObject(dog);
        dog.SetActive(false);
    }

    [ContextMenu("StartBoss")]
    public void StartBossFight()
    {
        GameplayStateManager.Instance.currentCamera.Follow = targetGroup;
        //GameplayStateManager.Instance.AutoSave();
        //while (GameplayStateManager.Instance.isSaving)
        //{
        //    //wait
        //}
        SoundManager.TryPlayMusic("Boss3BGM");
        boss.ActivateBoss();
        GameplayStateManager.Instance.Player.ForceCallCompanion(true);
    }

    [ContextMenu("OpenBossRoom")]
    public void ClearBossEntrance()
    {
        DataManager.Instance.DestroyObject(bossEntranceBlocker);
        bossEntranceBlocker.SetActive(false);
        triggerBeforeBoss.SetActive(true);
    }

    [ContextMenu("AttackBoss")]
    public void AttackBoss()
    {
        boss.ReceiveDamage();
    }

    [ContextMenu("StartPhase2")]
    public void OnStartPhase2()
    {
        DataManager.Instance.DestroyObject(tempPlatform);
        tempPlatform.SetActive(false);
    }

    [ContextMenu("EndBoss")]
    public void AfterBossDead()
    {
        GameplayStateManager.Instance.currentCamera.Follow = GameplayStateManager.Instance.Player.GetCameraFollow();
        SoundManager.TryPlayMusic("Zone 3 Music");

        SoundManager.TryPlayNew("Victory");
        DataManager.Instance.DestroyObject(doorToZone4);
        doorToZone4.SetActive(false);
        DataManager.Instance.DestroyObject(fishStorage);
        fishStorage.SetActive(false);
        GameplayStateManager.Instance.Player.SetUnlockedSkill(3);
        GameEventsManager.instance.miscEvents.Boss3Dead();
        DialogueTreeController.instance.StartDialogue(dialogue, 0);

        DataManager.Instance.tempData.position = GameplayStateManager.Instance.Player.GetCameraFollow().position;
        GameplayStateManager.Instance.AutoSave();
    }
}
