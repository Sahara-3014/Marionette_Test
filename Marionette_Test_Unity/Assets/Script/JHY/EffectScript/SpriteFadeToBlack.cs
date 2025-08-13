using UnityEngine;
using System.Collections;

public class SpriteFadeToBlack : MonoBehaviour
{
    [Tooltip("색상이 흰색에서 검은색(실루엣)으로 변하는 데 걸리는 시간(초)")]
    public float fadeDuration = 1.0f;

    [SerializeField] private SpriteRenderer spriteRenderer;
    private Coroutine runningFadeCoroutine = null;
    public bool isFade = true;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void FadetoBlack()
    {
        if(isFade)
        {
            if (runningFadeCoroutine != null)
            {
                StopCoroutine(runningFadeCoroutine);
            }
            runningFadeCoroutine = StartCoroutine(FadeToBlack());
        }
    }


    void OnDisable()
    {
        if (runningFadeCoroutine != null)
        {
            StopCoroutine(runningFadeCoroutine);
            runningFadeCoroutine = null;
        }
        spriteRenderer.color = Color.white;
    }
    private IEnumerator FadeToBlack()
    {
        Color startColor = Color.white;
        Color endColor = Color.black;

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