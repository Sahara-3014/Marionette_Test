using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Newtonsoft.Json.Bson;

public class Debate3ChoiceButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Image image;
    private PrintText debate3GameManager;

    public GameObject Debate3GameManager;
    public GameObject connect;
    public TMP_Text text;
    public GameObject textObject;
    public int BigfontSize, SmallfontSize;
    public bool thischoiceButtonSelected;
    public bool mouseEnter;
    void Start()
    {
        image = this.GetComponent<Image>();
        debate3GameManager = GameObject.Find("Debate3GameManager").GetComponent<PrintText>();
    }

    
    void Update()
    {
        ReSize();
        if (thischoiceButtonSelected)
        {
            connect.SetActive(true);
            Color color = image.color;
            color.a = 1f;
            image.color = color;
            
            text.fontSize = BigfontSize;
        }
        else if (mouseEnter)
        {

            Color color = image.color;
            color.a = 0.8f;
            image.color = color;
           
            text.fontSize = BigfontSize;
        }
        else
        {
            connect.SetActive(false);
            Color color = image.color;
            color.a = 0.8f;
            image.color = color;
            
            text.fontSize = SmallfontSize;
        }
    }

    public void ReSize()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        RectTransform textrectTransform = textObject.GetComponent<RectTransform>();
        
        rectTransform.sizeDelta = textrectTransform.sizeDelta * new Vector2(1.3f, 1.3f);
        textrectTransform.anchoredPosition = rectTransform.sizeDelta / 2;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseEnter = true; 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseEnter = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnRightClick();
        }
    }

    public void OnLeftClick()
    {
        debate3GameManager.DeselectAllChoiceButtons();
        thischoiceButtonSelected = true;
        Debug.Log("thischoiceButtonSelected: " + thischoiceButtonSelected);
    }

    public void OnRightClick()
    {

    }
}
