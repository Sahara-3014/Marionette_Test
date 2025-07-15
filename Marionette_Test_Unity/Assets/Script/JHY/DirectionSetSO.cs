using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DS_NewDirectionSet", menuName = "Marionet Test/Direction Set")]

public class DirectionSetSO : ScriptableObject
{
    [Header("이 세트에 포함될 이펙트 목록")]
    public List<EffectType> particlesToPlay;
    public List<PostProcessingEffectType> postProcessingsToEnable;

    [Header("효과 전환 시간")]
    [Range(0f, 5f)]
    public float fadeDuration = 1.0f;
}