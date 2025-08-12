using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(BoxCollider))]
public class InteractiveDebate_ScrollTMPObject : InteractObject
{
    [SerializeField] Color diableColor = Color.gray;
    public bool isSelected { get; protected set; } = false;

    public override void OnValidate()
    {
        base.OnValidate();
        BoxCollider collider = GetComponent<BoxCollider>();
        Vector3 v3 = collider.size;
        v3.x = gameObject.GetComponent<RectTransform>().rect.width;
        v3.y = gameObject.GetComponent<RectTransform>().rect.height;
    }

    public override void OnBtnPress()
    {
        base.OnBtnPress();
        if (isSelected)
            return;
        isSelected = true;
        onBtnPress?.Invoke();
    }
}
