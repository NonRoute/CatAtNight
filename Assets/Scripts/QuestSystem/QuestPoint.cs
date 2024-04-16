using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPoint : MonoBehaviour
{
    [Header("Quest")]
    [SerializeField] private QuestInfoSO questInfoForPoint;

    [Header("Config")]
    [SerializeField] private bool startPoint = true;
    [SerializeField] private bool finishPoint = true;

    [SerializeField] private bool isTriggerWhenCollide = false;
    [SerializeField] private bool destroyAfterStartQuest = false;
    [SerializeField] private bool destroyAfterFinishQuest = false;

    [SerializeField] private bool hasDialogue = false;
    [SerializeField] private DialogueTree dialogue;
    [SerializeField] private int repeatedSection;
    [SerializeField] private int startQuestSection;
    [SerializeField] private int finishQuestSection;
    [SerializeField] private bool isTalking = false;
    [SerializeField] private bool needToAcceptQuest = false;

    private bool playerIsNear = false;
    private string questId;
    private QuestState currentQuestState;

    private QuestIcon questIcon;

    private void Awake()
    {
        questId = questInfoForPoint.id;
        questIcon = GetComponentInChildren<QuestIcon>();
    }

    private void OnEnable()
    {
        GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
        GameEventsManager.instance.inputEvents.onSubmitPressed += SubmitPressed;
        DialogueTreeController.OnDialogueStarted += OnFinishConversation;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
        GameEventsManager.instance.inputEvents.onSubmitPressed -= SubmitPressed;
        DialogueTreeController.OnDialogueStarted -= OnFinishConversation;
    }

    private void SubmitPressed()
    {
        if (!playerIsNear)
        {
            return;
        }

        print(gameObject.name + "submit");
        // start or finish a quest
        OnTriggered();
    }

    private void OnTriggered()
    {
        if (currentQuestState.Equals(QuestState.CAN_START) && startPoint)
        {
            if (hasDialogue)
            {
                isTalking = true;
                DialogueTreeController.instance.StartDialogue(dialogue, startQuestSection);
            }
            else
            {
                StartQuest();
            }
        }
        else if (currentQuestState.Equals(QuestState.CAN_FINISH) && finishPoint)
        {
            if (hasDialogue)
            {
                isTalking = true;
                DialogueTreeController.instance.StartDialogue(dialogue, finishQuestSection);
            }
            else
            {
                FinishQuest();
            }
        }
        else
        {
            if (hasDialogue)
            {
                DialogueTreeController.instance.StartDialogue(dialogue, repeatedSection);
            }
        }
    }

    private void OnFinishConversation()
    {
        if (hasDialogue && isTalking)
        {
            if (!needToAcceptQuest || DialogueTreeController.instance.isAcceptClicked)
            {
                if (currentQuestState.Equals(QuestState.CAN_START) && startPoint)
                {
                    StartQuest();
                }
                else if (currentQuestState.Equals(QuestState.CAN_FINISH) && finishPoint)
                {
                    FinishQuest();
                }
            }
            isTalking = false;
        }
    }

    private void StartQuest()
    {
        if (destroyAfterStartQuest)
        {
            Destroy(gameObject);
        }
        GameEventsManager.instance.questEvents.StartQuest(questId);
    }
    private void FinishQuest()
    {
        if (destroyAfterFinishQuest)
        {
            Destroy(gameObject);
        }
        GameEventsManager.instance.questEvents.FinishQuest(questId);
    }

    private void QuestStateChange(Quest quest)
    {
        // only update the quest state if this point has the corresponding quest
        if (quest.info.id.Equals(questId))
        {
            currentQuestState = quest.state;
            questIcon.SetState(currentQuestState, startPoint, finishPoint, hasDialogue);
        }
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
