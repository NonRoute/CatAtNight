using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatusUIManager : MonoBehaviour
{

    private static StatusUIManager instance;
    public static StatusUIManager Instance => instance;

    [SerializeField] private TMP_Text hpText;
    [SerializeField] private Slider hpSlider;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void UpdateHealthBar(float hp, float maxHP)
    {
        hpSlider.value = hp / maxHP;
        hpText.text = Mathf.RoundToInt(hp).ToString();
    }
}
