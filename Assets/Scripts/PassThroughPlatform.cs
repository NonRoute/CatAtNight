using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassThroughPlatform : Platform
{
    private bool isPlayerOnPlatform;

    private void Update()
    {
        if(isPlayerOnPlatform && Input.GetAxisRaw("Vertical") < 0 && Input.GetKeyDown(KeyCode.Space))
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
        SetPlayerOnPlatform(collision,true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        SetPlayerOnPlatform(collision,false);
    }

}
