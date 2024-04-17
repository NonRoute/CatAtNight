using System;

public class FishEvents
{
    public event Action<int> onFishGained;
    public void FishGained(int amount) 
    {
        if (onFishGained != null) 
        {
            onFishGained(amount);
        }
    }

    public event Action<int> onGoldChange;
    public void GoldChange(int gold) 
    {
        if (onGoldChange != null) 
        {
            onGoldChange(gold);
        }
    }
}
