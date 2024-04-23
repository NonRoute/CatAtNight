using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetGem : MonoBehaviour
{
    public DialogueTree dialogue;
    public bool isTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTriggered) return;
        if (other.gameObject.CompareTag("Player"))
        {
            GameEventsManager.instance.miscEvents.GemGet();
            DialogueTreeController.instance.StartDialogue(dialogue, 0);
            DataManager.Instance.DestroyObject(gameObject);
            gameObject.SetActive(false);
            isTriggered = true;
        }
    }

}
