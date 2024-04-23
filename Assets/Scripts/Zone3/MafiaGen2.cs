using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MafiaGen2 : MonoBehaviour
{
    public BoxCollider2D mafia2_col;
    public GameObject mafia2_sprite;
    public GameObject mafia2_questIcon;

    private void OnDisable()
    {
        if(mafia2_col != null)
        {
            mafia2_col.enabled = true;
            mafia2_sprite.SetActive(true);
            mafia2_questIcon.SetActive(true);
        }
    }
}
