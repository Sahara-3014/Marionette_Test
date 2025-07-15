using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

#region 열거형 및 데이터 구조체
public enum EffectType 
{ None, 
  FireAshes, 
  HealingSparkle, 
  Glich,
  CameraShake,
  FadeToBlack,
  FadeFromBlack
}

public enum PostProcessingEffectType 
{ None, 
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
    #region 싱글톤 및 에셋 등록
    public static EffectManager Instance { get; private set; }
    void Awake() 
    { 
        if (Instance == null) 
        { Instance = this; 
            DontDestroyOnLoad(gameObject); 
        }
        else 
        { 
            Destroy(gameObject); 
        } 
    }

    [Header("개별 이펙트 프리팹 등록")]
    public List<NamedEffect> EffectList;
    public List<NamedVolume> volumeList;

    [Header("연출 SO 리스트 등록")]
    public List<DirectionSetSO> directionSetList;
    #endregion

    #region 내부 관리용 변수
    private Dictionary<EffectType, GameObject> particlePrefabs;
    private Dictionary<PostProcessingEffectType, GameObject> volumePrefabs;

    private Dictionary<EffectType, List<GameObject>> activeParticleInstances = new Dictionary<EffectType, List<GameObject>>();
    private Dictionary<PostProcessingEffectType, GameObject> activeVolumeInstances = new Dictionary<PostProcessingEffectType, GameObject>();
    #endregion

    void Start() 
    { 
        InitializeEffects(); 
    }

    #region 공개 함수

    public void PlayDirectionSet(DirectionSetSO set)
    {
        if (set == null) 
        { Debug.LogWarning("[EffectManager] 비어있는(null) 연출 세트를 재생할 수 없습니다."); 
          return; 
        }
        foreach (var pType in set.particlesToPlay) PlayParticle(pType);
        foreach (var ppType in set.postProcessingsToEnable) EnablePP(ppType, set.fadeDuration);
    }

    public void StopDirectionSet(DirectionSetSO set, bool stopParticlesImmediately = false)
    {
        if (set == null) return;
        foreach (var pType in set.particlesToPlay) StopParticle(pType, stopParticlesImmediately);
        foreach (var ppType in set.postProcessingsToEnable) DisablePP(ppType, set.fadeDuration);
    }


    
    #endregion

    #region 함수 구현

    private void InitializeEffects()
    {
        particlePrefabs = new Dictionary<EffectType, GameObject>();
        foreach (var p in EffectList) 
        {
            if (p.effectType != EffectType.None)
            {
                particlePrefabs[p.effectType] = p.prefab;
            }
        }

        volumePrefabs = new Dictionary<PostProcessingEffectType, GameObject>();
        foreach (var v in volumeList) 
        { 
            if (v.effectType != PostProcessingEffectType.None)
            {
                volumePrefabs[v.effectType] = v.prefab;
            }
        }
    }

    private void PlayParticle(EffectType effectType)
    {
        if (particlePrefabs.TryGetValue(effectType, out GameObject prefab))
        {
            GameObject instance = Instantiate(prefab);
            if (!activeParticleInstances.ContainsKey(effectType))
                activeParticleInstances[effectType] = new List<GameObject>();
            activeParticleInstances[effectType].Add(instance);
        }
    }

    private void StopParticle(EffectType effectType, bool stopImmediately)
    {
        if (activeParticleInstances.TryGetValue(effectType, out List<GameObject> instances))
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
                    instance.GetComponentsInChildren<ParticleSystem>().ToList().ForEach(ps => ps.Stop(true, ParticleSystemStopBehavior.StopEmitting));
                    EffectInfo info = instance.GetComponent<EffectInfo>();
                    Destroy(instance, info != null ? info.maxDuration : 5.0f);
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