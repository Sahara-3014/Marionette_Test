using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Newtonsoft.Json.Bson;
public class NavOptionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Image image;
    private NavOptionManager navOptionManager;

    public GameObject NavOptionMenu;
    public bool thisnavOptionButtonSelected;
    public bool mouseEnter;
    private void Start()
    {
        image = this.GetComponent<Image>();
        navOptionManager = GameObject.Find("UI").GetComponent<NavOptionManager>();
    }
    void Update()
    {
        if (thisnavOptionButtonSelected)
        {
            NavOptionMenu.SetActive(true);
            Color color = image.color;
            color.a = 1f;
            image.color = color;
        }
        else if(mouseEnter)
        {
            Color color = image.color;
            color.a = 0.5f;
            image.color = color;
        }
        else
        {
            NavOptionMenu.SetActive(false);
            Color color = image.color;
            color.a = 0f;
            image.color = color;
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
        Debug.Log("thisnavOptionButtonSelected1: " + thisnavOptionButtonSelected);
        navOptionManager.DeselectAllNavOptions();
        thisnavOptionButtonSelected = true;

    }

    public void OnRightClick()
    {

    }


}
