using UnityEngine;
using System.Collections;

public class CameraShakePlayer : MonoBehaviour
{
    [Header("흔들림 대상")]
    [Tooltip("실제로 흔들릴 카메라의 Transform. 비워두면 Main Camera를 자동으로 찾습니다.")]
    public Transform cameraTransform;

    [Header("흔들림 설정")]
    [Tooltip("흔들림이 지속되는 시간(초)입니다.")]
    public float duration = 0.5f;

    [Tooltip("흔들림의 세기(진폭)입니다.")]
    public float magnitude = 0.1f;

    [Tooltip("흔들림의 빠르기(진동수)입니다.")]
    public float frequency = 20.0f;

    [Header("방향별 강도")]
    [Range(0, 1)] public float horizontalFactor = 1.0f;
    [Range(0, 1)] public float verticalFactor = 1.0f;


    private Vector3 originalPosition;

    void OnEnable()
    {
        if (cameraTransform == null)
        {
            var mainCam = Camera.main;
            if (mainCam != null)
            {
                cameraTransform = mainCam.transform;
            }
        }

        if (cameraTransform != null)
        {
            StartCoroutine(ShakeCoroutine());
        }
    }

    private IEnumerator ShakeCoroutine()
    {
        originalPosition = cameraTransform.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // 펄린 노이즈를 사용해 부드럽고 불규칙한 값을 생성,서로 다른 시드 값을 주기 위해 X, Y 좌표에 큰 수를 더 한다.
            float xOffset = (Mathf.PerlinNoise(Time.time * frequency, 100f) * 2 - 1) * magnitude * horizontalFactor;
            float yOffset = (Mathf.PerlinNoise(200f, Time.time * frequency) * 2 - 1) * magnitude * verticalFactor;

            cameraTransform.localPosition = originalPosition + new Vector3(xOffset, yOffset, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cameraTransform.localPosition = originalPosition;
    }
}