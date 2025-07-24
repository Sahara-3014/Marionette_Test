using UnityEngine;
using System.Collections.Generic;

public class EffectTester : MonoBehaviour
{
    [Header("테스트할 연출 세트 목록")]
    [Tooltip("여기에 테스트하고 싶은 DirectionSetSO 에셋들을 등록")]
    public List<DirectionSetSO> testableDirectionSets;

    [Header("테스트할 시퀀스 SO 목록")]
    [Tooltip("여기에 테스트하고 싶은 EffectSequenceSO 에셋들을 등록")]
    public List<EffectSequenceSO> testableSequenceSOs;

    [HideInInspector]
    public int selectedSetIndex = 0;
    [HideInInspector]
    public int selectedSequenceIndex = 0;

    [Header("파티클 즉시 중지 여부")]
    public bool stopParticlesImmediately = false;
}