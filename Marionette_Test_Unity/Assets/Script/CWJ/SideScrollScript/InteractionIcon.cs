using UnityEngine;
using UnityEngine.UI;

public class InteractionIcon : MonoBehaviour
{
    public Vector3 iconPos;
    public GameObject iconImage;
    public bool iconEnable;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (iconEnable)
        {
            this.iconImage.SetActive(true);
            this.transform.position = iconPos + new Vector3(0,5,0);
        }
        else
        {
            this.iconImage.SetActive(false);
        }
    }
}
