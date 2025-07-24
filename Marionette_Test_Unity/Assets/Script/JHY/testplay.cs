using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class testplay : MonoBehaviour
{
    public int effectIdx = 0;
    private bool isPlay = false;
    IEnumerator Start()
    {
        yield return new WaitUntil(() =>
            EffectManager.Instance != null &&
            IsInitialized(EffectManager.Instance));

        //EffectManager.Instance.PlayDirectionSet(directionSetslist[1]);
        EffectManager.Instance.PlayEffect(effectIdx);
    }

    private void Update()
    {
        if (isPlay) return;
        if(!isPlay)
        {
            EffectManager.Instance.PlayEffect(effectIdx);
            isPlay = true;
        }
    }

    private bool IsInitialized(EffectManager manager)
    {
        var field = manager.GetType().GetField("particlePrefabs", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var dict = field?.GetValue(manager) as Dictionary<EffectType, GameObject>;
        return dict != null && dict.Count > 0;
    }
}
