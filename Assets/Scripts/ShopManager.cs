using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private Canvas shopCanvas;
    [SerializeField] private Player player;
    [SerializeField] private StatusUIManager statusUIManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private Sprite FishIcon;
    [SerializeField] private Sprite MonsterIcon;
    [SerializeField] private Sprite FriendshipIcon;
    [SerializeField] private Sprite AdventureIcon;
    [SerializeField] private ShopItem health = new ShopItem();

    void Update()
    {
        health.button.interactable = IsUpgradable(health.currentTier, health.upgradeValue) && IsScoreSufficient(health.upgradeCostType[health.currentTier], health.upgradeCostAmount[health.currentTier]);
    }

    public void ToggleShopCanvas()
    {
        shopCanvas.GetComponent<Canvas>().enabled = !shopCanvas.GetComponent<Canvas>().enabled;
    }
    private Sprite GetIcon(ShopItem.CostType costType)
    {
        switch (costType)
        {
            case ShopItem.CostType.Fish: return FishIcon;
            case ShopItem.CostType.Monster: return MonsterIcon;
            case ShopItem.CostType.Friendship: return FriendshipIcon;
            default: return AdventureIcon;
        }
    }

    private void DecreaseScore(ShopItem.CostType costType, int value)
    {
        switch (costType)
        {
            case ShopItem.CostType.Fish:
                scoreManager.fishScore -= value;
                break;
            case ShopItem.CostType.Monster:
                scoreManager.monsterScore -= value;
                break;
            case ShopItem.CostType.Friendship:
                scoreManager.friendshipScore -= value;
                break;
            default:
                scoreManager.adventureScore -= value;
                break;
        }
    }

    private bool IsScoreSufficient(ShopItem.CostType costType, int cost)
    {
        switch (costType)
        {
            case ShopItem.CostType.Fish: return cost <= scoreManager.fishScore;
            case ShopItem.CostType.Monster: return cost <= scoreManager.monsterScore;
            case ShopItem.CostType.Friendship: return cost <= scoreManager.friendshipScore;
            case ShopItem.CostType.Adventure: return cost <= scoreManager.adventureScore;
            default: return false;
        }
    }
    private bool IsUpgradable(int currentTier, List<int> upgradeValue)
    {
        return currentTier + 1 < upgradeValue.Count;
    }

    public void UpgradeHealth()
    {
        DecreaseScore(health.upgradeCostType[health.currentTier], health.upgradeCostAmount[health.currentTier]);
        health.currentTier++;
        player.maxHealth = health.upgradeValue[health.currentTier];
        player.health += health.upgradeValue[health.currentTier] - health.upgradeValue[health.currentTier - 1];
        statusUIManager.UpdateHealthBar(player.health, player.maxHealth);
        health.currentText.text = health.upgradeValue[health.currentTier].ToString();
        if (IsUpgradable(health.currentTier, health.upgradeValue))
        {

            health.nextText.text = health.upgradeValue[health.currentTier + 1].ToString();
            health.costSprite.sprite = GetIcon(health.upgradeCostType[health.currentTier + 1]);
            health.costText.text = health.upgradeCostAmount[health.currentTier + 1].ToString();
        }
        else
        {
            health.nextText.text = "-";
            health.costSprite.gameObject.SetActive(false);
            health.costText.text = "";
        }

    }

}

[System.Serializable]
public class ShopItem
{
    public enum CostType { Fish, Monster, Friendship, Adventure };

    public List<int> upgradeValue;
    public List<CostType> upgradeCostType;
    public List<int> upgradeCostAmount;
    public int currentTier = 0;
    public TextMeshProUGUI currentText;
    public TextMeshProUGUI nextText;
    public Image costSprite;
    public TextMeshProUGUI costText;
    public Button button;
}