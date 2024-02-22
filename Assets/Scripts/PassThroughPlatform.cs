using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassThroughPlatform : Platform
{
    private bool isPlayerOnPlatform;

    private void Update()
    {
        bool downKeyboard1 = Input.GetAxisRaw("Vertical") < 0 && Input.GetKeyDown(KeyCode.Space);
        bool downKeyboard2 = Input.GetKeyDown(KeyCode.LeftShift);
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

    private void SetPlayerOnPlatform(Collision2D other, bool onPlatform)
    {
        if (other.gameObject.TryGetComponent<Player>(out var player))
        {
            isPlayerOnPlatform = onPlatform;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        SetPlayerOnPlatform(collision, true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        SetPlayerOnPlatform(collision, false);
    }

}
