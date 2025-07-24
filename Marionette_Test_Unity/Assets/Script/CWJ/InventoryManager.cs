using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    public GameObject InventoryIcon;
    private bool menuActivated;
    public ItemSlot[] itemSlot;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Inventory") && menuActivated)
        {
            Time.timeScale = 1;
            InventoryIcon.SetActive(true);
            InventoryMenu.SetActive(false);
            menuActivated = false;
            Debug.Log("Inventory false");
        }
        else if (Input.GetButtonDown("Inventory") && !menuActivated)
        {
            Time.timeScale = 0;
            InventoryIcon.SetActive(false);
            InventoryMenu.SetActive(true);
            menuActivated = true;
            Debug.Log("Inventory true");
        }
    }

    public void AddItem(string itemName, int quantity, Sprite itemSprite, string ItemDescription)
    {
        Debug.Log("InventoryManager AddItem");
        for (int i = 0; i< itemSlot.Length; i++)
        {
            if (itemSlot[i].isFull == false)
            {
                itemSlot[i].AddItem(itemName, quantity, itemSprite, ItemDescription);
                return;
            }
        }
        Debug.Log("itemName: " + itemName + " quantity: " + quantity + " itemSprite: " + itemSprite);
    }
    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i].selectedShader.SetActive(false);
            itemSlot[i].thisItemSelected = false;
        }

    }
}
