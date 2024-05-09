using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    private bool isTriggered = false;
    [SerializeField] private GameObject boss;
    [SerializeField] private DialogueTree dialogue;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isTriggered)
        {
            boss.GetComponent<BossZone2>().isStarted = true;
            Zone2Manager.Instance?.StartBossFight();
            DialogueTreeController.instance.StartDialogue(dialogue, 0);
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
