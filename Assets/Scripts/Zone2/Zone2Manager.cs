using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone2Manager : MonoBehaviour
{
    private static Zone2Manager instance;
    public static Zone2Manager Instance => instance;

    [SerializeField] private Transform targetGroup;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        SoundManager.TryPlayMusic("Zone 2 Music");
    }

    [ContextMenu("StartBoss")]
    public void StartBossFight()
    {
        GameplayStateManager.Instance.currentCamera.Follow = targetGroup;
    }

    [ContextMenu("EndBoss")]
    public void AfterBossDead()
    {
        GameplayStateManager.Instance.currentCamera.Follow = GameplayStateManager.Instance.Player.GetCameraFollow();
    }
}
