using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneCollisionSettings : MonoBehaviour
{
    public Collider2D[] bodyColliders;
    public Rigidbody2D[] rigidbodies;
    public Vector3[] localPositions;

    private void Start()
    {
        bodyColliders = GetComponentsInChildren<Collider2D>();
        for (int i = 0; i < bodyColliders.Length; i++)
        {
            for (int j = i + 1; j < bodyColliders.Length; j++)
            {
                Physics2D.IgnoreCollision(bodyColliders[i], bodyColliders[j]);
            }
        }
        rigidbodies = GetComponentsInChildren<Rigidbody2D>();
        localPositions = new Vector3[rigidbodies.Length];
        for (int i = 0; i < rigidbodies.Length; i++)
        {
            localPositions[i] = rigidbodies[i].transform.localPosition;
        }
    }

    //private void OnDisable()
    //{
    //    foreach(Rigidbody2D rb in rigidbodies)
    //    {
    //        rb.Sleep();
    //    }
    //}

    //private void OnEnable()
    //{
    //    for (int i = 1; i < localPositions.Length; i++)
    //    {
    //        rigidbodies[i].transform.localPosition = localPositions[i];
    //        rigidbodies[i].transform.rotation = Quaternion.identity;
    //    }
    //    foreach (Rigidbody2D rb in rigidbodies)
    //    {
    //        rb.WakeUp();
    //    }
    //    print("YO");
    //}
}
