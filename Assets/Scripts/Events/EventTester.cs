using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTester : MonoBehaviour
{
    [ContextMenu("FishCollect")]
    private void TriggerCollectFish()
    {
        GameEventsManager.instance.miscEvents.FishCollected();
    }

    [ContextMenu("Submit")]
    private void TriggerSubmit()
    {
        GameEventsManager.instance.inputEvents.SubmitPressed();
    }
}
