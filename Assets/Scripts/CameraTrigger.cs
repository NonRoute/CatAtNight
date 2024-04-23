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
            DataManager.Instance.tempData.position = GameplayStateManager.Instance.Player.GetCameraFollow().position;
            GameplayStateManager.Instance.AutoSave();
            while (GameplayStateManager.Instance.isSaving)
            {
                //wait
            }
            StartCoroutine(DestroyDelayed(0.2f));
            DataManager.Instance.DestroyObject(gameObject);
            isTriggered = true;
            SoundManager.TryPlayMusic("Boss1BGM");
        }
    }

    IEnumerator DestroyDelayed(float duration)
    {
        yield return new WaitForSeconds(duration);
        DataManager.Instance.DestroyObject(gameObject);
    }

}
