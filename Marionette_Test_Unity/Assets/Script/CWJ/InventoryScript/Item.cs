using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;


public class Item : MonoBehaviour
{
    
    [SerializeField]
    private int id;

    [SerializeField]
    private int quantity;

    private InventoryManager inventoryManager;
    private InteractionIcon interactionIcon;
    public bool interactionRange;
    public int dialog_id;

    void Start()
    {
        inventoryManager = InventoryManager.Instance;
        interactionIcon = GameObject.FindAnyObjectByType<InteractionIcon>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && interactionRange)
        {
            inventoryManager.AddItem(id, quantity);
            if(dialog_id != 0)
            {
                Investigate_DialogManager.instance.SetDialogs(dialog_id, isPlaying: true);
            }
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
