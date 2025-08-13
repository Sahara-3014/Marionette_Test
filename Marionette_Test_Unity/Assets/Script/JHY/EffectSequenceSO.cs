using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ESQ_NewEffectSequence", menuName = "Marionet Test/Effect Sequence")]
public class EffectSequenceSO : ScriptableObject
{
    [System.Serializable]
    public class SequenceStep
    {
        [Header("실행할 연출 세트")]
        public DirectionSetSO directionSet;
        [Header("이 단계 연출 지속 시간(초)")]
        public float duration = 1.0f;
        [Header("이 단계 후 추가 대기 시간(초)")]
        public float delayAfter = 0.5f;
    }

    [Header("순차적으로 실행할 연출 단계 리스트")]
    public List<SequenceStep> steps = new List<SequenceStep>();
}