using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour, ISavable
{
    private static PlayerInventory instance;
    public static PlayerInventory Instance => instance;

    private Dictionary<string, int> inventory;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    public void AddItem(string item)
    {
        if(!inventory.ContainsKey(item))
        {
            inventory[item] = 0;
        }
        inventory[item]++;
    }

    public void RemoveItem(string item)
    {
        if (inventory.ContainsKey(item))
        {
            inventory[item]--;
        }
    }

    public int CheckItem(string item)
    {
        if (!inventory.ContainsKey(item))
        {
            return 0;
        }
        return inventory[item];
    }

    public bool IsStayInScene()
    {
        return false;
    }

    public void Save()
    {
        DataManager.Instance.gameData.inventory = inventory;
    }

    public void LoadSave()
    {
        inventory = DataManager.Instance.gameData.inventory;
    }
}
