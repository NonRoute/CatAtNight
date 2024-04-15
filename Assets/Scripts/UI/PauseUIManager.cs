using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseUIManager : MonoBehaviour
{

    private static PauseUIManager instance;
    public static PauseUIManager Instance => instance;

    [SerializeField] private Canvas canvas;
    [SerializeField] private string mainMenu = "Title";
    [SerializeField] private GameObject[] MenuPanels;
    [SerializeField] private Image[] MenuButtons;
    [SerializeField] private PlayerStatusPanel statusPanel;
    [SerializeField] private HelpControlPanel helpPanel;
    [SerializeField] private Color focusButtonColor;
    [SerializeField] private Color unfocusButtonColor;
    [SerializeField] private int currentIndex = 0;

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

    public bool IsOpen => canvas.enabled;

    public void TogglePauseMenu()
    {
        canvas.enabled = !canvas.enabled;
        if(canvas.enabled)
        {
            GameEventsManager.instance.playerEvents.DisablePlayerMovement();
            SwitchMenu(currentIndex);
        }
        else
        {
            GameEventsManager.instance.playerEvents.EnablePlayerMovement();
        }
    }

    public void SwitchMenu(int index)
    {
        for (int i = 0; i < MenuPanels.Length; i++)
        {
            if (index == i)
            {
                MenuButtons[i].color = focusButtonColor;
                MenuPanels[i].SetActive(true);
                InitMenu(index);
                currentIndex = index;
            }
            else
            {
                MenuButtons[i].color = unfocusButtonColor;
                MenuPanels[i].SetActive(false);
            }
        }
    }

    public void InitMenu(int index)
    {
        if (index < 2) return;
        PlayerData playerData = GameplayStateManager.Instance.Player.GetPlayerData();
        if(index == 2) // Player Status
        {
            statusPanel.SetupValue(playerData);
        }
        if (index == 3) // Help Control
        {
            helpPanel.SetupValue(playerData.skillProgression);
        }
    }

    public void SaveBackToMenu()
    {
        GameplayStateManager.Instance.SaveGame();
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
