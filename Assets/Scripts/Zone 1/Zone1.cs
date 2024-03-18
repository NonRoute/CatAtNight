using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Zone1 : MonoBehaviour
{
    private string musicName = "Zone 1 Music";
    [SerializeField] private GameObject[] bossRoomYarnBallBoxes;

    void Start()
    {
        SoundManager.TryPlayMusic(musicName);
    }

    public void ChangeYarnBallBoxPosition()
    {
        int newIndex = Random.Range(0, bossRoomYarnBallBoxes.Length);
        for (int i = 0; i < bossRoomYarnBallBoxes.Length; i++)
        {
            if (i == newIndex)
            {
                bossRoomYarnBallBoxes[i].SetActive(true);
            }
            else
            {
                bossRoomYarnBallBoxes[i].SetActive(false);
            }
        }
    }
}
