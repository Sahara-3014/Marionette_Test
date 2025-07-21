using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(EventTrigger))]
public class InterectObject : MonoBehaviour
{
    [Tooltip("해당 오브젝트를 눌렀을때 작동하는 이벤트")]
    [SerializeField] protected UnityEvent onBtnPress;
    [Tooltip("해당 오브젝트를 뗏을때 작동하는 이벤트")]
    [SerializeField] protected UnityEvent onBtnUp;
    [Tooltip("해당 오브젝트에 커서가 위치했을때 작동하는 이벤트")]
    [SerializeField] protected UnityEvent onCursorHover;
    [Tooltip("해당 오브젝트에 커서가 밖으로 나갔을때 작동하는 이벤트")]
    [SerializeField] protected UnityEvent onCursorExit;
    [Tooltip("해당 오브젝트를 눌렀을때 이펙트 전용 이벤트")]
    [SerializeField] protected UnityEvent playBtnPressEffect;

    private void OnValidate()
    {
        if (onBtnPress == null)
        {
            onBtnPress = new();
            onBtnPress.AddListener(OnBtnPress);
        }
        if (onBtnUp == null)
        {
            onBtnUp = new();
            onBtnUp.AddListener(OnBtnUp);
        }
        if (onCursorHover == null)
        {
            onCursorHover = new();
            onCursorHover.AddListener(OnCursorHover);
        }
        if (onCursorExit == null)
        {
            onCursorExit = new();
            onCursorExit.AddListener(OnCursorExit);
        }
        if (playBtnPressEffect == null)
            playBtnPressEffect = new();
        

        EventTrigger trigger = GetComponent<EventTrigger>() ??
            gameObject.AddComponent<EventTrigger>();

        if (trigger.triggers == null || trigger.triggers.Count == 0)
        {
            trigger.triggers = new List<EventTrigger.Entry>();
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerDown,
                callback = new EventTrigger.TriggerEvent()
            };
            entry.callback.AddListener(PressEvent);
            trigger.triggers.Add(entry);
        }
    }

    #region Button Press Event
    private void PressEvent(BaseEventData data) => OnBtnPress();
    public virtual void OnBtnPress()
    {
        onBtnPress?.Invoke();
        playBtnPressEffect?.Invoke();
    }
    public virtual void Rigister_BtnPressEvent(UnityAction action) => onBtnPress.AddListener(action);
    public virtual void UnRigister_BtnPressEvent(UnityAction action) => onBtnPress.RemoveListener(action);

    public virtual void Rigister_PlayBtnPressEffect(UnityAction action) => playBtnPressEffect.AddListener(action);
    public virtual void UnRigister_PlayBtnPressEffect(UnityAction action) => playBtnPressEffect.RemoveListener(action);

    #endregion

    #region Button Up Event
    public virtual void OnBtnUp() => onBtnUp?.Invoke();
    public virtual void Rigister_BtnUpEvent(UnityAction action) => onBtnUp.AddListener(action);
    public virtual void UnRigister_BtnUpEvent(UnityAction action) => onBtnUp.RemoveListener(action);

    #endregion

    #region Cursor Hover Event
    public virtual void OnCursorHover() => onCursorHover?.Invoke();
    public virtual void Rigister_CursorHoverEvent(UnityAction action) => onCursorHover.AddListener(action);
    public virtual void UnRigister_CursorHoverEvent(UnityAction action) => onCursorHover.RemoveListener(action);

    #endregion

    #region Cursor Exit Event
    public virtual void OnCursorExit() => onCursorExit?.Invoke();
    public virtual void Rigister_CursorExitEvent(UnityAction action) => onCursorExit.AddListener(action);
    public virtual void UnRigister_CursorExitEvent(UnityAction action) => onCursorExit.RemoveListener(action);

    #endregion

}
