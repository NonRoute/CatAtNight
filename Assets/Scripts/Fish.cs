using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField]
    private ScoreManager scoreManager;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (
            other.gameObject.CompareTag("Player"))
        {
            scoreManager.score++;
            other.GetComponentInParent<Player>().Heal(1);
            gameObject.SetActive(false);
        }
    }
}
