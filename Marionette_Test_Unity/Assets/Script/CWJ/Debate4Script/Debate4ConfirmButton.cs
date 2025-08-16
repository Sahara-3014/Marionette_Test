using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Newtonsoft.Json.Bson;
using Unity.VisualScripting;
public class Debate4ConfirmButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Image image;
    private Debate4GameManager debate4GameManager;
    private bool mouseEnter;
    public bool confirmButtonSelected;

    void Start()
    {
        image = this.GetComponent<Image>();
        debate4GameManager = GameObject.Find("Debate4GameManager").GetComponent<Debate4GameManager>();

    }

    void Update()
    {
        if (confirmButtonSelected)
        {
            //connect.SetActive(true);
            Color color = image.color;
            //color.a = 5f;
            image.color = color;

        }
        else if (mouseEnter)
        {

            Color color = image.color;
            //color.a = 0.2f;
            image.color = color;

        }
        else
        {
            //connect.SetActive(false);
            Color color = image.color;
            //color.a = 0.2f;
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
        //debate4GameManager.DeselectAllChoiceButtons();
        confirmButtonSelected = true;
        Debug.Log("confirmButtonSelected: " + confirmButtonSelected);
    }

    public void OnRightClick()
    {
        
    }
}
