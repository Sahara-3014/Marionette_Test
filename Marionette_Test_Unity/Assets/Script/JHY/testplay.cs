using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class testplay : MonoBehaviour
{
    public int effectIdx = 0;
    [SerializeField]private bool isPlay = false;
    [SerializeField] private bool isEnable = false;
    public bool isTest = false;

    public EffectManager effectManager;
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
        if (!isPlay)
        {
            if (!isTest) return;
            EffectManager.Instance.PlayEffect(effectIdx);
            isPlay = true;
        }
    }

    private void OnEnable()
    {
        isEnable = true;
        if(isEnable)
        {
            EffectManager.Instance.StopEffect(effectIdx);
        }
    }

    private bool IsInitialized(EffectManager manager)
    {
        var field = manager.GetType().GetField("particlePrefabs", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var dict = field?.GetValue(manager) as Dictionary<EffectType, GameObject>;
        return dict != null && dict.Count > 0;
    }
}
