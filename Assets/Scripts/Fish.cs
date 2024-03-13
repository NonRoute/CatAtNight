using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField]
    private ScoreManager scoreManager;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (
            collision.gameObject.transform.parent.TryGetComponent(
                out Player player))
        {
            scoreManager.score++;
            player.Heal(1);
            gameObject.SetActive(false);
        }
    }
}
