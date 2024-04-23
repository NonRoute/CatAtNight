using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushButtonPuzzle : MonoBehaviour
{

    [SerializeField] private PushButton red;
    [SerializeField] private PushButton green;

    private void Start()
    {
        red.puzzle = this;
        green.puzzle = this;
    }

    private void OnDisable()
    {
        red.isUnlocked = true;
        green.isUnlocked = true;
    }

    public void OnButtonPushed()
    {
        if(red.isPushed && green.isPushed)
        {
            red.isUnlocked = true;
            green.isUnlocked = true;
            DataManager.Instance.DestroyObject(red.door);
            red.door.SetActive(false);
            DataManager.Instance.DestroyObject(green.door);
            green.door.SetActive(false);
            SoundManager.TryPlay("Unlock");
            DataManager.Instance.DestroyObject(gameObject);
            gameObject.SetActive(false);
        }
    }
}
