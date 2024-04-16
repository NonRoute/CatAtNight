using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Talker : MonoBehaviour
{
    public DialogueTree dialogue;

    public bool firstInteraction = true;
    [SerializeField] int repeatStartPosition;
    [SerializeField] private bool isTriggerWhenCollide = false;
    [SerializeField] private bool destroyAfterFinish = false;

    private bool playerIsNear = false;
    private bool isTalking = false;

    [HideInInspector]
    public int StartPosition
    {
        get
        {
            if (firstInteraction)
            {
                firstInteraction = false;
                return 0;
            }
            else
            {
                return repeatStartPosition;
            }
        }
    }

    private void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onSubmitPressed += SubmitPressed;
        DialogueTreeController.OnDialogueEnded += OnDialogueFinish;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onSubmitPressed -= SubmitPressed;
        DialogueTreeController.OnDialogueEnded -= OnDialogueFinish;
    }

    private void SubmitPressed()
    {
        if (!playerIsNear || isTalking)
        {
            return;
        }

        print(gameObject.name + "submit");
        // start or finish a quest
        OnTriggered();
    }

    private void OnDialogueFinish()
    {
        if(isTalking && destroyAfterFinish)
        {
            Destroy(gameObject);
        }
        isTalking = false;
    }

    private void OnTriggered()
    {
        isTalking = true;
        DialogueTreeController.instance.StartDialogue(dialogue, StartPosition);
    }

    private void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.CompareTag("Player"))
        {
            if (isTriggerWhenCollide)
            {
                OnTriggered();
            }
            else
            {
                playerIsNear = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D otherCollider)
    {
        if (otherCollider.CompareTag("Player"))
        {
            playerIsNear = false;
        }
    }
}
