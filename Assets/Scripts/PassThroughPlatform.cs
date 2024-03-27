using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassThroughPlatform : Platform
{
    [SerializeField] private bool isPlayerOnPlatform;

    private void Update()
    {
        bool downKeyboard1 = Input.GetAxisRaw("Vertical") < 0 && Input.GetKeyDown(KeyCode.Space);
        bool downKeyboard2 = false;// Input.GetKeyDown(KeyCode.LeftControl);
        bool downController = Input.GetKeyDown(KeyCode.JoystickButton1);

        if (isPlayerOnPlatform && (downKeyboard1 || downKeyboard2 || downController))
        {
            col.enabled = false;
            StartCoroutine(EnableCollider());
        }
    }

    private IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(1f);
        col.enabled = true;
    }

    private void SetPlayerOnPlatform(Collider2D other, bool onPlatform)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerOnPlatform = onPlatform;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SetPlayerOnPlatform(other, true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        SetPlayerOnPlatform(other, false);
    }

}
