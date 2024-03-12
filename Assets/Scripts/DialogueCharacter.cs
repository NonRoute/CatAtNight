using UnityEngine;

[CreateAssetMenu(fileName = "New DialogueCharacter", menuName = "Dialogue/DialogueCharacter")]
public class DialogueCharacterData : ScriptableObject
{
    public string characterName;
    public Sprite characterIcon;
}