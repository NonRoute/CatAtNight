using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ShopManager : MonoBehaviour
{
    private Player player;
    [SerializeField] private StatusUIManager statusUIManager;
    [SerializeField] private ScoreManager scoreManager;
    [SerializeField] private Sprite FishIcon;
    [SerializeField] private Sprite MonsterIcon;
    [SerializeField] private Sprite FriendshipIcon;
    [SerializeField] private ShopItem health = new ShopItem();

    private void Start()
    {
        player = GameplayStateManager.Instance.Player;
    }

    void Update()
    {
        health.button.interactable = IsUpgradable(health.currentTier, health.upgradeValue) && IsScoreSufficient(health.upgradeCostType[health.currentTier], health.upgradeCostAmount[health.currentTier]);
    }

    private Sprite GetIcon(ShopItem.CostType costType)
    {
        return costType switch
        {
            ShopItem.CostType.Fish => FishIcon,
            ShopItem.CostType.Monster => MonsterIcon,
            ShopItem.CostType.Friendship => FriendshipIcon,
        };
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
                break;
        }
    }

    private bool IsScoreSufficient(ShopItem.CostType costType, int cost)
    {
        return costType switch
        {
            ShopItem.CostType.Fish => cost <= scoreManager.fishScore,
            ShopItem.CostType.Monster => cost <= scoreManager.monsterScore,
            ShopItem.CostType.Friendship => cost <= scoreManager.friendshipScore,
            _ => false,
        };
    }
    private bool IsUpgradable(int currentTier, List<int> upgradeValue)
    {
        return currentTier + 1 < upgradeValue.Count;
    }

    public void UpgradeHealth()
    {
        if (player == null)
        {
            player = GameplayStateManager.Instance.Player;
        }
        DecreaseScore(health.upgradeCostType[health.currentTier], health.upgradeCostAmount[health.currentTier]);
        health.currentTier++;
        player.UpgradeHealth(health.upgradeValue[health.currentTier]);
        health.currentText.text = health.upgradeValue[health.currentTier].ToString();
        if (IsUpgradable(health.currentTier, health.upgradeValue))
        {

            health.nextText.text = health.upgradeValue[health.currentTier + 1].ToString();
            health.costSprite.sprite = GetIcon(health.upgradeCostType[health.currentTier + 1]);
            health.costText.text = "x " + health.upgradeCostAmount[health.currentTier + 1].ToString();
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
    public enum CostType { Fish, Monster, Friendship };

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