using UnityEngine;

public class NavOptionManager : MonoBehaviour
{
    public NavOptionButton[] navOptionButton;

    void Update()
    {
        Debug.Log("NavOptionManager-Update");
    }
    public void DeselectAllNavOptions()
    {
        Debug.Log("NavOptionManager-DeselectAllNavOptions");
        for (int i = 0; i < navOptionButton.Length; i++)
        {
            //navOption[i].selectedShader.SetActive(false);
            navOptionButton[i].thisnavOptionButtonSelected = false;
            Debug.Log("i: "+i);
        }
    }
}
