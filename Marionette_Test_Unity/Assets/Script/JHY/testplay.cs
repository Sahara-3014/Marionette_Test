using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class testplay : MonoBehaviour
{
    public List<DirectionSetSO> directionSetslist;

    IEnumerator Start()
    {
        yield return new WaitUntil(() =>
            EffectManager.Instance != null &&
            IsInitialized(EffectManager.Instance));

        EffectManager.Instance.PlayDirectionSet(directionSetslist[1]);
    }

    private bool IsInitialized(EffectManager manager)
    {
        var field = manager.GetType().GetField("particlePrefabs", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var dict = field?.GetValue(manager) as Dictionary<EffectType, GameObject>;
        return dict != null && dict.Count > 0;
    }
}
