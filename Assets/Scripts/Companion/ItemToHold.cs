using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemToHold : MonoBehaviour
{
    private static ItemToHold instance;
    public static ItemToHold Instance => instance;

    public bool isWaiting;
    public Transform destination;
    public float lerpRate = 0.5f;
    public bool isMoving;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Update()
    {
        if (!isMoving)
        {
            return;
        }
        transform.position = Vector3.Lerp(transform.position, destination.position, lerpRate);
        if(Vector3.Distance(transform.position,destination.position) < 0.1f)
        {
            transform.position = destination.position;
            isMoving = false;
        }
    }

    public void WaitForCompanion()
    {
        isWaiting = true;
    }

    [ContextMenu("Move")]
    public void Move()
    {
        isWaiting = false;
        isMoving = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(!isWaiting)
        {
            return;
        }
        if (other.gameObject.CompareTag("Companion"))
        {
            isWaiting = false;
            SoundManager.TryPlayNew("AttackBox");
            Move();
            DataManager.Instance.DestroyObject(gameObject);
        }
    }
}
