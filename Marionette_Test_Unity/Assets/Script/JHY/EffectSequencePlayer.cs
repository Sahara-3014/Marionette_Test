using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectSequencePlayer : MonoBehaviour
{
    [Header("실행할 시퀀스 SO 리스트")]
    public List<EffectSequenceSO> sequenceList = new List<EffectSequenceSO>();

    [Header("실행할 시퀀스 인덱스 (0부터)")]
    public int selectedIndex = 0;

    [Header("시퀀스 자동 실행 여부")]
    public bool playOnStart = false;

    void Start()
    {
        if (playOnStart && sequenceList != null && sequenceList.Count > selectedIndex && sequenceList[selectedIndex] != null)
        {
            StartCoroutine(PlaySequence(sequenceList[selectedIndex]));
        }
    }

    public Coroutine PlaySelected()
    {
        if (sequenceList != null && sequenceList.Count > selectedIndex && sequenceList[selectedIndex] != null)
            return StartCoroutine(PlaySequence(sequenceList[selectedIndex]));
        return null;
    }

    private Coroutine currentSequenceCoroutine = null;

    public Coroutine Play(EffectSequenceSO so)
    {
        if (currentSequenceCoroutine != null)
        {
            StopCoroutine(currentSequenceCoroutine);
            currentSequenceCoroutine = null;
        }
        currentSequenceCoroutine = StartCoroutine(PlaySequence(so));
        return currentSequenceCoroutine;
    }

    public void StopSequence(EffectSequenceSO so)
    {
        if (currentSequenceCoroutine != null)
        {
            StopCoroutine(currentSequenceCoroutine);
            currentSequenceCoroutine = null;
        }
    }

    private IEnumerator PlaySequence(EffectSequenceSO so)
    {
        // EffectManager가 완전히 준비될 때까지 대기
        yield return new WaitUntil(() =>
            EffectManager.Instance != null &&
            IsInitialized(EffectManager.Instance));

        foreach (var step in so.steps)
        {
            if (step.directionSet != null)
            {
                EffectManager.Instance.PlayDirectionSet(step.directionSet);
                yield return new WaitForSeconds(step.duration);
                EffectManager.Instance.StopDirectionSet(step.directionSet);
            }
            yield return new WaitForSeconds(step.delayAfter);
        }
    }

    private bool IsInitialized(EffectManager manager)
    {
        var field = manager.GetType().GetField("effectPrefabs", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var dict = field?.GetValue(manager) as System.Collections.Generic.Dictionary<EffectType, GameObject>;
        return dict != null && dict.Count > 0;
    }
}