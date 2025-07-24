using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.ComponentModel;

#region 열거형 및 데이터 구조체
public enum EffectType
{
    None,
    FireAshes,
    HealingSparkle,
    Glich,
    CameraShake,
    FadeToBlack,
    FadeFromBlack,
    FadeOutTOFadeIn

}

public enum PostProcessingEffectType
{
    None,
    FireColorGrade,
    MentalBreakdown,
    Flashback,
    RedFlash
}

[System.Serializable]
public struct NamedEffect
{
    public EffectType effectType;
    public GameObject prefab;
}

[System.Serializable]
public struct NamedVolume
{
    public PostProcessingEffectType effectType;
    public GameObject prefab;
}

#endregion


public class EffectManager : MonoBehaviour
{
    #region 싱글톤 및 초기화
    public static EffectManager Instance { get; private set; }

    [Header("시퀀스 플레이어 등록")]
    [SerializeField] private EffectSequencePlayer sequencePlayer;

    [Header("개별 이펙트 프리팹 등록")]
    public List<NamedEffect> EffectList;
    public List<NamedVolume> volumeList;

    [Header("연출 SO 리스트 등록")]
    public List<DirectionSetSO> directionSetList;

    [Header("시퀀스 SO 리스트 등록")]
    public List<EffectSequenceSO> sequenceList;

    //내부 관리용 변수
    private Dictionary<EffectType, GameObject> effectPrefabs;
    private Dictionary<PostProcessingEffectType, GameObject> volumePrefabs;

