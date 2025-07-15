using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class RendererFeatureController : MonoBehaviour
{
    public static RendererFeatureController Instance { get; private set; }

    [Header("URP 렌더러 데이터")]
    [Tooltip("프로젝트의 Renderer Data 에셋 연결")]
    public ScriptableRendererData rendererData;

    private Dictionary<string, ScriptableRendererFeature> featureDict = new Dictionary<string, ScriptableRendererFeature>();

    void Awake()
    {
        if (Instance != null) 
        { 
            Destroy(gameObject); 
            return; 
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeFeatures();
    }

    void Start()
    {
        InitializeFeatures();
    }

    private void InitializeFeatures()
    {
        if (rendererData == null)
        {
            Debug.LogError("[RendererFeatureController] 렌더러 데이터가 인스펙터에 연결되지 않았습니다.");
            return;
        }

        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature != null)
            {
                featureDict[feature.name] = feature;
                feature.SetActive(false);
            }
        }
    }

    public void SetFeatureActive(string featureName, bool active)
    {
        if (featureDict.TryGetValue(featureName, out ScriptableRendererFeature feature))
        {
            feature.SetActive(active);
        }
        else
        {
            Debug.LogWarning($"[RendererFeatureController] '{featureName}' 이름의 Renderer Feature를 찾을 수 없습니다.");
        }
    }

    public void SetPassMaterial(string featureName, Material material)
    {
        if (featureDict.TryGetValue(featureName, out ScriptableRendererFeature feature))
        {
            if (feature is FullScreenPassRendererFeature fsFeature)
            {
                fsFeature.passMaterial = material;
            }
        }
    }
}