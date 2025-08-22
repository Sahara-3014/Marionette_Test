using UnityEngine;

public class NoiseEffectController_TimeLine : MonoBehaviour
{
    [Header("기본 설정")]
    [Tooltip("URP 렌더러 데이터에 등록된 피처의 이름과 정확히 일치해야 함")]
    public string targetFeatureName = "NoiseEffectFeature";

    [Tooltip("이 효과가 활성화될 때 사용할 노이즈/글리치 머티리얼")]
    public Material noiseMaterial;

    [Header("타임라인 제어용 변수")]
    [Tooltip("타임라인 애니메이션으로 이 값을 0~1 사이로 조절")]
    [Range(0, 1)] public float effectOpacity = 1.0f;
    [Tooltip("타임라인 애니메이션으로 이 값을 조절")]
    [Range(0, 1)] public float noiseIntensity = 0.8f;

    [Header("노이즈 및 글리치 강도 설정")]
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

    // 내부 타이머 변수
    private float colorTimer;
    private float sizeTimer;

    private bool isInitialized = false;

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
        isInitialized = false;
    }

    void Start()
    {
        InitializeEffect();
    }

    void OnDisable()
    {
        if (RendererFeatureController.Instance != null)
        {
            RendererFeatureController.Instance.SetFeatureActive(targetFeatureName, false);
        }
        isInitialized = false; 
    }

    void InitializeEffect()
    {
        if (isInitialized) return;

        if (RendererFeatureController.Instance != null && noiseMaterial != null)
        {
            // 타임라인이 제어하기 전의 초기값을 한번 설정해줌
            noiseMaterial.SetFloat(EffectOpacityID, effectOpacity);
            RendererFeatureController.Instance.SetPassMaterial(targetFeatureName, noiseMaterial);
            RendererFeatureController.Instance.SetFeatureActive(targetFeatureName, true);
            isInitialized = true;
        }
        else
        {
            if (RendererFeatureController.Instance == null)
            {
                Debug.LogError("[NoiseEffectController] RendererFeatureController.Instance가 null입니다. 씬에 해당 컨트롤러가 존재하는지 확인해주세요.", this);
            }
            if (noiseMaterial == null)
            {
                Debug.LogError("[NoiseEffectController] Noise Material이 할당되지 않았습니다.", this);
            }
        }
    }

    void Update()
    {
        if (!isInitialized)
        {
            InitializeEffect();
            return; 
        }

        if (noiseMaterial == null) return;

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