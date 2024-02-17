using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassThroughPlatform : MonoBehaviour
{
    private Collider2D col;
    private bool isPlayerOnPlatform;

    void Start()
    {
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if(isPlayerOnPlatform && Input.GetAxis("Vertical") < 0)
        {
            col.enabled = false;
            StartCoroutine(EnableCollider());
        }
    }

    private IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(0.5f);
        col.enabled = true;
    }

    private void SetPlayerOnPlatform(Collision2D other, bool onPlatform)
    {
        Player player = other.gameObject.GetComponent<Player>();
        if (player != null)
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
