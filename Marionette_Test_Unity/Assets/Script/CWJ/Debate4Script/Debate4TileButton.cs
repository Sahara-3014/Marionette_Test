using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Newtonsoft.Json.Bson;

public class Debate4TileButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Image image;
    private Debate4GameManager debate4GameManager;
    public bool mouseEnter;
    public bool thistileButtonSelected;

    void Start()
    {
        image = this.GetComponent<Image>();
        debate4GameManager = GameObject.Find("Debate4GameManager").GetComponent<Debate4GameManager>();

    }
 
    void Update()
    {
        if (thistileButtonSelected)
        {
            //connect.SetActive(true);
            Color color = image.color;
            color.a = 5f;
            image.color = color;
      
        }
        else if (mouseEnter)
        {

            Color color = image.color;
            color.a = 0.2f;
            image.color = color;

        }
        else
        {
            //connect.SetActive(false);
            Color color = image.color;
            color.a = 0.2f;
            image.color = color;

        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("Enter");
        mouseEnter = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Exit");
        mouseEnter = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click");
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
        //debate4GameManager.DeselectAllChoiceButtons();
        thistileButtonSelected = true;
        Debug.Log("thischoiceButtonSelected: " + thistileButtonSelected);
    }

    public void OnRightClick()
    {
        thistileButtonSelected = false;
        Debug.Log("thischoiceButtonSelected: " + thistileButtonSelected);
    }
}
