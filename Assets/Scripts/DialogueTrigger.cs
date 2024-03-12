using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public DialogueCharacterData character;
    [TextArea(3, 10)]
    public string line;
}

[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;
    private bool isTriggered = false;

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue, gameObject);
        isTriggered = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform?.parent?.CompareTag("Player") == true && !isTriggered)
        {
            TriggerDialogue();
        }
    }
}