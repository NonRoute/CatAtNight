using Cinemachine;
using System.Collections;
using System.Linq;
using UnityEngine;
using StoneShelter;
using UnityEngine.SceneManagement;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class GameplayStateManager : MonoBehaviour
{
    private static GameplayStateManager instance;
    public static GameplayStateManager Instance => instance;

    [SerializeField] private bool isLoadSave;
    public bool isNewGame;
    public int saveSlot;
    private bool isChangingZone = false;
    private bool isPausing;
    public bool isInDialogue;
    private Player player;
    public Player Player => player;
    public CinemachineVirtualCamera mainCamera;
    public CinemachineVirtualCamera currentCamera;

    public bool isSaving = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("GameplayStateManager: OnSceneLoaded");
        if(isChangingZone)
        {
            StartCoroutine(RestoreData());
            isChangingZone = false;
        }
        if(isLoadSave)
        {
            StartCoroutine(LoadSave());
        }
        if(isNewGame)
        {
            // New Game
            StartCoroutine(OnNewGame());
            isNewGame = false;
        }
    }

    IEnumerator RestoreData()
    {
        Debug.Log("Waiting for Player Reference");
        yield return new WaitUntil(() => player != null);
        Debug.Log("Player Reference Get! Loading Values");
        foreach (ISavable savable in FindObjectsOfType<MonoBehaviour>(true).OfType<ISavable>().ToArray())
        {
            savable.RestoreData();
        }
        DataManager.Instance.tempData.currentScene = SceneManager.GetActiveScene().buildIndex;
        DataManager.Instance.tempData.sceneName = SceneManager.GetActiveScene().name;
        DataManager.Instance.tempData.dateTime = (JsonDateTime)System.DateTime.Now;
        DataManager.Instance.autoSave();
        LoadDestroyedObjects(DataManager.Instance.tempData);
    }

    IEnumerator LoadSave()
    {
        Debug.Log("Waiting for Player Reference");
        yield return new WaitUntil(() => player != null);
        Debug.Log("Player Reference Get! Loading Values");
        foreach (ISavable savable in FindObjectsOfType<MonoBehaviour>(true).OfType<ISavable>().ToArray())
        {
            savable.LoadSave();
        }
        //player.RestoreFromSave();
        isLoadSave = false;
        LoadDestroyedObjects(DataManager.Instance.gameData);
    }

    IEnumerator OnNewGame()
    {
        Debug.Log("Waiting for Player Reference");
        yield return new WaitUntil(() => player != null);
        Debug.Log("Player Reference Get! Loading Values");
        print("NEWGAME");
        player.NewGame();
    }

    public void SaveGame()
    {
        isSaving = true;
        // For Data outside Scene
        DataManager.Instance.gameData = DataManager.Instance.tempData;
        // For Data inside Scene
        foreach(ISavable savable in FindObjectsOfType<MonoBehaviour>(true).OfType<ISavable>().ToArray())
        {
            savable.Save();
        }
        DataManager.Instance.gameData.currentScene = SceneManager.GetActiveScene().buildIndex;
        DataManager.Instance.gameData.sceneName = SceneManager.GetActiveScene().name;
        DataManager.Instance.gameData.dateTime = (JsonDateTime) System.DateTime.Now;

        //DataManager.Instance.gameData.inventory.Add(new ItemCount("TestTempItem", 4));
        //DataManager.Instance.gameData.allQuestData.Add(new QuestAndData("TestTempQuest", "data idk"));

        DataManager.Instance.saveData();
        isSaving = false;
    }

    public void SetStartMode(bool isLoadSave, int saveSlot)
    {
        this.isLoadSave = isLoadSave;
        this.saveSlot = saveSlot;
    }

    public void SetPlayer(Player player)
    {
        this.player = player;
    }

    public void ChangeZone(string sceneName)
    {
        SoundManager.TryStop(SoundManager.GetCurrentMusicName());
        isChangingZone = true;
        PreserveData();
        player = null;
        SceneManager.LoadScene(sceneName);
    }

    public void PreserveData()
    {
        foreach (ISavable savable in FindObjectsOfType<MonoBehaviour>(true).OfType<ISavable>().ToArray())
        {
            savable.PreserveData();
        }
    }

    public void LoadDestroyedObjects(GameData data)
    {
        foreach(string str in data.destroyedObjects)
        {
            GUID guid = GUID.Find(str);
            if (guid != null)
            {
                GameObject obj = guid.gameObject;
                obj.SetActive(false);
            }
        }
    }

    public void SetCamera(CinemachineVirtualCamera virtualCamera)
    {
        if(currentCamera != null)
        {
            currentCamera.gameObject.SetActive(false);
        }
        currentCamera = virtualCamera;
        currentCamera.gameObject.SetActive(true);
    }

    public void AutoSave()
    {
        isSaving = true;
        PreserveData();
        DataManager.Instance.tempData.currentScene = SceneManager.GetActiveScene().buildIndex;
        DataManager.Instance.tempData.sceneName = SceneManager.GetActiveScene().name;
        DataManager.Instance.tempData.dateTime = (JsonDateTime)System.DateTime.Now;
        DataManager.Instance.autoSave();
        isSaving = false;
    }

}
