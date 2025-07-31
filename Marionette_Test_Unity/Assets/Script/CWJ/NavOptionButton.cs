using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class NavOptionButton : MonoBehaviour, IPointerClickHandler
{
    private NavOptionManager navOptionManager;

    public GameObject NavOptionMenu;
    public bool thisnavOptionButtonSelected;
    private void Start()
    {
        navOptionManager = GameObject.Find("UI").GetComponent<NavOptionManager>();
    }
    void Update()
    {
        if (thisnavOptionButtonSelected)
        {
            NavOptionMenu.SetActive(true);
        }
        else
        {
            NavOptionMenu.SetActive(false);
        }
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

        Debug.Log("thisnavOptionButtonSelected2: " + thisnavOptionButtonSelected);
    }

    public void OnRightClick()
    {

    }


}
