using UnityEngine;

public class TimelineEffectPlayer : MonoBehaviour
{
    public void Play(DirectionSetSO setToPlay)
    {
        if (EffectManager.Instance != null && setToPlay != null)
        {
            EffectManager.Instance.PlayDirectionSetBySO(setToPlay);
        }
    }

    public void Stop(DirectionSetSO setToStop)
    {
        if (EffectManager.Instance != null && setToStop != null)
        {
            EffectManager.Instance.StopDirectionSetBySO(setToStop);
        }
    }
}