using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [SerializeField] private ScoreManager scoreManager;
    public Image characterIcon;
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;

    private Queue<DialogueLine> lines;

    public bool isDialogueActive = false;

    public float typingSpeed = 0.05f;

    public Animator animator;

    private GameObject currentNPC;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        lines = new Queue<DialogueLine>();
    }

    public void StartDialogue(Dialogue dialogue, GameObject NPC)
    {
        GameEventsManager.instance.playerEvents.DisablePlayerMovement();
        isDialogueActive = true;
        currentNPC = NPC;

        gameObject.SetActive(true);
        animator.Play("show");

        lines.Clear();

        foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
        {
            lines.Enqueue(dialogueLine);
        }

        DisplayNextDialogueLine();
    }

    public void DisplayNextDialogueLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine currentLine = lines.Dequeue();

        characterIcon.sprite = currentLine.character.characterIcon;
        characterName.text = currentLine.character.characterName;

        StopAllCoroutines();

        StartCoroutine(TypeSentence(currentLine));
    }

    IEnumerator TypeSentence(DialogueLine dialogueLine)
    {
        dialogueArea.text = "";
        foreach (char letter in dialogueLine.line.ToCharArray())
        {
            dialogueArea.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void EndDialogue()
    {
        GameEventsManager.instance.playerEvents.EnablePlayerMovement();
        isDialogueActive = false;
        animator.Play("hide");
        StartCoroutine(HideDialogue());
        Destroy(currentNPC);
        scoreManager.friendshipScore++;
    }

    IEnumerator HideDialogue()
    {
        yield return new WaitForSeconds(1.0f);
        gameObject.SetActive(false);
    }
}
