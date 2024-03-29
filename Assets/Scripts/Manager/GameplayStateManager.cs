using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayStateManager : MonoBehaviour
{
    private static GameplayStateManager instance;
    public static GameplayStateManager Instance => instance;

    private bool isLoadSave;
    private int saveSlot;
    private bool isPausing;
    private bool isInDialogue;
    private Player player;

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
    }

    public void SetStartMode(bool isLoadSave, int saveSlot)
    {
        this.isLoadSave = isLoadSave;
        this.saveSlot = saveSlot;
    }

}
