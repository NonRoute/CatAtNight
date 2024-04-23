using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;

public class QuestLogUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject contentParent;
    [SerializeField] private QuestLogScrollingList scrollingList;
    [SerializeField] private TextMeshProUGUI questDisplayNameText;
    [SerializeField] private TextMeshProUGUI questStatusText;
    [SerializeField] private TextMeshProUGUI fishRewardsText;
    [SerializeField] private TextMeshProUGUI itemsRewardText;
    //[SerializeField] private TextMeshProUGUI levelRequirementsText;
    //[SerializeField] private TextMeshProUGUI questRequirementsText;

    private Button firstSelectedButton;

    private void OnEnable()
    {
        //GameEventsManager.instance.inputEvents.onQuestLogTogglePressed += QuestLogTogglePressed;
        GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
    }

    private void OnDisable()
    {
        //GameEventsManager.instance.inputEvents.onQuestLogTogglePressed -= QuestLogTogglePressed;
        GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
    }

    //private void QuestLogTogglePressed()
    //{
    //    if (contentParent.activeInHierarchy)
    //    {
    //        HideUI();
    //    }
    //    else
    //    {
    //        ShowUI();
    //    }
    //}

    //private void ShowUI()
    //{
    //    contentParent.SetActive(true);
    //    GameEventsManager.instance.playerEvents.DisablePlayerMovement();
    //    // note - this needs to happen after the content parent is set active,
    //    // or else the onSelectAction won't work as expected
    //    if (firstSelectedButton != null)
    //    {
    //        firstSelectedButton.Select();
    //    }
    //}
    public void OnShowUI()
    {
        // note - this needs to happen after the content parent is set active,
        // or else the onSelectAction won't work as expected
        if (firstSelectedButton != null)
        {
            firstSelectedButton.Select();
        }
    }

    //private void HideUI()
    //{
    //    contentParent.SetActive(false);
    //    GameEventsManager.instance.playerEvents.EnablePlayerMovement();
    //    EventSystem.current.SetSelectedGameObject(null);
    //}

    private void QuestStateChange(Quest quest)
    {
        if (quest.state == QuestState.REQUIREMENTS_NOT_MET || quest.state == QuestState.CAN_START) return;
        // add the button to the scrolling list if not already added
        QuestLogButton questLogButton = scrollingList.CreateButtonIfNotExists(quest, () => {
            SetQuestLogInfo(quest);
        });

        // initialize the first selected button if not already so that it's
        // always the top button
        if (firstSelectedButton == null)
        {
            firstSelectedButton = questLogButton.button;
        }

        // set the button color based on quest state
        questLogButton.SetState(quest.state);
    }

    private void SetQuestLogInfo(Quest quest)
    {
        print("setquestlog");
        // quest name
        questDisplayNameText.text = quest.info.displayName;

        // status
        questStatusText.text = quest.GetFullStatusText();

        //// requirements
        //levelRequirementsText.text = "Level " + quest.info.progressionRequirement;
        //questRequirementsText.text = "";
        //foreach (QuestInfoSO prerequisiteQuestInfo in quest.info.questPrerequisites)
        //{
        //    questRequirementsText.text += prerequisiteQuestInfo.displayName + "\n";
        //}

        // rewards
        fishRewardsText.text = quest.info.fishReward + " Fish";
        string itemText = "";
        foreach(ItemCount itemData in quest.info.itemsReward)
        {
            itemText += itemData.ItemName + " x" + itemData.Amount + ", ";
        }
        itemsRewardText.text = itemText;
    }
}
