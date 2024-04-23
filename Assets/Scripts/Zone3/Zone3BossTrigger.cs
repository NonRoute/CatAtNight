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
            DataManager.Instance.tempData.position = GameplayStateManager.Instance.Player.GetCameraFollow().position;
            GameplayStateManager.Instance.AutoSave();
            while (GameplayStateManager.Instance.isSaving)
            {
                //wait
            }
            StartCoroutine(DestroyDelayed(0.2f));
            isTriggered = true;
        }
    }

    IEnumerator DestroyDelayed(float duration)
    {
        yield return new WaitForSeconds(duration);
        DataManager.Instance.DestroyObject(gameObject);
    }

}
