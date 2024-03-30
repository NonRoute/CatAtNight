using UnityEngine;

public class Zone1 : MonoBehaviour
{
    [SerializeField] private GameObject[] bossRoomYarnBallBoxes;

    private void Start()
    {
        SoundManager.TryPlayMusic("Zone 1 Music");
    }

    public void ChangeYarnBallBoxPosition()
    {
        int newIndex = Random.Range(0, bossRoomYarnBallBoxes.Length);
        for (int i = 0; i < bossRoomYarnBallBoxes.Length; i++)
        {
            if (i == newIndex)
            {
                bossRoomYarnBallBoxes[i].SetActive(true);
            }
            else
            {
                bossRoomYarnBallBoxes[i].SetActive(false);
            }
        }
    }
}
