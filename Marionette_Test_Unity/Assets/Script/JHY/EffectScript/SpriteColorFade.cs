using UnityEngine;
using System.Collections;

public class SpriteColorFade : MonoBehaviour
{
    [Tooltip("색상이 검은색에서 흰색으로 변하는 데 걸리는 시간(초)")]
    public float fadeDuration = 1.0f;

    [SerializeField]private SpriteRenderer spriteRenderer;
    private Coroutine runningFadeCoroutine = null;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        if (runningFadeCoroutine != null)
        {
            StopCoroutine(runningFadeCoroutine);
        }

        runningFadeCoroutine = StartCoroutine(FadeFromBlackToWhite());
    }

    private IEnumerator FadeFromBlackToWhite()
    {
        Color startColor = Color.black;
        Color endColor = Color.white;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            Color currentColor = Color.Lerp(startColor, endColor, elapsedTime / fadeDuration);
            spriteRenderer.color = currentColor;
            elapsedTime += Time.deltaTime;
            yield return null; 
        }
        spriteRenderer.color = endColor;

        runningFadeCoroutine = null;
    }
}