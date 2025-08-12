using UnityEngine;

[System.Serializable]
public class ItemData
{
    public int id;
    public string itemName;
    public Sprite itemSprite;

    [TextArea]
    public string ItemDescription;
}
