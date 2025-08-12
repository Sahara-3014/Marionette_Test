using UnityEngine;

public class NavOptionManager : MonoBehaviour
{
    public NavOptionButton[] navOptionButton;
    public int Defaultnav;
    private void Start()
    {
        navOptionButton[Defaultnav].thisnavOptionButtonSelected = true;
        for (int i = 0; i < navOptionButton.Length; i++)
        {
            if(i != Defaultnav)
            {
                navOptionButton[i].thisnavOptionButtonSelected = false;
            }
        }
    }
    void Update()
    {
        Debug.Log("NavOptionManager-Update");

    }
    public void DeselectAllNavOptions()
    {
        Debug.Log("NavOptionManager-DeselectAllNavOptions");
        for (int i = 0; i < navOptionButton.Length; i++)
        {
            navOptionButton[i].thisnavOptionButtonSelected = false;
            Debug.Log("i: "+i);
        }
    }
}
