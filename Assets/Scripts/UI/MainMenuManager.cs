using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[System.Serializable]
internal struct PaneData
{
    public Transform pane;
    public bool scrollable;
    public float maxScroll;
}

public class MainMenuManager : MonoBehaviour
{
    public string gameplaySceneName = "Zone1";
    public string bgmName = "TestBGM";
    public string clickSfxName = "TestClick";

    public TMP_Text[] unlockedLevelTexts;
    public TMP_Text[] saveDateTexts;
    public Animator catAnimator;
    public Transform cat;
    public RectTransform grouper;
    public Transform canvas;
    public Transform currentPane;
    public RectTransform currentPaneRect;
    [SerializeField] private PaneData[] paneDatas;
    [SerializeField] private float switchPaneDuration = 1f;
    [SerializeField] private float switchPaneTime = 0;
    [SerializeField] private bool switchPaneFinished = true;
    [SerializeField] private float startX = 0f;
    [SerializeField] private float targetX = -1920f;
    [SerializeField] private bool isScrollable = false;
    [SerializeField] private float maxScroll = 0f;
    [SerializeField] private float currentScroll = 0f;
    [SerializeField] private float targetScroll = 0f;
    [SerializeField] private float scrollSpeed = 20f;
    [SerializeField] private int saveSlot = 0;

    private void Start()
    {
        SoundManager.TryPlayMusic(bgmName);
        LoadSaveSlotText();
    }

    private void Update()
    {
        UpdateSwitchingPane();
        UpdateScrolling();
        CheckKeyInput();
    }

    private void CheckKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (currentPane == null || currentPane == canvas)
            {
                ExitGame();
            }
            else
            {
                GoBack();
            }
        }
    }

    private void UpdateSwitchingPane()
    {
        if (switchPaneFinished) return;
        grouper.anchoredPosition = Vector3.right * (targetX * switchPaneTime + startX * (switchPaneDuration - switchPaneTime)) / switchPaneDuration;
        if (switchPaneTime > switchPaneDuration)
        {
            grouper.anchoredPosition = Vector3.right * targetX;
            cat.SetParent(currentPane);
            catAnimator.SetBool("isWalking", false);
            switchPaneFinished = true;
        }
        switchPaneTime += Time.deltaTime;
    }

    private void UpdateScrolling()
    {
        if (!switchPaneFinished || !isScrollable) return;
        float scroll = Mouse.current.scroll.ReadValue().y;
        targetScroll -= scroll;
        targetScroll = Mathf.Clamp(targetScroll, 0f, maxScroll);
        currentScroll = Mathf.Lerp(currentScroll, targetScroll, scrollSpeed * Time.deltaTime);
        //currentScroll += scrollSpeed * scroll * Time.deltaTime;
        Vector3 pos = currentPaneRect.anchoredPosition;
        pos.y = currentScroll;
        currentPaneRect.anchoredPosition = pos;
    }

    public void SwitchPane(int index)
    {
        for (int i = 0; i < paneDatas.Length; i++)
        {
            if (index == i)
            {
                currentPane = paneDatas[i].pane;
                currentPaneRect = currentPane.gameObject.GetComponent<RectTransform>();
                isScrollable = paneDatas[i].scrollable;
                maxScroll = paneDatas[i].maxScroll;
            }
            else
            {
                paneDatas[i].pane.gameObject.SetActive(false);
            }
        }
        currentScroll = 0f;
        targetScroll = 0f;
        Vector3 pos = currentPaneRect.anchoredPosition;
        pos.y = 0f;
        currentPaneRect.anchoredPosition = pos;
        currentPane.gameObject.SetActive(true);
        catAnimator.SetBool("isWalking", true);
        cat.SetParent(canvas);
        cat.localScale = new Vector3(1, 1, 1);
        startX = grouper.anchoredPosition.x;
        targetX = -1920f;
        switchPaneTime = 0f;
        switchPaneFinished = false;
    }

    public void GoBack()
    {
        currentScroll = 0f;
        targetScroll = 0f;
        Vector3 pos = currentPaneRect.anchoredPosition;
        pos.y = 0f;
        currentPaneRect.anchoredPosition = pos;
        catAnimator.SetBool("isWalking", true);
        currentPane = canvas;
        cat.SetParent(canvas);
        cat.localScale = new Vector3(-1, 1, 1);
        startX = grouper.anchoredPosition.x;
        targetX = 0f;
        switchPaneTime = 0f;
        switchPaneFinished = false;
    }

    public void NewGame()
    {
        DataManager.Instance.resetData();
        GameplayStateManager.Instance.isNewGame = true;
        saveSlot = 0;
        StartGame(false);
    }

    public void SetSaveSlot(int saveSlot)
    {
        this.saveSlot = saveSlot;
    }

    public void StartGame(bool isLoadSave)
    {
        GameplayStateManager.Instance.SetStartMode(isLoadSave, saveSlot);
        if (isLoadSave)
        {
            DataManager.Instance.reloadData();
            string sceneName = DataManager.Instance.gameData.sceneName;
            if (sceneName != "")
            {
                SoundManager.TryPlayNew(clickSfxName);
                SoundManager.TryStop(bgmName);
                SceneManager.LoadScene(sceneName);
            }
        }
        else
        {
            SoundManager.TryPlayNew(clickSfxName);
            SoundManager.TryStop(bgmName);
            SceneManager.LoadScene(gameplaySceneName);
        }
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void LoadSaveSlotText()
    {
        //continueLevelText.text = $"Level {gameData.unlockedLevel}";
        for (int i = 0; i <= 3; i++)
        {
            var gameData = DataManager.Instance.getData(i);
            if (gameData == null)
            {
                unlockedLevelTexts[i].text = "[NO DATA]";
                saveDateTexts[i].text = "";
                continue;
            }
            unlockedLevelTexts[i].text = $"[Level: {gameData.unlockedLevel}]";
            saveDateTexts[i].text = $"{((DateTime)gameData.dateTime).ToLocalTime()}";
        }
    }
}
