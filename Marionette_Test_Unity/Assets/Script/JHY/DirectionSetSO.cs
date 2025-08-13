using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct EffectExecutionData
{
    public EffectType effectType;

    [Tooltip("플리커 이펙트의 기본값을 덮어씁니다.")]
    public FlickerParams flickerOverrides;

}



[CreateAssetMenu(fileName = "DS_NewDirectionSet", menuName = "Marionet Test/Direction Set")]
public class DirectionSetSO : ScriptableObject
{


    [Header("이 세트에 포함될 이펙트 목록")]
    public List<EffectType> particlesToPlay;
    public List<PostProcessingEffectType> postProcessingsToEnable;

    [Header("효과 전환 시간")]
    [Range(0f, 5f)]
    public float fadeDuration = 1.0f;

    [Tooltip("체크하면 이펙트가 서서히 나타납니다 (페이드 인)")]
    public bool useFadeIn = true;

    [Tooltip("체크하면 이펙트가 서서히 사라집니다 (페이드 아웃)")]
    public bool useFadeOut = true;
}