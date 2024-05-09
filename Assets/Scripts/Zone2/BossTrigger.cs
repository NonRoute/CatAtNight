using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] private GameObject boss;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            boss.GetComponent<BossZone2>().isStarted = true;
            SoundManager.TryPlayMusic("Boss2BGM");
            Zone2Manager.Instance?.StartBossFight();
            DataManager.Instance.tempData.position = GameplayStateManager.Instance.Player.GetCameraFollow().position;
            GameplayStateManager.Instance.AutoSave();
        }
    }
}
