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

    public event Action onBoss3Dead;
    public void Boss3Dead() 
    {
        if (onBoss3Dead != null) 
        {
            onBoss3Dead();
        }
    }

    public event Action onGemGet;
    public void GemGet()
    {
        if (onGemGet != null)
        {
            onGemGet();
        }
    }
}
