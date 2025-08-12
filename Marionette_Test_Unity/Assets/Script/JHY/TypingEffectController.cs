using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TypingEffectController : MonoBehaviour
{
    [Header("연결 대상 (Required)")]
    public TextMeshProUGUI textComponent;
    public ScrollRect scrollRect;

    [Header("타이핑 내용")]
    public List<string> normalLines;
    public List<string> errorLines;

    [Header("타이핑 속도 설정")]
    public float normalCharsPerSecond = 20f;
    public float errorCharsPerSecond = 300f;

    [Header("딜레이 설정")]
    public float delayBetweenLines = 0.2f;
    public float delayBeforeErrors = 1.0f;

    // 사용자가 스크롤을 수동으로 조작했는지 확인하는 플래그
    private bool isManuallyScrolled = false;

    void OnEnable()
    {
        if (textComponent == null || scrollRect == null)
        {
            Debug.LogError("[TypingEffectController] Text Component 또는 Scroll Rect가 연결되지 않았습니다!");
            return;
        }

        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);

        StopAllCoroutines();
        StartCoroutine(TypewriterSequence());
    }

    void OnDisable()
    {
        if (scrollRect != null)
        {
            scrollRect.onValueChanged.RemoveListener(OnScrollValueChanged);
        }
    }

    private void OnScrollValueChanged(Vector2 position)
    {
        if (scrollRect.verticalNormalizedPosition > 0.01f)
        {
            isManuallyScrolled = true;
        }
    }

    private IEnumerator TypewriterSequence()
    {
        isManuallyScrolled = false; // 시퀀스 시작 시 초기화
        textComponent.text = "";
        yield return StartCoroutine(TypeLines(normalLines, normalCharsPerSecond));
        yield return new WaitForSeconds(delayBeforeErrors);
        yield return StartCoroutine(TypeLines(errorLines, errorCharsPerSecond));
    }

    private IEnumerator TypeLines(List<string> lines, float speed)
    {
        float safeSpeed = Mathf.Max(0.1f, speed);

        foreach (var line in lines)
        {
            isManuallyScrolled = false;

            for (int i = 0; i < line.Length; i++)
            {
                textComponent.text += line[i];
                yield return new WaitForSeconds(1f / safeSpeed);
            }
            textComponent.text += "\n";
            yield return null;

            if (!isManuallyScrolled)
            {
                scrollRect.verticalNormalizedPosition = 0f;
            }
            yield return new WaitForSeconds(delayBetweenLines);
        }
    }
}