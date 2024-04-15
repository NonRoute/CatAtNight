using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class DialogueTree : ScriptableObject
{
    public DialogueSection[] sections;
}

[System.Serializable]
public struct DialogueSection
{
    public NewDialogue[] dialogue;
    public bool endAfterDialogue;
    public bool goToNextDialogue;
    public int nextDialogue;
    public BranchPoint branchPoint;
}


[System.Serializable]
public struct NewDialogue
{
    [TextArea]
    public string text;
    public DialogueAction action;
}

[System.Serializable]
public struct BranchPoint
{
    [TextArea]
    public string question;
    public Answer[] answers;
}

[System.Serializable]
public struct Answer
{
    public string answerLabel;
    public int nextElement;
    public DialogueAction action;
}

[System.Serializable]
public struct DialogueAction
{
    public DialogueActionType type;
    public string parameter;
}


[System.Serializable]
public enum DialogueActionType
{
    Nothing,
    AcceptQuest,
    RejectQuest,
    StartQuest,
    AddItem,
    RemoveItem,
}
