using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GetGemQuestStep : QuestStep
{

    private void Start()
    {
        string status = "Get the gem at the Far east tower.";
        ChangeState("", status);
    }

    private void OnEnable()
    {
        GameEventsManager.instance.miscEvents.onGemGet += OnGem;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.miscEvents.onGemGet -= OnGem;
    }

    private void OnGem()
    {
        FinishQuestStep();
    }

    protected override void SetQuestStepState(string state)
    {
        // no state is needed for this quest step
    }
}
