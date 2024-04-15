using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class GameplayStateManager : MonoBehaviour
{
    private static GameplayStateManager instance;
    public static GameplayStateManager Instance => instance;

    [SerializeField] private bool isLoadSave;
    [SerializeField] private int saveSlot;
    private bool isPausing;
    private bool isInDialogue;
    private Player player;
    public Player Player => player;

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
        if(isLoadSave)
        {
            StartCoroutine(LoadSave());
        }
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
    }

    public void SaveGame()
    {
        foreach(ISavable savable in FindObjectsOfType<MonoBehaviour>(true).OfType<ISavable>().ToArray())
        {
            savable.Save();
        }
        DataManager.Instance.gameData.currentScene = SceneManager.GetActiveScene().buildIndex;
        DataManager.Instance.gameData.sceneName = SceneManager.GetActiveScene().name;
        DataManager.Instance.saveData();
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

}
