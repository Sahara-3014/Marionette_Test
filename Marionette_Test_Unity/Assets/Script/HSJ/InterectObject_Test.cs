using UnityEngine;

public class InterectObject_Test : MonoBehaviour
{
    InterectObject interect;
    [SerializeField] Sprite[] cursor;

    private void Start()
    {
        interect = GetComponent<InterectObject>();
        interect.Rigister_CursorHoverEvent(() => CursorChange(true));
        interect.Rigister_CursorExitEvent(() => CursorChange(false));

        interect.Rigister_BtnPressEvent(OnBtnPress);
        interect.Rigister_BtnUpEvent(() => Debug.Log("Button Released"));
        Cursor.SetCursor(cursor[0].texture, Vector2.zero, CursorMode.Auto);
    }

    //private void Update()
    //{
    //    Vector3 v3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    v3.z -= 100f;
    //    Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), v3, Camera.main.transform.rotation);
    //}

    private void OnDisable()
    {
        interect.UnRigister_CursorHoverEvent(() => CursorChange(true));
        interect.UnRigister_CursorExitEvent(() => CursorChange(false));
    }


    public void CursorChange(bool isHover)
    {
        Sprite sprite = isHover ? cursor[1] : cursor[0];
        Cursor.SetCursor(sprite.texture, Vector2.zero, CursorMode.Auto);
    }

    public void OnBtnPress()
    {
        Debug.Log("Button Pressed");
    }
}
