using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToNextZone : MonoBehaviour
{
    [SerializeField] private string nextScene;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform?.parent?.CompareTag("Player") == true)
        {
            SceneManager.LoadScene(nextScene);
        }
    }
}
