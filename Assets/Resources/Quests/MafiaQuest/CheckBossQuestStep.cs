using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckBossQuestStep : QuestStep
{

    private void Start()
    {
        string status = "Check what happened below.";
        ChangeState("", status);
    }

    private void OnEnable()
    {
        GameEventsManager.instance.miscEvents.onBoss3Dead += OnBossDead;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.miscEvents.onBoss3Dead -= OnBossDead;
    }

    private void OnBossDead()
    {
        FinishQuestStep();
    }

    protected override void SetQuestStepState(string state)
    {
        // no state is needed for this quest step
    }
}
