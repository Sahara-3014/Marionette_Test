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
    public bool isAcquisition = true;
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
            if(Investigate_DialogManager.instance != null && Investigate_DialogManager.instance.IsRunning() == false)
            {
                if(dialog_id != 0)
                    Investigate_DialogManager.instance.SetDialogs(dialog_id, isPlaying: true);
                if (isAcquisition)
                {
                    inventoryManager.AddItem(id, quantity);
                    Destroy(gameObject);
                }
            }
            
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
