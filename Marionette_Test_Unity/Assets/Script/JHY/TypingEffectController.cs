using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TypingEffectController : MonoBehaviour
{
    [Header("연결 대상 (Required)")]
    [Tooltip("타이핑 효과를 표시할 TextMeshProUGUI 컴포넌트를 연결해주세요.")]
    public TextMeshProUGUI textComponent;
    [Tooltip("스크롤을 제어할 ScrollRect 컴포넌트를 연결해주세요.")]
    public ScrollRect scrollRect;

    [Header("타이핑 내용")]
    [TextArea(3, 5)]
    public List<string> normalLines;
    [TextArea(3, 10)]
    public List<string> errorLines;

    [Header("타이핑 속도 설정")]
    public float normalCharsPerSecond = 20f;
    public float errorCharsPerSecond = 300f;

    [Header("딜레이 설정")]
    public float delayBetweenLines = 0.2f;
    public float delayBeforeErrors = 1.0f;

    void OnEnable()
    {
        if (textComponent == null || scrollRect == null)
        {
            Debug.LogError("[TypingEffectController] Text Component 또는 Scroll Rect가 연결되지 않았습니다!");
            return;
        }
        StopAllCoroutines();
        StartCoroutine(TypewriterSequence());
    }

    private IEnumerator TypewriterSequence()
    {
        textComponent.text = "";
        yield return StartCoroutine(TypeLines(normalLines, normalCharsPerSecond));
        yield return new WaitForSeconds(delayBeforeErrors);
        yield return StartCoroutine(TypeLines(errorLines, errorCharsPerSecond));
    }

    private IEnumerator TypeLines(List<string> lines, float speed)
    {
        float safeSpeed = Mathf.Max(0.1f, speed);
        RectTransform contentRect = scrollRect.content;

        foreach (var line in lines)
        {
            // 한 글자씩 타이핑
            for (int i = 0; i < line.Length; i++)
            {
                textComponent.text += line[i];
                yield return new WaitForSeconds(1f / safeSpeed);
            }

            textComponent.text += "\n";

            // ▼▼▼▼▼ 여기가 최종 수정된 단일 솔루션입니다 ▼▼▼▼▼

            // 1. 레이아웃을 강제로 즉시 재계산하도록 명령합니다.
            if (contentRect != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
            }

            // 2. 모든 UI 업데이트와 렌더링이 끝나는 프레임의 맨 마지막까지 기다립니다.
            yield return new WaitForEndOfFrame();

            // 3. 이제 모든 계산이 끝난 가장 확실한 타이밍에 스크롤을 맨 아래로 내립니다.
            if (scrollRect != null)
            {
                scrollRect.verticalNormalizedPosition = 0f;
            }

            // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

            yield return new WaitForSeconds(delayBetweenLines);
        }
    }
}