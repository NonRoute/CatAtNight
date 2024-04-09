using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public abstract class CompanionTrigger : MonoBehaviour
{

    [SerializeField] string actionText = "";
    [SerializeField] string talkText = "Hold this Box";
    [SerializeField] string responseText = "OK";

    abstract public void SetAction();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CompanionUIManager.Instance.OpenChoice3(actionText);
            GameplayStateManager.Instance.Player.SetUpChoice3(talkText,responseText);
            SetAction();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            CompanionUIManager.Instance.HideChoice3();
            GameplayStateManager.Instance.Player.ClearChoice3();
        }
    }
}
