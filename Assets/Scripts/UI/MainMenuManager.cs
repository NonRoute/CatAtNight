using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string gameplaySceneName = "Zone1";
    public string bgmName = "TestBGM";
    public string clickSfxName = "TestClick";

    public TMP_Text continueLevelText;

    public void Start()
    {
        SoundManager.TryPlayMusic(bgmName);
        LoadContinueText();
    }

    public void StartGame()
    {
        SoundManager.TryPlayNew(clickSfxName);
        SoundManager.TryStop(bgmName);
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    private void LoadContinueText()
    {
        var gameData = DataManager.Instance.gameData;

        continueLevelText.text = $"Level {gameData.unlockedLevel}";
    }
}
