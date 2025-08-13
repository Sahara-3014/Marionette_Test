using UnityEngine;
using System.Collections;

public class NoiseEffectController : MonoBehaviour, IFadeableEffect
{
    [Header("기본 설정")]
    [Tooltip("URP 렌더러 데이터에 등록된 피처의 이름과 정확히 일치해야 함")]
    public string targetFeatureName = "NoiseEffectFeature";

    [Tooltip("이 효과가 활성화될 때 사용할 노이즈/글리치 머티리얼")]
    public Material noiseMaterial;

    [Header("이펙트 투명도 (0=투명, 1=불투명)")]
    [Tooltip("페이드 인이 끝났을 때 도달할 최종 투명도.")]
    [Range(0, 1)] public float effectOpacity = 1.0f;

    [Header("노이즈 및 글리치 강도 설정")]
    [Range(0, 1)] public float noiseIntensity = 0.8f;
    [Range(0, 100)] public float noiseSpeed = 50f;
    [Tooltip("글리치 강도가 이 범위 안에서 계속 변함")]
    [Range(0, 1)] public float minGlitchIntensity = 0.05f;
    [Range(0, 1)] public float maxGlitchIntensity = 0.2f;
    [Tooltip("강도가 변하는 속도")]
    public float glitchChangeSpeed = 5.0f;

    [Header("글리치 블록 상세 설정")]
    [Tooltip("기본 글리치 블록 크기 (가로, 세로 타일링)")]
    public Vector2 blockSize = new Vector2(50, 200);
    [Tooltip("체크하면 블록 크기가 무작위로 계속 바뀜")]
    public bool useRandomBlockSize = false;
    [Tooltip("무작위 크기의 최소값 (가로, 세로)")]
    public Vector2 minRandomSize = new Vector2(10, 50);
    [Tooltip("무작위 크기의 최대값 (가로, 세로)")]
    public Vector2 maxRandomSize = new Vector2(100, 400);
    [Tooltip("크기가 바뀌는 주기(초). 낮을수록 빠르게 바뀜")]
    public float sizeChangeRate = 0.2f;

    [Space(10)]

    [Tooltip("체크하면 아래 목록의 색상들이 무작위로 계속 바뀜")]
    public bool useRandomBlockColor = false;
    [Tooltip("기본 글리치 블록 색상 (랜덤 사용 안 할 시)")]
    public Color glitchBlockColor = Color.magenta;
    [Tooltip("무작위로 바뀔 색상 목록")]
    public Color[] randomColors = { Color.magenta, Color.green, Color.cyan };
    [Tooltip("색상이 바뀌는 주기(초). 낮을수록 빠르게 바뀜")]
    public float colorChangeRate = 0.1f;

    // 내부 상태 변수
    private bool isFadingIn = false;
    private bool isFadingOut = false;
    private float initialOpacity;
    private float colorTimer;
    private float sizeTimer;

    // 셰이더 프로퍼티 ID
    private static readonly int EffectOpacityID = Shader.PropertyToID("_EffectOpacity");
    private static readonly int NoiseIntensityID = Shader.PropertyToID("_NoiseIntensity");
    private static readonly int NoiseSpeedID = Shader.PropertyToID("_NoiseSpeed");
    private static readonly int GlitchIntensityID = Shader.PropertyToID("_GlitchIntensity");
    private static readonly int GlitchBlockIntensityID = Shader.PropertyToID("_GlitchBlockIntensity");
    private static readonly int GlitchBlockColorID = Shader.PropertyToID("_GlitchBlockColor");
    private static readonly int BlockSizeID = Shader.PropertyToID("_BlockSize");

    void OnEnable()
    {
        initialOpacity = effectOpacity;
        effectOpacity = 0f;

        isFadingIn = false;
        isFadingOut = false;

        if (RendererFeatureController.Instance != null && noiseMaterial != null)
        {
            noiseMaterial.SetFloat(EffectOpacityID, effectOpacity);

            RendererFeatureController.Instance.SetPassMaterial(targetFeatureName, noiseMaterial);
            RendererFeatureController.Instance.SetFeatureActive(targetFeatureName, true);
        }
    }

    void OnDisable()
    {
        if (RendererFeatureController.Instance != null)
        {
            RendererFeatureController.Instance.SetFeatureActive(targetFeatureName, false);
        }
    }

    void Update()
    {
        if (noiseMaterial == null) return;

        if (isFadingIn || isFadingOut)
        {
            noiseMaterial.SetFloat(EffectOpacityID, effectOpacity);
        }
        else
        {
            noiseMaterial.SetFloat(EffectOpacityID, effectOpacity);

            if (effectOpacity > 0)
            {
                noiseMaterial.SetFloat(NoiseIntensityID, noiseIntensity);
                noiseMaterial.SetFloat(NoiseSpeedID, noiseSpeed);

                float dynamicIntensity = Mathf.Lerp(minGlitchIntensity, maxGlitchIntensity, Mathf.PerlinNoise(Time.time * glitchChangeSpeed, 0));
                noiseMaterial.SetFloat(GlitchIntensityID, dynamicIntensity);
                noiseMaterial.SetFloat(GlitchBlockIntensityID, dynamicIntensity);

                if (useRandomBlockSize)
                {
                    if (Time.time > sizeTimer)
                    {
                        float randomX = Random.Range(minRandomSize.x, maxRandomSize.x);
                        float randomY = Random.Range(minRandomSize.y, maxRandomSize.y);
                        noiseMaterial.SetVector(BlockSizeID, new Vector2(randomX, randomY));
                        sizeTimer = Time.time + sizeChangeRate;
                    }
                }
                else
                {
                    noiseMaterial.SetVector(BlockSizeID, blockSize);
                }

                if (useRandomBlockColor)
                {
                    if (Time.time > colorTimer)
                    {
                        if (randomColors != null && randomColors.Length > 0)
                        {
                            Color newColor = randomColors[Random.Range(0, randomColors.Length)];
                            noiseMaterial.SetColor(GlitchBlockColorID, newColor);
                        }
                        colorTimer = Time.time + colorChangeRate;
                    }
                }
                else
                {
                    noiseMaterial.SetColor(GlitchBlockColorID, glitchBlockColor);
                }
            }
        }
    }


    public void StartFadeIn(float duration)
    {
        if (isFadingIn) return;
        isFadingIn = true;
        StartCoroutine(FadeInCoroutine(duration));
    }

    public void StartFadeOut(float duration)
    {
        isFadingIn = false;
        if (isFadingOut) return;
        isFadingOut = true;
        StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeInCoroutine(float duration)
    {
        float time = 0;
        float startOpacity = effectOpacity;

        while (time < duration)
        {
            effectOpacity = Mathf.Lerp(startOpacity, initialOpacity, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        effectOpacity = initialOpacity;
        isFadingIn = false;
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float time = 0;
        float startOpacity = effectOpacity;

        while (time < duration)
        {
            effectOpacity = Mathf.Lerp(startOpacity, 0f, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        effectOpacity = 0f;
        noiseMaterial.SetFloat(EffectOpacityID, 0f);

        Destroy(gameObject);
    }
}