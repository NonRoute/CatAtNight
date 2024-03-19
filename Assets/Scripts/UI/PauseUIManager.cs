using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseUIManager : MonoBehaviour
{

    private static PauseUIManager instance;
    public static PauseUIManager Instance => instance;

    [SerializeField] private Canvas canvas;
    [SerializeField] private Player player;
    [SerializeField] private string mainMenu = "Title";

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

    public void TogglePauseMenu()
    {
        StatusUIManager.Instance.ToggleHide(!canvas.enabled);
        canvas.enabled = !canvas.enabled;
    }

    public void SaveBackToMenu()
    {
        player.SaveGame();
        BackToMenu();
    }

    public void BackToMenu()
    {
        SoundManager.TryStop(SoundManager.GetCurrentMusicName());
        SceneManager.LoadScene(mainMenu);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
