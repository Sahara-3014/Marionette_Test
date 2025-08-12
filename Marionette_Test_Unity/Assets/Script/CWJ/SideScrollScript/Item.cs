using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private string itemName;

    [SerializeField]
    private int quantity;

    [SerializeField]
    private Sprite sprite;

    [TextArea]
    [SerializeField]
    private string ItemDescription;

    private InventoryManager inventoryManager;
    private InteractionIcon interactionIcon;
    public bool interactionRange;

    void Start()
    {
        inventoryManager = GameObject.Find("UI").GetComponent<InventoryManager>();
        interactionIcon = GameObject.Find("Interaction Icon").GetComponent<InteractionIcon>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Interaction") && interactionRange)
        {
            inventoryManager.AddItem(itemName, quantity, sprite, ItemDescription);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Interaction")
        {
            interactionRange = true;
            interactionIcon.iconPos = this.transform.position;
            interactionIcon.iconEnable = true;
            UnityEngine.Debug.Log("interactionIcon.transform.position = " + this.transform.position);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Interaction")
        {
            interactionRange = false;
            interactionIcon.iconEnable = false;
        }
    }

}
