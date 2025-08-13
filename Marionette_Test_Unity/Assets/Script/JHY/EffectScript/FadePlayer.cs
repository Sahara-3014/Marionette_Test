// FadePlayer.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadePlayer : MonoBehaviour
{
    [Header("페이드 설정")]
    public float duration = 1.0f;
    [Tooltip("페이드의 최종 목표 색상")]
    public Color targetColor = Color.black;
    [Tooltip("체크하면 투명하게(Fade In),해제하면 투명한 상태에서 이 색상으로(Fade Out)")]
    public bool isFadeIn = false;
    [Header("페이드 아웃-인 사이 대기 시간(초)")]
    public float waitTime = 0.5f;

    [Header("시작 시 페이드 아웃-인 자동 실행 여부")]
    public bool autoFadeOutIn = false;

    private Image fadeImage;

    void Start()
    {
        fadeImage = GetComponentInChildren<Image>();
        if (autoFadeOutIn)
            PlayFadeOutIn();
        else
            StartCoroutine(FadeCoroutine());
    }

    // 단일 페이드 인/아웃
    private IEnumerator FadeCoroutine()
    {
        Color startColor;
        Color endColor;

        if (isFadeIn)
        {
            startColor = new Color(targetColor.r, targetColor.g, targetColor.b, 1f);
            endColor = new Color(targetColor.r, targetColor.g, targetColor.b, 0f);
        }
        else 
        {
            startColor = new Color(targetColor.r, targetColor.g, targetColor.b, 0f);
            endColor = new Color(targetColor.r, targetColor.g, targetColor.b, 1f);
        }
        fadeImage.color = startColor;
        float time = 0;

        while (time < duration)
        {
            fadeImage.color = Color.Lerp(startColor, endColor, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        fadeImage.color = endColor;
    }

    // 페이드 아웃-대기-페이드 인 연출
    public void PlayFadeOutIn()
    {
        if (fadeImage == null)
            fadeImage = GetComponentInChildren<Image>();
        StartCoroutine(FadeOutInCoroutine());
    }

    private IEnumerator FadeOutInCoroutine()
    {
        // 페이드 아웃
        Color outStart = new Color(targetColor.r, targetColor.g, targetColor.b, 0f);
        Color outEnd = new Color(targetColor.r, targetColor.g, targetColor.b, 1f);
        float t = 0;
        fadeImage.color = outStart;
        while (t < duration)
        {
            fadeImage.color = Color.Lerp(outStart, outEnd, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        fadeImage.color = outEnd;
        // 대기
        if (waitTime > 0f)
            yield return new WaitForSeconds(waitTime);

        // 페이드 인
        Color inStart = new Color(targetColor.r, targetColor.g, targetColor.b, 1f);
        Color inEnd = new Color(targetColor.r, targetColor.g, targetColor.b, 0f);
        t = 0;
        while (t < duration)
        {
            fadeImage.color = Color.Lerp(inStart, inEnd, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        fadeImage.color = inEnd;
    }
}