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
    private Debate3GameManager debate3GameManager;

    public GameObject Debate3GameManager;
    public GameObject connect;
    public TMP_Text text;
    public bool thischoiceButtonSelected;
    public bool mouseEnter;
    void Start()
    {
        image = this.GetComponent<Image>();
        debate3GameManager = GameObject.Find("Debate3GameManager").GetComponent<Debate3GameManager>();
    }

    
    void Update()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        
        if (thischoiceButtonSelected)
        {
            /*connect.SetActive(true);
            Color color = image.color;
            color.a = 1f;
            image.color = color;*/
            
            rectTransform.sizeDelta = new Vector2(1600, 400);
            text.fontSize = 140;
        }
        else if (mouseEnter)
        {

            /*Color color = image.color;
            color.a = 0.5f;
            image.color = color;*/
            rectTransform.sizeDelta = new Vector2(1600, 400);
            text.fontSize = 140;
        }
        else
        {
            /*connect.SetActive(false);
            Color color = image.color;
            color.a = 0f;
            image.color = color;*/
            rectTransform.sizeDelta = new Vector2(1200, 300);
            text.fontSize = 108;
        }
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
