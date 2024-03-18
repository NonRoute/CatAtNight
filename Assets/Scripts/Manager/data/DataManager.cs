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

    public GameData gameData;

    private DataManager()
    {
        gameData = loadData();
    }

    private string saveFilePath => Application.persistentDataPath + "/save.json";

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
        var dataAsJson = JsonUtility.ToJson(gameData);
        System.IO.File.WriteAllText(saveFilePath, dataAsJson);
    }
}
