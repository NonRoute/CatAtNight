using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    private bool isEnable = true;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isEnable) return;
        if (
            other.gameObject.CompareTag("Player"))
        {
            scoreManager.fishScore++;
            other.GetComponentInParent<Player>().Heal(1f);
            gameObject.SetActive(false);
            isEnable = false;
        }
    }
}
