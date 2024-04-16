using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour, ISavable
{
    private static PlayerInventory instance;
    public static PlayerInventory Instance => instance;

    private Dictionary<string, int> inventory = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void AddItem(string item)
    {
        if(!inventory.ContainsKey(item))
        {
            inventory[item] = 0;
        }
        inventory[item]++;
        GameEventsManager.instance.questEvents.QuestRequirementChange();
    }

    public void RemoveItem(string item)
    {
        if (inventory.ContainsKey(item))
        {
            inventory[item]--;
        }
        GameEventsManager.instance.questEvents.QuestRequirementChange();
    }

    public int CheckItem(string item)
    {
        if (!inventory.ContainsKey(item))
        {
            return 0;
        }
        return inventory[item];
    }

    public void PreserveData()
    {
        DataManager.Instance.tempData.inventory = inventory.Select(x => new ItemCount(x.Key, x.Value)).ToList();
    }

    public void RestoreData()
    {
        inventory = DataManager.Instance.tempData.inventory.ToDictionary(t => t.ItemName, t => t.Amount);
    }

    public void Save()
    {
        DataManager.Instance.gameData.inventory = inventory.Select(x => new ItemCount(x.Key, x.Value)).ToList();
    }

    public void LoadSave()
    {
        inventory = DataManager.Instance.gameData.inventory.ToDictionary(t => t.ItemName, t => t.Amount);
    }
}
