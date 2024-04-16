using UnityEngine;

public partial class Player : MonoBehaviour, IDamagable, ISavable
{
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
    }

    public void RestoreData()
    {
        print("RESTORE");
        var tempData = DataManager.Instance.tempData;

        rb.transform.position = tempData.position;
        skillProgression = tempData.skillProgression;

        health = tempData.health;
        maxHealth = tempData.maxHealth;
        StatusUIManager.Instance.UpdateHealthBar(health, maxHealth);

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
    }

    public void LoadSave()
    {
        var gameData = DataManager.Instance.gameData;

        rb.transform.position = gameData.position;
        UpdateCameraFollowPosition();
        skillProgression = gameData.skillProgression;

        health = gameData.health;
        maxHealth= gameData.maxHealth;

        if (gameData.liquidMode)
        {
            SwitchMode(true);
        }
    }
}