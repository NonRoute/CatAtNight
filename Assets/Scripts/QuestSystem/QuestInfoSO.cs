using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillProgression
{
    Nothing = 0,
    Dash = 1,
    Companion = 2,
    LiquidForm = 3,
    SpecialEyes = 4,
}

[CreateAssetMenu(fileName = "QuestInfoSO", menuName = "ScriptableObjects/QuestInfoSO", order = 1)]
public class QuestInfoSO : ScriptableObject
{
    [field: SerializeField] public string id { get; private set; }

    [Header("General")]
    public string displayName;
    public bool hideBeforeStarted;
    public bool hideAfterFinished;

    [Header("Requirements")]
    public int progressionRequirement;
    [field: SerializeField] private SkillProgression skillRequirement;
    public int SkillRequirement => (int) skillRequirement;
    public QuestInfoSO[] questPrerequisites;

    [Header("Steps")]
    public GameObject[] questStepPrefabs;

    [Header("Rewards")]
    public int fishReward;
    public Tuple<string,int>[] itemsReward;

    // ensure the id is always the name of the Scriptable Object asset
    private void OnValidate()
    {
        #if UNITY_EDITOR
        id = this.name;
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }
}
