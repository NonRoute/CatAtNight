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

    [ContextMenu("StartBoss")]
    public void StartBossFight()
    {
        GameplayStateManager.Instance.currentCamera.Follow = targetGroup;
        GameplayStateManager.Instance.AutoSave();
        while (GameplayStateManager.Instance.isSaving)
        {
            //wait
        }
        SoundManager.TryPlayMusic("Boss3BGM");
    }

    [ContextMenu("OpenBoss")]
    public void ClearBossEntrance()
    {
        DataManager.Instance.DestroyObject(bossEntranceBlocker);
        bossEntranceBlocker.SetActive(false);
    }

    [ContextMenu("EndBoss")]
    public void AfterBossDead()
    {
        GameplayStateManager.Instance.currentCamera.Follow = GameplayStateManager.Instance.Player.GetCameraFollow();
        SoundManager.TryPlayMusic("Zone 3 Music");

        SoundManager.TryPlay("Victory");
        DataManager.Instance.DestroyObject(doorToZone4);
        doorToZone4.SetActive(false);
        GameplayStateManager.Instance.Player.SetUnlockedSkill(3);

        DataManager.Instance.tempData.position = GameplayStateManager.Instance.Player.GetCameraFollow().position;
        GameplayStateManager.Instance.AutoSave();
    }
}
