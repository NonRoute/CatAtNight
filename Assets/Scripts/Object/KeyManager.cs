using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyManager : MonoBehaviour, ISavable
{
    private Transform player;

    public bool isPickedUp;
    private Vector2 vel;
    public float smoothTime;

    void Update()
    {
        if (isPickedUp)
        {
            if(player == null)
            {
                player = GameplayStateManager.Instance.Player.transform;
                return;
            }
            Vector3 offset = new Vector3(0, 1.6f, 0);
            transform.position = Vector2.SmoothDamp(transform.position, player.position + offset, ref vel, smoothTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !isPickedUp)
        {
            player = GameplayStateManager.Instance.Player.transform;
            SoundManager.TryPlayNew("GetKey");
            isPickedUp = true;
        }
    }

    public void PreserveData()
    {
        DataManager.Instance.tempData.isKeyPickedUp = isPickedUp;
    }

    public void RestoreData()
    {
        isPickedUp = DataManager.Instance.tempData.isKeyPickedUp;
    }

    public void Save()
    {
        DataManager.Instance.gameData.isKeyPickedUp = isPickedUp;
    }

    public void LoadSave()
    {
        isPickedUp = DataManager.Instance.gameData.isKeyPickedUp;
    }
}
