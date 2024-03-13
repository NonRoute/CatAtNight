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
    [SerializeField] private Slider staminaSlider;

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
        var hpPercent = hp / maxHP;
        hpSlider.value = hpPercent;
        hpText.text = $"HEALTH {Mathf.RoundToInt(hp)} / {Mathf.RoundToInt(maxHP)}";

        if (hpPercent <= (1f / 3))
        {
            hpSlider.fillRect.GetComponent<Image>().color = Color.red;
        }
        else
        {
            hpSlider.fillRect.GetComponent<Image>().color = Color.white;
        }
    }
    public void UpdateStaminaBar(float percent)
    {
        var fraction = percent/100f;
        staminaSlider.value = fraction;
    }

}
