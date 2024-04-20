using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable

[Serializable]
public struct JsonDateTime
{
    public long value;
    public static implicit operator DateTime(JsonDateTime jdt)
    {
        Debug.Log("Converted to time");
        return DateTime.FromFileTimeUtc(jdt.value);
    }
    public static implicit operator JsonDateTime(DateTime dt)
    {
        Debug.Log("Converted to JDT");
        JsonDateTime jdt = new JsonDateTime();
        jdt.value = dt.ToFileTimeUtc();
        return jdt;
    }
}

[Serializable]
public struct ItemCount
{
    public string ItemName;
    public int Amount;

    public ItemCount(string itemName, int amount)
    {
        ItemName = itemName;
        Amount = amount;
    }

    public static int CompareByNames(ItemCount i1, ItemCount i2)
    {
        return String.Compare(i1.ItemName, i2.ItemName);
    }
}

[Serializable]
public struct QuestAndData
{
    public string QuestId;
    public string QuestData;

    public QuestAndData(string questId, string questData)
    {
        QuestId = questId;
        QuestData = questData;
    }
}

[Serializable]
public class GameData
{
    // Game Progress
    // TODO Implement
    public JsonDateTime dateTime;
    public int skillProgression;
    public int unlockedLevel;

    // TODO Implement
    public int score;
    public List<string> destroyedObjects = new();
    public string mainObjective = "";

    // Position
    public int currentScene;
    public string sceneName = "";
    public Vector3 position = new(-3.13f, -0.41f, 0f);

    // State
    public float health = 9f;
    public float maxHealth = 9f;
    public bool liquidMode;
    public List<ItemCount> inventory = new();
    public int fishCount;

    // Quest
    public List<QuestAndData> allQuestData = new();

    // Other
    public bool isKeyPickedUp;
}
