using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private StatusUIManager statusUIManager;
    [SerializeField] private Sprite FishIcon;
    [SerializeField] private Sprite MonsterIcon;
    [SerializeField] private Sprite FriendshipIcon;
    [SerializeField] private Sprite AdventureIcon;
    [SerializeField] private ShopItem health = new ShopItem();

    private Sprite getIcon(ShopItem.CostType costType)
    {
        switch (costType)
        {
            case ShopItem.CostType.Fish: return FishIcon;
            case ShopItem.CostType.Monster: return MonsterIcon;
            case ShopItem.CostType.Friendship: return FriendshipIcon;
            default: return AdventureIcon;
        }
    }
    public void UpgradeHealth()
    {
        // TODO: Check cost
        health.currentTier++;
        player.maxHealth = health.upgradeValue[health.currentTier];
        player.health += health.upgradeValue[health.currentTier] - health.upgradeValue[health.currentTier - 1];
        statusUIManager.UpdateHealthBar(player.health, player.maxHealth);
        health.currentText.text = health.upgradeValue[health.currentTier].ToString();
        if (health.currentTier + 1 < health.upgradeValue.Count)
        {

            health.nextText.text = health.upgradeValue[health.currentTier + 1].ToString();
            health.costSprite.sprite = getIcon(health.upgradeCostType[health.currentTier + 1]);
            health.costText.text = health.upgradeCostAmount[health.currentTier + 1].ToString();
        }
        else
        {
            // Last Tier
            health.nextText.text = "-";
            health.button.interactable = false;
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