using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField]
    private ScoreManager scoreManager;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.transform.parent.GetComponent<Player>();
        if (player != null)
        {
            scoreManager.score++;
            gameObject.SetActive(false);
        }
    }
}
