using System;

public class PlayerEvents
{
    public event Action onDisablePlayerMovement;
    public void DisablePlayerMovement()
    {
        if (onDisablePlayerMovement != null) 
        {
            onDisablePlayerMovement();
        }
    }

    public event Action onEnablePlayerMovement;
    public void EnablePlayerMovement()
    {
        if (onEnablePlayerMovement != null) 
        {
            onEnablePlayerMovement();
        }
    }

    public event Action<ItemCount[]> onItemsGained;
    public void ItemsGained(ItemCount[] items) 
    {
        if (onItemsGained != null) 
        {
            onItemsGained(items);
        }
    }

    public event Action<int> onPlayerProgressionChange;
    public void PlayerProgressionChange(int progression) 
    {
        if (onPlayerProgressionChange != null) 
        {
            onPlayerProgressionChange(progression);
        }
    }

    public event Action<int> onPlayerSkillProgressionChange;
    public void PlayerSkillProgressionChange(int skillUnlocked) 
    {
        if (onPlayerSkillProgressionChange != null) 
        {
            onPlayerSkillProgressionChange(skillUnlocked);
        }
    }
}
