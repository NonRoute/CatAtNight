using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CompanionUIManager : MonoBehaviour
{

    private static CompanionUIManager instance;
    public static CompanionUIManager Instance => instance;

    [SerializeField] private TMP_Text mainText;
    [SerializeField] private GameObject statusPanel;

    [SerializeField] private Color normalColor;
    [SerializeField] private Color chosenColor;

    [SerializeField] private Image[] choicesBG;
    [SerializeField] private GameObject choice3Box;
    [SerializeField] private TMP_Text choice3Text;

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
        choicesBG[0].color = chosenColor;
        for (int i = 1; i < choicesBG.Length; i++)
        {
            choicesBG[i].color = normalColor;
        }
        HideChoice3();
    }

    public void OpenChoice3(string text)
    {
        choice3Box.SetActive(true);
        choice3Text.text = "Press 3: " + text;
    }

    public void HideChoice3()
    {
        choice3Box.SetActive(false);
    }

    public void SetStatus(int choice)
    {
        for (int i = 0; i < choicesBG.Length; i++)
        {
            choicesBG[i].color = (choice-1 == i) ? chosenColor : normalColor;
        }
    }

    public void SetOpen(bool isOpen)
    {
        if (isOpen)
        {
            mainText.text = "Press C to Say Goodbye";
            //mainText.fontSize = 22;
        }
        else
        {
            mainText.text = "Press C to Call Companion";
            //mainText.fontSize = 30;
        }
        statusPanel.SetActive(isOpen);
    }

    public void SetShow(bool isShow)
    {
        gameObject.SetActive(isShow);
    }
}
