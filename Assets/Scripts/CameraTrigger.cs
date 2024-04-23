using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrigger : MonoBehaviour
{

    private bool isTriggered = false;
    [SerializeField] private Transform targetGroup;
    [SerializeField] private CinemachineVirtualCamera cam;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTriggered)
        {
            cam.Follow = targetGroup;
            //while(GameplayStateManager.Instance.isSaving)
            //{
            //    //wait
            //}
            DataManager.Instance.DestroyObject(gameObject);
            isTriggered = true;
            SoundManager.TryPlayMusic("Boss1BGM");


            DataManager.Instance.tempData.position = GameplayStateManager.Instance.Player.GetCameraFollow().position;
            GameplayStateManager.Instance.AutoSave();
        }
    }

}
