using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class RaycastInput : MonoBehaviour
{
    [SerializeField] private bool isDebug = false; // 디버그 모드 여부
    [field:SerializeField] List<RaycastHit> hits = new(); // RaycastHit 배열
    private Vector3 mousePosition;
    InterectObject mousePressObject = null;
    InterectObject mouseHoverObject = null;

    // Update is called once per frame
    private void Update()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return; // UI 위에 마우스가 있을 경우 Raycast 무시

        var _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        var _hits = Physics.RaycastAll(_ray.origin, _ray.direction, 1000);

        //var _v3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //var _dir = (v3 - Camera.main.transform.position).normalized;
        //var _hits = Physics.RaycastAll(Camera.main.transform.position, dir, 1000f);
        if (isDebug)
        {
            //Vector3 _dir = _v3 - Camera.main.transform.position;
            //Debug.DrawRay(Camera.main.transform.position, dir, Color.red);
            Debug.DrawRay(Camera.main.transform.position, (Camera.main.ScreenToWorldPoint(Input.mousePosition) - Camera.main.transform.position).normalized, Color.red);

            Debug.DrawRay(_ray.origin, _ray.direction * 1000, Color.red);
        }


        if (isDebug && Input.GetMouseButtonDown(0))
            Debug.Log($"Click! : {_hits.Length} / {hits.Count}");
        if (_hits == null || _hits.Length == 0)
            return;

        // Mouse Hover & Exit Event
        if (Input.mousePosition != mousePosition)
        {
            for(int i=0;i<_hits.Length; i++)
            {
                if (hits.Contains(_hits[i]) == false)
                {
                    if (_hits[i].collider.gameObject.GetComponent<InterectObject>() == null)
                        continue;
                    if(mouseHoverObject != null && mouseHoverObject != _hits[i].collider.gameObject.GetComponent<InterectObject>())
                        mouseHoverObject.OnCursorExit();
                    mouseHoverObject = _hits[i].collider.gameObject.GetComponent<InterectObject>();
                    mouseHoverObject.OnCursorHover();
                    break;
                }
            }

            for(int i=0;i<hits.Count;i++)
            {
                if (_hits.Contains(hits[i]) == false)
                {
                    if (hits[i].collider.gameObject.GetComponent<InterectObject>() == null)
                        continue;
                    hits[i].collider.gameObject.GetComponent<InterectObject>()?.OnCursorExit();
                    if(mouseHoverObject == hits[i].collider.gameObject.GetComponent<InterectObject>())
                        mouseHoverObject = null;
                    break;
                }
            }
        }

        mousePosition = Input.mousePosition;
        hits = _hits.ToList();

        // Mouse Click Event
        if (Input.GetMouseButtonDown(0))
            OnClik();

        // Mouse Up Event
        if (mousePressObject != null && Input.GetMouseButtonUp(0))
        {
            mousePressObject.OnBtnUp();
            mousePressObject = null;
        }
    }

    public void OnClik(int index = 1)
    {
        int _index = 0;
        for (int i = 0; i < hits.Count; i++)
        {
            if (_index >= index)
                break;
            if (hits[i].collider.gameObject.GetComponent<InterectObject>() != null)
            {
                var obj = hits[i].collider.gameObject.GetComponent<InterectObject>();
                obj.OnBtnPress();
                mousePressObject = obj;
                _index += 1;
            }
        }
    }

    public RaycastHit[] GetHits(LayerMask layerMask = default)
    {
        List<RaycastHit> hit = new List<RaycastHit>();
        foreach(RaycastHit hitInfo in hits)
        {
            if (hitInfo.collider.gameObject.layer == layerMask)
                hit.Add(hitInfo);
            
        }

        return hit.ToArray();
    }
}
