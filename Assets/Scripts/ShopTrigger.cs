using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopTrigger : MonoBehaviour
{
    [SerializeField] private Canvas shopCanvas;
    [SerializeField] private GameObject shopObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            shopCanvas.GetComponent<Canvas>().enabled = true;
            shopObject.GetComponent<ShopManager>().refreshCurrentTier();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            shopCanvas.GetComponent<Canvas>().enabled = false;
        }
    }

}