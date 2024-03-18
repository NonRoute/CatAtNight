using UnityEngine;

public partial class Player : MonoBehaviour, IDamagable
{
    private void saveGame()
    {
        var gameData = DataManager.Instance.gameData;
        gameData.position = playerPosition.position;
        gameData.health = health;
        gameData.liquidMode = isLiquid;

        DataManager.Instance.saveData();
    }

    private void restoreFromSave()
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