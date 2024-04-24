using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushButton : MonoBehaviour
{
    public PushButtonPuzzle puzzle;
    [SerializeField] private GameObject unpushSprite;
    [SerializeField] private GameObject pushSprite;
    public GameObject door;
    public bool isPushed;
    public bool isUnlocked;

    private void UpdateSprite()
    {
        pushSprite.SetActive(isPushed);
        unpushSprite.SetActive(!isPushed);
        if (isUnlocked) return;
        door.SetActive(!isPushed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Companion"))
        {
            SoundManager.TryPlay("PushButton");
            isPushed = true;
            UpdateSprite();
            if (isUnlocked) return;
            puzzle.OnButtonPushed();
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (isPushed) return;
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Companion"))
        {
            isPushed = true;
            UpdateSprite();
            if (isUnlocked) return;
            puzzle.OnButtonPushed();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Companion"))
        {
            SoundManager.TryPlay("UnpushButton");
            isPushed = false;
            UpdateSprite();
        }
    }
}
