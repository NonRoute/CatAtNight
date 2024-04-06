using System.Collections;
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
        player.RestoreFromSave();
        isLoadSave = false;
    }

    public void SaveGame()
    {
        player.SaveGame();
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
