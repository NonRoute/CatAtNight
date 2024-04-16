using UnityEditor;
using UnityEngine;

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

    public void DestroyObject(Object obj)
    {
        GlobalObjectId id = GlobalObjectId.GetGlobalObjectIdSlow(obj);
        gameData.destroyedObjects.Add(id.ToString());
        tempData.destroyedObjects.Add(id.ToString());
    }
}
