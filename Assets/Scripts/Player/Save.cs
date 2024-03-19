using UnityEngine;

public partial class Player : MonoBehaviour, IDamagable
{
    public void SaveGame()
    {
        var gameData = DataManager.Instance.gameData;
        gameData.position = playerPosition.position;
        gameData.health = health;
        gameData.liquidMode = isLiquid;

        DataManager.Instance.saveData();
    }

    private void RestoreFromSave()
    {
        var gameData = DataManager.Instance.gameData;

        rb.transform.position = gameData.position;
        UpdatePlayerPosition();

        health = gameData.health;

        if (gameData.liquidMode)
        {
            SwitchMode(true);
        }
    }
}