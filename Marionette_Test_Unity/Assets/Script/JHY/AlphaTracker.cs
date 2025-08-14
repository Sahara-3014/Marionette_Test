using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class AlphaChangeTracker : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color lastColor;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("[AlphaChangeTracker] SpriteRenderer가 없습니다.");
            enabled = false;
            return;
        }
        lastColor = sr.color;
    }

    void LateUpdate()
    {
        if (!Mathf.Approximately(sr.color.a, lastColor.a))
        {
            LogAlphaChange(lastColor.a, sr.color.a);
            lastColor = sr.color;
        }
    }

    private void LogAlphaChange(float oldAlpha, float newAlpha)
    {
        StackTrace stackTrace = new StackTrace(2, true);
        // 2단계 위부터 스택 찍음 (LogAlphaChange 호출자 제외)

        Debug.Log($"[AlphaChangeTracker] Alpha 변경 감지: {oldAlpha} → {newAlpha}\n" +
                  $"호출 스택:\n{stackTrace}");
    }
}
