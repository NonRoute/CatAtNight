using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestManager : MonoBehaviour, ISavable
{
    [Header("Config")]
    [SerializeField] private bool loadQuestState = true;

    private Dictionary<string, Quest> questMap;

    private Dictionary<string, string> tempQuestDict;

    // quest start requirements
    private int currentProgression;
    private int currentSkillProgression;
    private void Awake()
    {
        questMap = CreateNewQuestMap();
    }

    private void OnEnable()
    {
        GameEventsManager.instance.questEvents.onRequirementChange += UpdateRequirement;

        GameEventsManager.instance.questEvents.onStartQuest += StartQuest;
        GameEventsManager.instance.questEvents.onAdvanceQuest += AdvanceQuest;
        GameEventsManager.instance.questEvents.onFinishQuest += FinishQuest;

        GameEventsManager.instance.questEvents.onQuestStepStateChange += QuestStepStateChange;

        GameEventsManager.instance.playerEvents.onPlayerProgressionChange += ProgressionChange;
        GameEventsManager.instance.playerEvents.onPlayerSkillProgressionChange += SkillProgressionChange;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.questEvents.onRequirementChange -= UpdateRequirement;

        GameEventsManager.instance.questEvents.onStartQuest -= StartQuest;
        GameEventsManager.instance.questEvents.onAdvanceQuest -= AdvanceQuest;
        GameEventsManager.instance.questEvents.onFinishQuest -= FinishQuest;

        GameEventsManager.instance.questEvents.onQuestStepStateChange -= QuestStepStateChange;

        GameEventsManager.instance.playerEvents.onPlayerProgressionChange -= ProgressionChange;
        GameEventsManager.instance.playerEvents.onPlayerSkillProgressionChange -= SkillProgressionChange;
    }

    private void Start()
    {
        ReloadState();
        UpdateRequirement();
    }

    private void ReloadState()
    {
        foreach (Quest quest in questMap.Values)
        {
            // initialize any loaded quest steps
            if (quest.state == QuestState.IN_PROGRESS)
            {
                quest.InstantiateCurrentQuestStep(this.transform);
            }
            // broadcast the initial state of all quests on startup
            GameEventsManager.instance.questEvents.QuestStateChange(quest);
        }
    }

    private void ChangeQuestState(string id, QuestState state)
    {
        Quest quest = GetQuestById(id);
        quest.state = state;
        GameEventsManager.instance.questEvents.QuestStateChange(quest);
    }

    private bool CheckRequirementsMet(Quest quest)
    {
        // start true and prove to be false
        bool meetsRequirements = true;

        // check player level requirements
        if (currentProgression < quest.info.progressionRequirement)
        {
            meetsRequirements = false;
        }
        // check player skill requirements
        if (currentSkillProgression < quest.info.SkillRequirement)
        {
            meetsRequirements = false;
        }
        // check player item requirements
        foreach(ItemCount itemData in quest.info.itemsRequirement)
        {
            if(PlayerInventory.Instance.CheckItem(itemData.ItemName) < itemData.Amount)
            {
                meetsRequirements = false;
            }
        }

        // check quest prerequisites for completion
        foreach (QuestInfoSO prerequisiteQuestInfo in quest.info.questPrerequisites)
        {
            if (GetQuestById(prerequisiteQuestInfo.id).state != QuestState.FINISHED)
            {
                meetsRequirements = false;
            }
        }

        return meetsRequirements;
    }

    private void UpdateRequirement()
    {
        // loop through ALL quests
        foreach (Quest quest in questMap.Values)
        {
            // if we're now meeting the requirements, switch over to the CAN_START state
            if (quest.state == QuestState.REQUIREMENTS_NOT_MET && CheckRequirementsMet(quest))
            {
                ChangeQuestState(quest.info.id, QuestState.CAN_START);
            }
            GameEventsManager.instance.questEvents.QuestStateChange(quest);
        }
    }

    private void StartQuest(string id) 
    {
        Quest quest = GetQuestById(id);
        quest.InstantiateCurrentQuestStep(this.transform);
        ChangeQuestState(quest.info.id, QuestState.IN_PROGRESS);
    }

    private void AdvanceQuest(string id)
    {
        Quest quest = GetQuestById(id);

        // move on to the next step
        quest.MoveToNextStep();

        // if there are more steps, instantiate the next one
        if (quest.CurrentStepExists())
        {
            quest.InstantiateCurrentQuestStep(this.transform);
        }
        // if there are no more steps, then we've finished all of them for this quest
        else
        {
            ChangeQuestState(quest.info.id, QuestState.CAN_FINISH);
        }
    }

    private void FinishQuest(string id)
    {
        Quest quest = GetQuestById(id);
        ClaimRewards(quest);
        ChangeQuestState(quest.info.id, QuestState.FINISHED);
    }

    private void ClaimRewards(Quest quest)
    {
        GameEventsManager.instance.fishEvents.GoldGained(quest.info.fishReward);
        GameEventsManager.instance.playerEvents.ItemsGained(quest.info.itemsReward);
    }

    private void QuestStepStateChange(string id, int stepIndex, QuestStepState questStepState)
    {
        Quest quest = GetQuestById(id);
        quest.StoreQuestStepState(questStepState, stepIndex);
        ChangeQuestState(id, quest.state);
    }

    private Dictionary<string, Quest> CreateNewQuestMap()
    {
        // loads all QuestInfoSO Scriptable Objects under the Assets/Resources/Quests folder
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");
        // Create the quest map
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        foreach (QuestInfoSO questInfo in allQuests)
        {
            if (idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning("Duplicate ID found when creating quest map: " + questInfo.id);
            }
            idToQuestMap.Add(questInfo.id, new Quest(questInfo));
        }
        return idToQuestMap;
    }

    private Dictionary<string, Quest> CreateQuestMap()
    {
        // loads all QuestInfoSO Scriptable Objects under the Assets/Resources/Quests folder
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");
        // Create the quest map
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        tempQuestDict = DataManager.Instance.gameData.allQuestData.ToDictionary(t => t.QuestId, t => t.QuestData);
        foreach (QuestInfoSO questInfo in allQuests)
        {
            if (idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning("Duplicate ID found when creating quest map: " + questInfo.id);
            }
            idToQuestMap.Add(questInfo.id, LoadQuest(questInfo));
        }
        return idToQuestMap;
    }

    private Quest GetQuestById(string id)
    {
        Quest quest = questMap[id];
        if (quest == null)
        {
            Debug.LogError("ID not found in the Quest Map: " + id);
        }
        return quest;
    }

    //private void OnApplicationQuit()
    //{
    //    SaveAllQuest();
    //}

    private void SaveAllQuest()
    {
        DataManager.Instance.gameData.allQuestData = new();
        foreach (Quest quest in questMap.Values)
        {
            SaveQuest(quest);
        }
    }

    private void SaveQuest(Quest quest)
    {
        try 
        {
            QuestData questData = quest.GetQuestData();
            // serialize using JsonUtility, but use whatever you want here (like JSON.NET)
            string serializedData = JsonUtility.ToJson(questData);

            //DataManager.Instance.gameData.allQuestData[quest.info.id] = serializedData;
            DataManager.Instance.gameData.allQuestData.Add(new QuestAndData(quest.info.id, serializedData));
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to save quest with id " + quest.info.id + ": " + e);
        }
    }

    private Quest LoadQuest(QuestInfoSO questInfo)
    {
        Quest quest = null;
        try
        {
            // load quest from saved data
            if (tempQuestDict.ContainsKey(questInfo.id) && loadQuestState)
            {
                string serializedData = tempQuestDict[questInfo.id];
                QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);
                quest = new Quest(questInfo, questData.state, questData.questStepIndex, questData.questStepStates);
            }
            // otherwise, initialize a new quest
            else
            {
                quest = new Quest(questInfo);
            }

            //if (PlayerPrefs.HasKey(questInfo.id) && loadQuestState)
            //{
            //    string serializedData = PlayerPrefs.GetString(questInfo.id);
            //    QuestData questData = JsonUtility.FromJson<QuestData>(serializedData);
            //    quest = new Quest(questInfo, questData.state, questData.questStepIndex, questData.questStepStates);
            //}
            //else 
            //{
            //    quest = new Quest(questInfo);
            //}
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to load quest with id " + quest.info.id + ": " + e);
        }
        return quest;
    }

    public void PreserveData()
    {
        DataManager.Instance.tempData.allQuestData = new();
        foreach (Quest quest in questMap.Values)
        {
            try
            {
                QuestData questData = quest.GetQuestData();
                // serialize using JsonUtility, but use whatever you want here (like JSON.NET)
                string serializedData = JsonUtility.ToJson(questData);

                //DataManager.Instance.gameData.allQuestData[quest.info.id] = serializedData;
                DataManager.Instance.tempData.allQuestData.Add(new QuestAndData(quest.info.id, serializedData));
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to save quest with id " + quest.info.id + ": " + e);
            }
        }
    }

    public void RestoreData()
    {
        // loads all QuestInfoSO Scriptable Objects under the Assets/Resources/Quests folder
        QuestInfoSO[] allQuests = Resources.LoadAll<QuestInfoSO>("Quests");
        // Create the quest map
        Dictionary<string, Quest> idToQuestMap = new Dictionary<string, Quest>();
        tempQuestDict = DataManager.Instance.tempData.allQuestData.ToDictionary(t => t.QuestId, t => t.QuestData);
        foreach (QuestInfoSO questInfo in allQuests)
        {
            if (idToQuestMap.ContainsKey(questInfo.id))
            {
                Debug.LogWarning("Duplicate ID found when creating quest map: " + questInfo.id);
            }
            idToQuestMap.Add(questInfo.id, LoadQuest(questInfo));
        }
        questMap = idToQuestMap;

        ReloadState();
    }

    public void Save()
    {
        SaveAllQuest();
    }

    public void LoadSave()
    {
        questMap = CreateQuestMap();
        ReloadState();
        UpdateRequirement();
    }

    private void ProgressionChange(int progression)
    {
        currentProgression = Math.Max(currentProgression, progression);
        UpdateRequirement();
    }

    public void SkillProgressionChange(int skillProgression)
    {
        currentSkillProgression = skillProgression;
        UpdateRequirement();
    }
}
