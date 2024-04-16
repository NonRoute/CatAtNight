using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollectFishQuestStep : QuestStep
{
    private int fishCollected = 0;
    private int fishToComplete = 5;

    private void Start()
    {
        UpdateState();
    }

    private void OnEnable()
    {
        GameEventsManager.instance.miscEvents.onFishCollected += FishCollected;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.miscEvents.onFishCollected -= FishCollected;
    }

    private void FishCollected()
    {
        if (fishCollected < fishToComplete)
        {
            fishCollected++;
            UpdateState();
        }

        if (fishCollected >= fishToComplete)
        {
            FinishQuestStep();
        }
    }

    private void UpdateState()
    {
        string state = fishCollected.ToString();
        string status = "Collected " + fishCollected + " / " + fishToComplete + " fish.";
        ChangeState(state, status);
    }

    protected override void SetQuestStepState(string state)
    {
        this.fishCollected = System.Int32.Parse(state);
        UpdateState();
    }
}
