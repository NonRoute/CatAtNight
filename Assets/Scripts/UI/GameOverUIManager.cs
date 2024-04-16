using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUIManager : MonoBehaviour
{
    private static GameOverUIManager instance;
    public static GameOverUIManager Instance => instance;

    [SerializeField] private string clickSfxName = "TestClick";
    [SerializeField] private string mainMenu = "Title";
    [SerializeField] private Canvas canvas;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        canvas = GetComponent<Canvas>();
    }

    public void OpenGameOverUI()
    {
        canvas.enabled = true;
    }

    public void LoadAutoSave()
    {
        GameplayStateManager.Instance.SetStartMode(isLoadSave:true, saveSlot:0);
        DataManager.Instance.reloadData();
        string sceneName = DataManager.Instance.gameData.sceneName;
        if (sceneName != "")
        {
            SoundManager.TryPlayNew(clickSfxName);
            SceneManager.LoadScene(sceneName);
        }
    }

    public void BackToMenu()
    {
        SoundManager.TryStop(SoundManager.GetCurrentMusicName());
        SceneManager.LoadScene(mainMenu);
    }
}
