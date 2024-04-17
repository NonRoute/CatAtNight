using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorUnlock : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Key"))
        {
            //Destroy(other.gameObject);
            DataManager.Instance.DestroyObject(other.gameObject);
            other.gameObject.SetActive(false);
            DataManager.Instance.DestroyObject(gameObject);
            gameObject.SetActive(false);
        }
    }

    public void Unlock() { Destroy(gameObject); }
}
