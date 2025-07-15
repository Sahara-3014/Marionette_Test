using UnityEngine;

public class GlichController : MonoBehaviour
{
    [Header("제어할 Renderer Feature의 이름")]
    [Tooltip("URP 렌더러 데이터에 등록된 피처의 이름과 정확히 일치해야 함")]
    public string targetFeatureName = "FullScreen_Glitch";

    [Header("사용할 머티리얼")]
    [Tooltip("이 효과가 활성화될 때 사용할 글리치 머티리얼")]
    public Material glitchMaterial;
    [Tooltip("효과가 끝났을 때 돌아갈 기본 머티리얼")]
    public Material defaultMaterial;

    [Header("효과 강도")]
    [Range(0, 100)] public float glichNoiseAmount;
    [Range(0, 100)] public float glitchStrength;
    [Range(0, 1)] public float scanLinesStrength;

    private static readonly int GlichNoiseAmountID = Shader.PropertyToID("_GlichNoiseAmount");
    private static readonly int GlichStrengthID = Shader.PropertyToID("_GlichStrength");
    private static readonly int ScanlineStrengthID = Shader.PropertyToID("_ScanlineStrength");


    void OnEnable()
    {
        if (RendererFeatureController.Instance != null)
        {
            RendererFeatureController.Instance.SetPassMaterial(targetFeatureName, glitchMaterial);
            RendererFeatureController.Instance.SetFeatureActive(targetFeatureName, true);
        }
    }

    void OnDisable()
    {
        if (RendererFeatureController.Instance != null)
        {
            RendererFeatureController.Instance.SetPassMaterial(targetFeatureName, defaultMaterial);
            RendererFeatureController.Instance.SetFeatureActive(targetFeatureName, false);
        }
    }
    void Update()
    {
        if (glitchMaterial != null)
        {
            glitchMaterial.SetFloat(GlichNoiseAmountID, glichNoiseAmount);
            glitchMaterial.SetFloat(GlichStrengthID, glitchStrength);
            glitchMaterial.SetFloat(ScanlineStrengthID, scanLinesStrength);
        }
    }
}