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

    private Image fadeImage;

    void Start()
    {
        fadeImage = GetComponentInChildren<Image>();
        StartCoroutine(FadeCoroutine());
    }

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
}