using UnityEngine;

public partial class Player : MonoBehaviour, IDamagable, ISavable
{

    public void NewGame()
    {
        health = 5;
        maxHealth = 5;
        skillProgression = 0;
        UpdateSkillUI();
        DataManager.Instance.tempData.position = rb.transform.position;
        StatusUIManager.Instance.UpdateHealthBar(health,maxHealth);
        SwitchMode(false);
        PreserveData();
    }
    //public void SaveGame()
    //{
    //    var gameData = DataManager.Instance.gameData;
    //    gameData.position = cameraFollowTransform.position;
    //    gameData.health = health;
    //    gameData.liquidMode = isLiquid;

    //    DataManager.Instance.saveData();
    //}

    //public void RestoreFromSave()
    //{
    //    var gameData = DataManager.Instance.gameData;

    //    rb.transform.position = gameData.position;
    //    UpdateCameraFollowPosition();

    //    health = gameData.health;

    //    if (gameData.liquidMode)
    //    {
    //        SwitchMode(true);
    //    }
    //}

    public void PreserveData()
    {
        var tempData = DataManager.Instance.tempData;
        // wait for new position
        tempData.health = health;
        tempData.maxHealth = maxHealth;
        tempData.liquidMode = isLiquid;
        tempData.skillProgression = skillProgression;
        tempData.mainObjective = mainObjective;
    }

    public void RestoreData()
    {
        print("RESTORE");
        var tempData = DataManager.Instance.tempData;

        rb.transform.position = tempData.position;
        skillProgression = tempData.skillProgression;
        mainObjective = tempData.mainObjective;

        health = tempData.health;
        maxHealth = tempData.maxHealth;
        StatusUIManager.Instance.UpdateHealthBar(health, maxHealth);
        UpdateSkillUI();

        if (tempData.liquidMode)
        {
            SwitchMode(true);
        }
    }
    public void Save()
    {
        var gameData = DataManager.Instance.gameData;
        gameData.position = cameraFollowTransform.position;
        gameData.health = health;
        gameData.maxHealth = maxHealth;
        gameData.liquidMode = isLiquid;
        gameData.skillProgression = skillProgression;
        gameData.mainObjective = mainObjective;
    }

    public void LoadSave()
    {
        var gameData = DataManager.Instance.gameData;

        rb.transform.position = gameData.position;
        UpdateCameraFollowPosition();
        skillProgression = gameData.skillProgression;
        mainObjective = gameData.mainObjective;

        health = gameData.health;
        maxHealth= gameData.maxHealth;
        StatusUIManager.Instance.UpdateHealthBar(health, maxHealth);
        UpdateSkillUI();

        if (gameData.liquidMode)
        {
            SwitchMode(true);
        }
    }
}