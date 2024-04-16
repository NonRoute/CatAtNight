using UnityEngine;

public class GoToNextZone : MonoBehaviour
{
    [SerializeField] private string nextScene;

    [SerializeField] private Vector3 nextPosition;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform?.parent?.CompareTag("Player") == true)
        {
            DataManager.Instance.tempData.position = nextPosition;
            GameplayStateManager.Instance.ChangeZone(nextScene);
        }
    }
}
