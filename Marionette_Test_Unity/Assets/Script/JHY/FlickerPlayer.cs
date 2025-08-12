using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlickerPlayer : MonoBehaviour
{
    [Header("깜빡임 설정")]
    [Tooltip("깜빡임이 지속되는 총 시간")]
    public float duration = 1.0f;
    [Tooltip("초당 깜빡이는 횟수")]
    public float frequency = 10.0f;
    [Tooltip("깜빡이는 색상의 알파(투명도)")]
    [Range(0, 1)]
    public float alpha = 1.0f;

    [Tooltip("첫 번째 색상")]
    public Color colorA = Color.red;
    [Tooltip("두 번째 색상")]
    public Color colorB = Color.black;

    [SerializeField] private Image flickerImage;
    private Color finalColorA;
    private Color finalColorB;

    void Start()
    {
        finalColorA = new Color(colorA.r, colorA.g, colorA.b, alpha);
        finalColorB = new Color(colorB.r, colorB.g, colorB.b, alpha);
        StartCoroutine(FlickerCoroutine());
    }

    private IEnumerator FlickerCoroutine()
    {
        float elapsedTime = 0f;
        float interval = 1f / frequency;

        //시작 시 초기 색상을 투명하게
        flickerImage.color = Color.clear;

        while (elapsedTime < duration)
        {
            flickerImage.color = finalColorA;
            yield return new WaitForSeconds(interval / 2);
            if (elapsedTime + (interval / 2) > duration) break;
            flickerImage.color = finalColorB;
            yield return new WaitForSeconds(interval / 2);

            elapsedTime += interval;
        }

        flickerImage.color = Color.clear;
    }
}