using System;

public class FishEvents
{
    public event Action<int> onGoldGained;
    public void FishGained(int gold) 
    {
        if (onGoldGained != null) 
        {
            onGoldGained(gold);
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
