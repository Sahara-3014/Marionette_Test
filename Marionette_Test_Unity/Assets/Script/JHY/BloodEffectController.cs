using UnityEngine;
using System.Collections;

// 이펙트 시스템과 완벽하게 호환되도록 IFadeableEffect를 구현합니다.
public class BloodEffectController : MonoBehaviour, IFadeableEffect
{
    [Header("기본 설정")]
    public string targetFeatureName = "BloodEffectFeature"; 
    public Material bloodMaterial; 

    // 내부 상태 변수
    private bool isFading = false;

    // 셰이더 프로퍼티 ID
    private static readonly int BloodAmountID = Shader.PropertyToID("_BloodAmount");

    void OnEnable()
    {
        if (bloodMaterial != null)
        {
            bloodMaterial.SetFloat(BloodAmountID, 0f);
        }

        if (RendererFeatureController.Instance != null && bloodMaterial != null)
        {
            RendererFeatureController.Instance.SetPassMaterial(targetFeatureName, bloodMaterial);
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

    public void StartFadeIn(float duration)
    {
        if (isFading) return;
        StartCoroutine(AnimateBloodAmount(0f, 1f, duration));
    }

    public void StartFadeOut(float duration)
    {
        if (isFading) return;
        StartCoroutine(AnimateBloodAmount(1f, 0f, duration, true));
    }



    private IEnumerator AnimateBloodAmount(float start, float end, float duration, bool destroyOnEnd = false)
    {
        isFading = true;
        float time = 0;

        while (time < duration)
        {
            float amount = Mathf.Lerp(start, end, time / duration);
            if (bloodMaterial != null)
            {
                bloodMaterial.SetFloat(BloodAmountID, amount);
            }
            time += Time.deltaTime;
            yield return null;
        }

        if (bloodMaterial != null)
        {
            bloodMaterial.SetFloat(BloodAmountID, end);
        }

        if (destroyOnEnd)
        {
            Destroy(gameObject);
        }
        isFading = false;
    }
}