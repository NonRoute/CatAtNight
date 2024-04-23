using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone3BossTrigger : MonoBehaviour
{
    private bool isTriggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTriggered)
        {
            Zone3Manager.Instance.StartBossFight();
            GameplayStateManager.Instance.AutoSave();
            while (GameplayStateManager.Instance.isSaving)
            {
                //wait
            }
            DataManager.Instance.DestroyObject(gameObject);
            isTriggered = true;
        }
    }
}
