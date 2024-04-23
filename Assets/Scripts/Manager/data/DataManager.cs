using UnityEngine;
using StoneShelter;

#nullable enable

public class DataManager
{
    private static DataManager? instance;
    public static DataManager Instance
    {
        get
        {
            instance ??= new DataManager();
            return instance;
        }
        private set { }
    }

    public GameData tempData;

    public GameData gameData;

    private DataManager()
    {
        gameData = loadData();
        tempData = gameData;
    }

    private string saveFilePath => Application.persistentDataPath + "/save" + GameplayStateManager.Instance.saveSlot + ".json";

    private GameData loadData()
    {
        var filePath = saveFilePath;

        if (System.IO.File.Exists(filePath))
        {
            var dataAsJson = System.IO.File.ReadAllText(filePath);
            return JsonUtility.FromJson<GameData>(dataAsJson);
        }
        else
        {
            return new GameData();
        }
    }

    public void saveData()
    {
        var dataAsJson = JsonUtility.ToJson(gameData,prettyPrint:true);
        System.IO.File.WriteAllText(saveFilePath, dataAsJson);
    }

    public void reloadData()
    {
        gameData = loadData();
        tempData = gameData;
    }

    public void autoSave()
    {
        var dataAsJson = JsonUtility.ToJson(tempData, prettyPrint: true);
        var autoSaveFilePath = Application.persistentDataPath + "/save0.json";
        System.IO.File.WriteAllText(autoSaveFilePath, dataAsJson);
    }

    public GameData getData(int saveSlot)
    {
        string filePath = Application.persistentDataPath + "/save" + saveSlot + ".json";
        if (System.IO.File.Exists(filePath))
        {
            var dataAsJson = System.IO.File.ReadAllText(filePath);
            return JsonUtility.FromJson<GameData>(dataAsJson);
        }
        else
        {
            return null;
        }
    }

    public void DestroyObject(GameObject obj)
    {
        string obj_guid = obj.GetComponent<GUID>().guid;
        if (tempData.destroyedObjects.Contains(obj_guid)) return;
        gameData.destroyedObjects.Add(obj_guid);
        tempData.destroyedObjects.Add(obj_guid);
    }

    public void resetData()
    {
        gameData = new();
        tempData = gameData;
    }
}