    private Dictionary<EffectType, List<GameObject>> activeEffectInstances = new Dictionary<EffectType, List<GameObject>>();
    private Dictionary<PostProcessingEffectType, GameObject> activeVolumeInstances = new Dictionary<PostProcessingEffectType, GameObject>();



    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeEffects();
        if (sequencePlayer == null)
        {
            sequencePlayer = Object.FindFirstObjectByType<EffectSequencePlayer>();
            if (sequencePlayer == null)
            {
                var go = new GameObject("EffectSequencePlayer");
                sequencePlayer = go.AddComponent<EffectSequencePlayer>();
            }
        }
    }

    #endregion


    #region 공개 함수
    public void PlayDirectionSet(DirectionSetSO set)
    {
        if (set == null)
        {
            Debug.LogWarning("[EffectManager] 비어있는 연출 세트를 재생할 수 없습니다.");
            return;
        }
        foreach (var pType in set.particlesToPlay) PlayEffect(pType);
        foreach (var ppType in set.postProcessingsToEnable) EnablePP(ppType, set.fadeDuration);
    }
    public void StopDirectionSet(DirectionSetSO set, bool stopParticlesImmediately = false)
    {
        if (set == null) return;
        foreach (var pType in set.particlesToPlay) StopEffect(pType, stopParticlesImmediately);
        foreach (var ppType in set.postProcessingsToEnable) DisablePP(ppType, set.fadeDuration);
    }
    public Coroutine PlaySequence(EffectSequenceSO so)
    {
        if (so == null)
        {
            Debug.LogWarning("[EffectManager] 비어있는 시퀀스 SO를 재생할 수 없습니다.");
            return null;
        }
        if (sequencePlayer == null)
        {
            sequencePlayer = Object.FindFirstObjectByType<EffectSequencePlayer>();
            if (sequencePlayer == null)
            {
                var go = new GameObject("EffectSequencePlayer");
                sequencePlayer = go.AddComponent<EffectSequencePlayer>();
            }
        }
        return sequencePlayer.Play(so);
    }

    public void PlayDirectionSetByIndex(int index)
    {
        if (directionSetList != null && index >= 0 && index < directionSetList.Count)
        {
            var set = directionSetList[index];
            if (set != null)
            {
                PlayDirectionSet(set);
            }
            else
            {
                Debug.LogWarning($"[EffectManager] {index}번 인덱스의 연출 세트가 null입니다.");
            }
        }
        else
        {
            Debug.LogWarning($"[EffectManager] {index}번 인덱스의 연출 세트가 없습니다.");
        }
    }

    public void PlayEffect(int idx)
    {
        int singleCount = 0;
        int seqCount = 0;

        if (directionSetList != null)
        {
            singleCount = directionSetList.Count;
        }
        if (sequenceList != null)
        {
            seqCount = sequenceList.Count;
        }

        if (idx < 0)
        {
            Debug.LogWarning($"[EffectManager] 인덱스가 0보다 작습니다: {idx}");
            return;
        }
        if (singleCount == 0 && seqCount == 0)
        {
            Debug.LogWarning("[EffectManager] 연출 리스트가 모두 비어 있습니다.");
            return;
        }

        if (idx < singleCount)
        {
            var set = directionSetList[idx];
            if (set != null)
                PlayDirectionSet(set);
            else
                Debug.LogWarning($"[EffectManager] {idx}번 단일 연출 SO가 null입니다.");
        }
        else if (idx - singleCount < seqCount)
        {
            var seq = sequenceList[idx - singleCount];
            if (seq != null)
                PlaySequence(seq);
            else
                Debug.LogWarning($"[EffectManager] {idx}번 시퀀스 SO가 null입니다.");
        }
        else
        {
            Debug.LogWarning($"[EffectManager] {idx}번 인덱스의 연출이 없습니다.");
        }
    }

    public void StopEffect(int idx, bool stopParticlesImmediately = false)
    {
        int singleCount = 0;
        int seqCount = 0;

        if (directionSetList != null)
        {
            singleCount = directionSetList.Count;
        }
        if (sequenceList != null)
        {
            seqCount = sequenceList.Count;
        }

        if (idx < 0)
        {
            Debug.LogWarning($"[EffectManager] 인덱스가 0보다 작습니다: {idx}");
            return;
        }
        if (singleCount == 0 && seqCount == 0)
        {
            Debug.LogWarning("[EffectManager] 연출 리스트가 모두 비어 있습니다.");
            return;
        }

        if (idx < singleCount)
        {
            var set = directionSetList[idx];
            if (set != null)
                StopDirectionSet(set, stopParticlesImmediately);
            else
                Debug.LogWarning($"[EffectManager] {idx}번 단일 연출 SO가 null입니다.");
        }
        else if (idx - singleCount < seqCount)
        {
            var seq = sequenceList[idx - singleCount];
            if (seq != null)
                StopSequence(seq);
            else
                Debug.LogWarning($"[EffectManager] {idx}번 시퀀스 SO가 null입니다.");
        }
        else
        {
            Debug.LogWarning($"[EffectManager] {idx}번 인덱스의 연출이 없습니다.");
        }
    }

    private void StopSequence(EffectSequenceSO so)
    {
        if (so == null) return;
        if (sequencePlayer != null)
            sequencePlayer.StopSequence(so);
        else
            Debug.LogWarning("[EffectManager] 시퀀스 플레이어가 할당되지 않았습니다.");
    }

    #endregion

    #region 함수 구현

    private void InitializeEffects()
    {
        activeEffectInstances.Clear();
        activeVolumeInstances.Clear();

        effectPrefabs = new Dictionary<EffectType, GameObject>();
        foreach (var p in EffectList) { if (p.effectType != EffectType.None) effectPrefabs[p.effectType] = p.prefab; }

        volumePrefabs = new Dictionary<PostProcessingEffectType, GameObject>();
        foreach (var v in volumeList) { if (v.effectType != PostProcessingEffectType.None) volumePrefabs[v.effectType] = v.prefab; }
    }

    private void PlayEffect(EffectType effectType)
    {
        if (effectPrefabs.TryGetValue(effectType, out GameObject prefab))
        {
            GameObject instance = Instantiate(prefab);

            if (!activeEffectInstances.ContainsKey(effectType))
            {
                activeEffectInstances[effectType] = new List<GameObject>();
            }
            activeEffectInstances[effectType].Add(instance);
        }
    }

    private void StopEffect(EffectType effectType, bool stopImmediately)
    {
        if (activeEffectInstances.TryGetValue(effectType, out List<GameObject> instances))
        {
            foreach (var instance in new List<GameObject>(instances))
            {
                if (instance == null) continue;

                if (stopImmediately)
                {
                    Destroy(instance);
                }
                else
                {
                    ParticleSystem[] pss = instance.GetComponentsInChildren<ParticleSystem>();
                    if (pss.Length > 0)
                    {
                        foreach (var ps in pss) ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                    }

                    FadePlayer fadePlayer = instance.GetComponent<FadePlayer>();
                    if (fadePlayer != null)
                    {
                        // fadePlayer.PlayFade(fadeOutDuration, transparentColor);
                    }

                    EffectInfo info = instance.GetComponent<EffectInfo>();
                    float destroyDelay = (info != null) ? info.MaxDuration : 3.0f;

                    Destroy(instance, destroyDelay);
                }
            }

            instances.Clear();
        }
    }

    private void EnablePP(PostProcessingEffectType effectType, float duration)
    {
        if (activeVolumeInstances.ContainsKey(effectType) && activeVolumeInstances[effectType] != null) return;

        if (volumePrefabs.TryGetValue(effectType, out GameObject prefab))
        {
            GameObject instance = Instantiate(prefab);
            Volume volumeComponent = instance.GetComponent<Volume>();
            if (volumeComponent != null)
            {
                volumeComponent.weight = 0f;
                activeVolumeInstances[effectType] = instance;
                StartCoroutine(FadeVolumeWeight(volumeComponent, 1f, duration, false));
            }
        }
    }

    private void DisablePP(PostProcessingEffectType effectType, float duration)
    {
        if (activeVolumeInstances.TryGetValue(effectType, out GameObject instance))
        {
            if (instance == null)
            {
                activeVolumeInstances.Remove(effectType);
                return;
            }

            Volume volumeComponent = instance.GetComponent<Volume>();
            if (volumeComponent != null)
            {
                StartCoroutine(FadeVolumeWeight(volumeComponent, 0f, duration, true));
            }
            activeVolumeInstances.Remove(effectType);
        }
    }

    private IEnumerator FadeVolumeWeight(Volume volume, float targetWeight, float duration, bool destroyOnEnd)
    {
        if (volume == null) yield break;
        float startWeight = volume.weight;
        float time = 0;
        if (duration <= 0.01f) { volume.weight = targetWeight; }
        else
        {
            while (time < duration)
            {
                if (volume == null) yield break;
                volume.weight = Mathf.Lerp(startWeight, targetWeight, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
        }
        if (volume != null) volume.weight = targetWeight;
        if (destroyOnEnd && volume != null) Destroy(volume.gameObject);
    }


    #endregion
}