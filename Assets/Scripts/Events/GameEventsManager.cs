using System;
using UnityEngine;

public class GameEventsManager : MonoBehaviour
{
    public static GameEventsManager instance { get; private set; }

    public InputEvents inputEvents;
    public PlayerEvents playerEvents;
    public FishEvents fishEvents;
    public MiscEvents miscEvents;
    public QuestEvents questEvents;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Game Events Manager in the scene.");
        }
        instance = this;

        // initialize all events
        inputEvents = new InputEvents();
        playerEvents = new PlayerEvents();
        fishEvents = new FishEvents();
        miscEvents = new MiscEvents();
        questEvents = new QuestEvents();
    }
}
