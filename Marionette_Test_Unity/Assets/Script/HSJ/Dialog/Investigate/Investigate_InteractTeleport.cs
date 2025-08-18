using Cinemachine;
using UnityEngine;


public class Investigate_InteractTeleport : MonoBehaviour
{

    private InteractionIcon interactionIcon;
    private bool interactionRange;
    public Collider2D camCol;
    public Transform toPos;

    void Start()
    {
        interactionIcon = GameObject.FindAnyObjectByType<InteractionIcon>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && interactionRange)
        {
            Teleport();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Interaction")
        {
            interactionRange = true;
            interactionIcon.iconPos = this.transform.position;
            interactionIcon.iconEnable = true;
            Debug.Log("interactionIcon.transform.position = " + this.transform.position);
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

    void Teleport()
    {
        var cinemachine = FindAnyObjectByType<CinemachineConfiner2D>();
        if (cinemachine != null)
        {
            Transform trf = FindAnyObjectByType<CharacterController>().transform;
            trf.position = toPos.position;
            cinemachine.m_BoundingShape2D = camCol;
        }
    }

}
