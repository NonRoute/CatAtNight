using System.Collections;
using UnityEngine;

public class Fish : MonoBehaviour
{
    [SerializeField] private ScoreManager scoreManager;
    private bool isEnable = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isEnable) return;
        if (
            other.gameObject.CompareTag("Player"))
        {
            GameEventsManager.instance.miscEvents.FishCollected();
            scoreManager.fishScore++;
            other.GetComponentInParent<Player>().Heal(1f);
            gameObject.GetComponent<Animator>().Play("CollectFish");
            isEnable = false;
            StartCoroutine(DestroyFish());
        }
    }

    IEnumerator DestroyFish()
    {
        yield return new WaitForSeconds(0.583f);
        DataManager.Instance.DestroyObject(gameObject);
        gameObject.SetActive(false);
    }
}

