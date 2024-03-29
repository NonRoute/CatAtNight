using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private bool isPassthrough;
    public bool IsPassthrough => isPassthrough;
    private Collider2D col;

    private void Start()
    {
        col = GetComponent<Collider2D>();
    }

    public void TemporaryDisableCollider()
    {
        col.enabled = false;
        StartCoroutine(EnableCollider());
    }

    private IEnumerator EnableCollider()
    {
        yield return new WaitForSeconds(1f);
        col.enabled = true;
    }
}
