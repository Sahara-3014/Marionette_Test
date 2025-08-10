using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InteractiveDebate_ChoiceBtn : MonoBehaviour
{
    Image img;
    public Sprite hoverSprite;
    public Sprite defaultSprite;
    public Color pressColor;
    public Color disableColor;
    public UnityAction onPressAction;
    bool isEnabled = true;
    public bool IsEnabled
    {
        get => isEnabled;
        set
        {
            isEnabled = value;
            img.color = isEnabled ? Color.white : disableColor;
        }
    }

    private void OnEnable()
    {
        img = GetComponent<Image>();
        OnChoiceBtnExit();
    }

    public void OnChoiceBtnHover()
    {
        if (!isEnabled)
            return;
        Debug.Log(gameObject.name + " Hovered");
        img.sprite = hoverSprite;
    }

    public void OnChoiceBtnExit()
    {
        if (!isEnabled)
            return;
        Debug.Log(gameObject.name + " Exited");
        img.sprite = defaultSprite;
        img.color = Color.white;
    }

    public void OnChoiceBtnPress()
    {
        if(!isEnabled)
            return;
        Debug.Log(gameObject.name + " Pressed");
        img.color = pressColor;
        onPressAction?.Invoke();
    }

    public void OnChoiceBtnUp()
    {
        if (!isEnabled)
            return;
        Debug.Log(gameObject.name + " Released");
        img.color = Color.white;
    }
}
