using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTester : MonoBehaviour
{
    [SerializeField] private string parameter;
    [SerializeField] private int parameterInt;

    [ContextMenu("SetMainObjective")]
    private void SetMainObjective()
    {
        GameplayStateManager.Instance.Player.SetMainObjective(parameter);
    }

    [ContextMenu("SetSkill")]
    private void SetPlayerSkill()
    {
        GameplayStateManager.Instance.Player.SetUnlockedSkill(parameterInt);
    }

    [ContextMenu("AddItem")]
    private void AddItemToInventory()
    {
        PlayerInventory.Instance.AddItem(parameter);
    }

    [ContextMenu("RemoveItem")]
    private void RemoveItemToInventory()
    {
        PlayerInventory.Instance.RemoveItem(parameter);
    }


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
