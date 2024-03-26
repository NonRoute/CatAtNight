using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpControlPanel : MonoBehaviour
{
    [SerializeField] private GameObject[] skillBlockPanes;
    public void SetupValue(int skillUnlockedCount)
    {
        for (int i = 0; i < skillBlockPanes.Length; i++)
        {
            if (i == 2 || i == 5) continue;
            skillBlockPanes[i].SetActive(skillUnlockedCount <= i);
        }
    }
}
