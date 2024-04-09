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
    [SerializeField] private GameObject solidImage;
    [SerializeField] private GameObject liquidImage;
    [SerializeField] private GameObject dashIcon;

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
    }

    public void ToggleLiquidImage(bool isLiquid)
    {
        solidImage.SetActive(!isLiquid);
        liquidImage.SetActive(isLiquid);
    }

    public void UpdateHealthBar(float hp, float maxHP)
    {
        var hpPercent = hp / maxHP;
        hpSlider.value = hpPercent;
        hpText.text = $"Health: {Mathf.RoundToInt(hp)} / {Mathf.RoundToInt(maxHP)}";

        if (hpPercent <= (1f / 3))
        {
            hpSlider.fillRect.GetComponent<Image>().color = Color.red;
        }
        else
        {
            hpSlider.fillRect.GetComponent<Image>().color = Color.white;
        }
    }

    public void UpdateStaminaBar(float fraction, bool isStaminaOut)
    {
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

    public void ToggleDashIcon(bool isShow)
    {
        dashIcon.SetActive(isShow);
    }

    public void ToggleHide(bool isHide)
    {
        canvas.enabled = !isHide;
        //gameObject.SetActive(!isHide);
    }

}
