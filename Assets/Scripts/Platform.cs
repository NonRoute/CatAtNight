using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    protected Collider2D col;

    void Start()
    {
        col = GetComponent<Collider2D>();
    }
}
