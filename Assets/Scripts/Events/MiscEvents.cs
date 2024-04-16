using System;

public class MiscEvents
{
    public event Action onFishCollected;
    public void FishCollected() 
    {
        if (onFishCollected != null) 
        {
            onFishCollected();
        }
    }

    public event Action onGemCollected;
    public void GemCollected() 
    {
        if (onGemCollected != null) 
        {
            onGemCollected();
        }
    }
}
