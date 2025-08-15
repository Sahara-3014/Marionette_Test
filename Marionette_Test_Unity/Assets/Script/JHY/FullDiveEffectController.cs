using UnityEngine;
using System.Collections;

public class FullDiveEffectController : MonoBehaviour
{
    [Header("제어 대상")]
    [Tooltip("제어할 파티클 시스템.")]
    public ParticleSystem controlledParticleSystem;
    [Header("연출 설정")]
    [Tooltip("연출이 진행되는 총 시간")]
    public float effectDuration = 3.0f;
    [Header("파티클 양 조절")]
    [Tooltip("시작 시의 파티클 방출량")]
    public float startEmissionRate = 500f;
    [Tooltip("연출이 끝날 때의 파티클 최대 방출량")]
    public float maxEmissionRate = 2000f;
    [Header("카메라 줌 조절")]
    [Tooltip("연출 시작 시의 카메라 Orthographic Size")]
    public float startCameraSize = 5f;
    [Tooltip("연출이 끝날 때의 카메라 목표 Orthographic Size (확대)")]
    public float endCameraSize = 1f;

    // 내부 변수
    private Camera mainCamera;
    private float initialCameraSize;
    private ParticleSystem.EmissionModule emissionModule;

    void Awake()
    {
        if (controlledParticleSystem == null)
        {
            controlledParticleSystem = GetComponentInChildren<ParticleSystem>();
        }
        mainCamera = Camera.main;

        if (controlledParticleSystem != null)
        {
            emissionModule = controlledParticleSystem.emission;
        }
        if (mainCamera != null)
        {
            initialCameraSize = mainCamera.orthographicSize;
        }
    }

    public void Start()
    {
        StopAllCoroutines(); 
        StartCoroutine(EffectCoroutine());
    }

    private IEnumerator EffectCoroutine()
    {
        if (controlledParticleSystem == null || mainCamera == null)
        {
            Debug.LogError("파티클 시스템 또는 메인 카메라가 없습니다!");
            yield break;
        }

        float elapsedTime = 0f;
        controlledParticleSystem.Play();

        while (elapsedTime < effectDuration)
        {
            float progress = elapsedTime / effectDuration; 

            // 진행도에 따라 파티클 방출량과 카메라 사이즈를 선형 보간(Lerp)
            emissionModule.rateOverTime = Mathf.Lerp(startEmissionRate, maxEmissionRate, progress);
            mainCamera.orthographicSize = Mathf.Lerp(startCameraSize, endCameraSize, progress);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 연출 종료 처리
        emissionModule.rateOverTime = maxEmissionRate;
        mainCamera.orthographicSize = endCameraSize;

        StartCoroutine(CleanUpEffect());
    }

    private IEnumerator CleanUpEffect()
    {
        emissionModule.rateOverTime = 0;

        yield return new WaitForSeconds(controlledParticleSystem.main.startLifetime.constantMax);

        mainCamera.orthographicSize = initialCameraSize;

        Destroy(gameObject);
    }
}