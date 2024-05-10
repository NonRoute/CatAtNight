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
    [SerializeField] private ShopItem stamina = new ShopItem();
    [SerializeField] private ShopItem immortal = new ShopItem();

    private void Start()
    {
        player = GameplayStateManager.Instance.Player;
    }

    void Update()
    {
        health.button.interactable = IsUpgradable(health.currentTier, health.upgradeValue) && IsScoreSufficient(health.upgradeCostType[health.currentTier], health.upgradeCostAmount[health.currentTier]);
        stamina.button.interactable = IsUpgradable(stamina.currentTier, stamina.upgradeValue) && IsScoreSufficient(stamina.upgradeCostType[stamina.currentTier], stamina.upgradeCostAmount[stamina.currentTier]);
        immortal.button.interactable = IsUpgradable(immortal.currentTier, immortal.upgradeValue) && IsScoreSufficient(immortal.upgradeCostType[immortal.currentTier], immortal.upgradeCostAmount[immortal.currentTier]);
    }

    public void refreshCurrentTier()
    {
        Player player = GameplayStateManager.Instance.Player;
        while (health.upgradeValue[health.currentTier] < player.GetPlayerData().maxHealth && health.currentTier <= health.upgradeValue.Count)
        {
            health.currentTier++;
            performUpgrade(health);
        }
        while (stamina.upgradeValue[stamina.currentTier] < player.GetPlayerData().maxStamina && stamina.currentTier <= stamina.upgradeValue.Count)
        {
            stamina.currentTier++;
            performUpgrade(stamina);
        }
        while (immortal.upgradeValue[immortal.currentTier] < player.GetPlayerData().immortalDuration && immortal.currentTier <= immortal.upgradeValue.Count)
        {
            immortal.currentTier++;
            performUpgrade(immortal);
        }
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
                //scoreManager.fishScore -= value;
                PlayerInventory.Instance.fishCount -= value;
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
            ShopItem.CostType.Fish => cost <= PlayerInventory.Instance.fishCount,
            ShopItem.CostType.Monster => cost <= scoreManager.monsterScore,
            ShopItem.CostType.Friendship => cost <= scoreManager.friendshipScore,
            _ => false,
        };
    }
    private bool IsUpgradable(int currentTier, List<float> upgradeValue)
    {
        return currentTier + 1 < upgradeValue.Count;
    }

    public void UpgradeHealth()
    {
        Upgrade(health);
    }
    public void UpgradeStamina()
    {
        Upgrade(stamina);
    }

    public void UpgradeImmortal()
    {
        Upgrade(immortal);
    }

    public void Upgrade(ShopItem item)
    {
        if (player == null)
        {
            player = GameplayStateManager.Instance.Player;
        }
        DecreaseScore(item.upgradeCostType[item.currentTier], item.upgradeCostAmount[item.currentTier]);
        item.currentTier++;
        switch (item.name)
        {
            case "health":
                player.UpgradeHealth(item.upgradeValue[item.currentTier]);
                break;
            case "stamina":
                player.UpgradeStamina(item.upgradeValue[item.currentTier]);
                break;
            case "immortal":
                player.UpgradeImmortal(item.upgradeValue[item.currentTier]);
                break;

        }
        performUpgrade(item);
    }

    public void performUpgrade(ShopItem item)
    {
        item.currentText.text = item.upgradeValue[item.currentTier].ToString();
        if (IsUpgradable(item.currentTier, item.upgradeValue))
        {

            item.nextText.text = item.upgradeValue[item.currentTier + 1].ToString();
            item.costSprite.sprite = GetIcon(item.upgradeCostType[item.currentTier + 1]);
            item.costText.text = "x " + item.upgradeCostAmount[item.currentTier + 1].ToString();
        }
        else
        {
            item.nextText.text = "-";
            item.costSprite.gameObject.SetActive(false);
            item.costText.text = "";
        }
    }
}

[System.Serializable]
public class ShopItem
{
    public enum CostType { Fish, Monster, Friendship };

    public string name;
    public List<float> upgradeValue;
    public List<CostType> upgradeCostType;
    public List<int> upgradeCostAmount;
    public int currentTier = 0;
    public TextMeshProUGUI currentText;
    public TextMeshProUGUI nextText;
    public Image costSprite;
    public TextMeshProUGUI costText;
    public Button button;
}