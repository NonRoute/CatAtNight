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

    public bool IsStayInScene()
    {
        return false;
    }

    public void Save()
    {
        var gameData = DataManager.Instance.gameData;
        gameData.position = cameraFollowTransform.position;
        gameData.health = health;
        gameData.liquidMode = isLiquid;
    }

    public void LoadSave()
    {
        var gameData = DataManager.Instance.gameData;

        rb.transform.position = gameData.position;
        UpdateCameraFollowPosition();

        health = gameData.health;

        if (gameData.liquidMode)
        {
            SwitchMode(true);
        }
    }
}