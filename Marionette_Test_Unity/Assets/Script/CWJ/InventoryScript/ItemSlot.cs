using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    //아이템 데이터
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    private Sprite defaultItemSprite;
    public bool isFull;
    public string ItemDescription;
    //아이템 슬롯
    [SerializeField]
    private TMP_Text quantityText;
    [SerializeField]
    private Image SlotImage;
    [SerializeField]
    private Image itemImage;

    //아이템 설명 슬롯
    public Image itemDescriptionImage;
    public TMP_Text ItemDescriptionNameText;
    public TMP_Text ItemDescriptionText;

    public GameObject selectedShader;
    public bool thisItemSelected;

    private InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GameObject.Find("UI").GetComponent<InventoryManager>();
        defaultItemSprite = itemImage.sprite;
    }
    public void AddItem(int id, string itemName, int quantity, Sprite itemSprite, string ItemDescription)
    {
        Debug.Log("ItemSlot AddItem");
        this.itemName = itemName;
        this.quantity = quantity;
        this.itemSprite = itemSprite;
        this.ItemDescription = ItemDescription;
        isFull = true;
        quantityText.text = quantity.ToString();
        SlotImage.enabled = true;
        //quantityText.enabled = true; //아이템 개수 텍스트 보이기
        itemImage.sprite = itemSprite;
    }

    public void EmptyItem()
    {
        Debug.Log("ItemSlot EmptyItem");
        this.itemName = "";
        this.quantity = 0;
        this.itemSprite = defaultItemSprite;
        this.ItemDescription = "";
        isFull = false;
        quantityText.text = quantity.ToString();
        SlotImage.enabled = false;
        //quantityText.enabled = true; //아이템 개수 텍스트 보이기
        itemImage.sprite = defaultItemSprite;
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if(eventData.button == PointerEventData.InputButton.Left)
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
        if (quantity != 0)
        {
            inventoryManager.DeselectAllSlots();
            selectedShader.SetActive(true);
            thisItemSelected = true;
            ItemDescriptionNameText.text = itemName;
            ItemDescriptionText.text = ItemDescription;
            itemDescriptionImage.sprite = itemSprite;
        }
    }
    public void OnRightClick()
    {

    }
    



}
