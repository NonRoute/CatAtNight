using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatusUIManager : MonoBehaviour
{

    private static StatusUIManager instance;
    public static StatusUIManager Instance => instance;

    [SerializeField] private Canvas canvas;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private Slider hpSlider;
    [SerializeField] private Slider staminaSlider;
    [SerializeField] private Color staminaColor;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        staminaColor = staminaSlider.fillRect.GetComponent<Image>().color;
        canvas = GetComponent<Canvas>();
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
    public void UpdateStaminaBar(float percent, bool isStaminaOut)
    {
        var fraction = percent/100f;
        staminaSlider.value = fraction;

        if (isStaminaOut)
        {
            staminaSlider.fillRect.GetComponent<Image>().color = Color.red;
        }
        else
        {
            staminaSlider.fillRect.GetComponent<Image>().color = staminaColor;
        }
    }

    public void ToggleHide(bool isHide)
    {
        gameObject.SetActive(!isHide);
    }

}
