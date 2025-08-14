using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Data.Common;
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    public GameObject InventoryMenu;
    public GameObject InventoryIcon;
    private bool menuActivated;
    public ItemSlot[] itemSlot;

    [SerializeField] private List<ItemData> itemDatabase;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && menuActivated)
        {
            Time.timeScale = 1;
            InventoryIcon.SetActive(true);
            InventoryMenu.SetActive(false);
            menuActivated = false;
            Debug.Log("Inventory false");
        }
        else if (Input.GetKeyDown(KeyCode.Tab) && !menuActivated)
        {
            Time.timeScale = 0;
            InventoryIcon.SetActive(false);
            InventoryMenu.SetActive(true);
            menuActivated = true;
            Debug.Log("Inventory true");
        }
    }

    public void AddItem(int id, int quantity)
    {
        Debug.Log("InventoryManager AddItem");
        for (int i = 0; i< itemSlot.Length; i++)
        {
            if (itemSlot[i].isFull == false)
            {
                foreach(var findingitem in itemDatabase)
                {
                    if(findingitem.id == id)
                    {
                        itemSlot[i].AddItem(findingitem.id, findingitem.itemName, quantity, findingitem.itemSprite, findingitem.ItemDescription);
                        Debug.Log("id: " + findingitem.id + "itemName: " + findingitem.itemName + " quantity: " + quantity + " itemSprite: " + findingitem.itemSprite);
                        return;
                    }
                        
                    
                    
                }

            }
        }
        
    }
    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i].selectedShader.SetActive(false);
            itemSlot[i].thisItemSelected = false;
        }

    }

    public ItemData GetItemData(int id)
    {
        
        foreach (var findingitem in itemDatabase)
        {
            if (findingitem.id == id)
            {
                return findingitem;
            }
        }

        //없으면
        Debug.Log("GetItemData -> Id 못 찾음 return Null");
        return null;
    }
}
