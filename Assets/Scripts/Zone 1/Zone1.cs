using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zone1 : MonoBehaviour
{
    private string musicName = "Zone 1 Music";

    void Start()
    {
        SoundManager.TryPlayMusic(musicName);
    }
}
