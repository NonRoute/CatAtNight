using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public string gameplaySceneName = "Zone1";
    public string bgmName = "TestBGM";
    public string clickSfxName = "TestClick";

    public void Start()
    {
        SoundManager.TryPlayMusic(bgmName);
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

}
