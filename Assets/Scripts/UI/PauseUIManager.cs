using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.EventSystems;

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
    [SerializeField] private GameObject saveSlotPanel;

    [SerializeField] private TMP_Text[] unlockedLevelTexts;
    [SerializeField] private TMP_Text[] saveDateTexts;
    [SerializeField] private TMP_Text mainObjectiveText;
    [SerializeField] private QuestLogUI questLogUI;

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
        if (canvas.enabled)
        {
            GameEventsManager.instance.playerEvents.DisablePlayerMovement();
            SwitchMenu(currentIndex);
        }
        else
        {
            GameEventsManager.instance.playerEvents.EnablePlayerMovement();
            EventSystem.current.SetSelectedGameObject(null);
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
        if (index == 0)
        {
            questLogUI.OnShowUI();
        }
        if (index < 2) return;
        PlayerData playerData = GameplayStateManager.Instance.Player.GetPlayerData();
        if (index == 2) // Player Status
        {
            statusPanel.SetupValue(playerData);
        }
        if (index == 3) // Help Control
        {
            helpPanel.SetupValue(playerData.skillProgression);
        }
    }

    public void ToggleSelectSaveMenu()
    {
        saveSlotPanel.SetActive(!saveSlotPanel.activeSelf);
        if (saveSlotPanel.activeSelf)
        {
            LoadSaveSlotText();
        }
    }

    public void SelectSaveSlot(int slot)
    {
        GameplayStateManager.Instance.saveSlot = slot;
    }

    public void SaveGame()
    {
        GameplayStateManager.Instance.SaveGame();
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

    private void LoadSaveSlotText()
    {
        for (int i = 1; i <= 3; i++)
        {
            var gameData = DataManager.Instance.getData(i);
            if (gameData == null)
            {
                unlockedLevelTexts[i].text = "[NO DATA]";
                saveDateTexts[i].text = "";
                continue;
            }
            unlockedLevelTexts[i].text = $"[Level: {gameData.unlockedLevel}]";
            saveDateTexts[i].text = $"{(DateTime)gameData.dateTime}";
        }
    }

    public void SetMainObjective(string mainObjective)
    {
        mainObjectiveText.text = "- " + mainObjective;
    }

}
