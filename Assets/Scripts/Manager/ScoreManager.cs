using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public int fishScore = 0;
    public TMP_Text fishScoreText;
    public int monsterScore = 0;
    public TMP_Text monsterScoreText;
    public int friendshipScore = 0;
    public TMP_Text friendshipScoreText;

    void Update()
    {
        fishScore = PlayerInventory.Instance.fishCount;
        fishScoreText.SetText(fishScore.ToString());
        monsterScoreText.SetText(monsterScore.ToString());
        friendshipScoreText.SetText(friendshipScore.ToString());
    }
}
