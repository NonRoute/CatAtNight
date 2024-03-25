using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerStatusPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text staminaDrainRateText;
    [SerializeField] private TMP_Text staminaRegenRateText;
    [SerializeField] private GameObject[] skillBlockPanes;

    public void SetupValue(PlayerData pd)
    {
        healthText.text = "Health: " + pd.health + "/" + pd.maxHealth;
        staminaDrainRateText.text = "Stamina Drain Rate: " + pd.staminaDrainRate + "% / s";
        staminaRegenRateText.text = "Stamina Regen Rate: " + pd.staminaRegenRate + "% / s";
        for (int i = 0; i < skillBlockPanes.Length; i++)
        {
            skillBlockPanes[i].SetActive(pd.skillUnlockedCount <= i);
        }
    }
}
