using UnityEngine;
using UnityEngine.Events;

public class DayCycleSystem : MonoBehaviour
{
    public static DayCycleSystem Instance { get; private set; }
    public int days { get; private set; }
    public float times { get; private set; }
    public bool isPaused { get; private set; }
    public bool isTimeCurcular { get; private set; }
    private UnityAction<bool> onPauseEvent;
    private UnityAction<int> onDayChangeEvent;
    private UnityAction<float> onTimeChangeEvent;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void FixedUpdate()
    {
        if (isPaused) return;
        if(isTimeCurcular)
        {
            times += Time.fixedDeltaTime;
            onTimeChangeEvent?.Invoke(times);
        }
    }

    public int GetDays() => days;
    public void NextDays()
    {
        days++;
        times = 0f;
        onDayChangeEvent?.Invoke(days);
    }

    /// <summary> юс╫ц </summary>
    public void SetDays(int day) => days = day;
    public float GetTimes() => times;

    #region Events
    public void SetPause(bool isPause)
    {
        isPaused = isPause;
        onPauseEvent?.Invoke(isPaused);
    }

    public void RigisterPauseEvent(UnityAction<bool> action)
    {
        if (onPauseEvent == null)
            onPauseEvent = new UnityAction<bool>(action);
        else
            onPauseEvent += action;
    }

    public void UnrigisterPauseEvent(UnityAction<bool> action)
    {
        if (onPauseEvent != null)
            onPauseEvent -= action;
    }

    public void RigisterDayChangeEvent(UnityAction<int> action)
    {
        if (onDayChangeEvent == null)
            onDayChangeEvent = new UnityAction<int>(action);
        else
            onDayChangeEvent += action;
    }

    public void UnrigisterDayChangeEvent(UnityAction<int> action)
    {
        if (onDayChangeEvent != null)
            onDayChangeEvent -= action;
    }

    public void RigisterTimeChangeEvent(UnityAction<float> action)
    {
        if (onTimeChangeEvent == null)
            onTimeChangeEvent = new UnityAction<float>(action);
        else
            onTimeChangeEvent += action;
    }

    public void UnrigisterTimeChangeEvent(UnityAction<float> action)
    {
        if (onTimeChangeEvent != null)
            onTimeChangeEvent -= action;
    }
    #endregion
}
