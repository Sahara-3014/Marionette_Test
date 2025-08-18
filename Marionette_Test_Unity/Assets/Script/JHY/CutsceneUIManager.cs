// UIManager.cs
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 게임의 전반적인 UI 상태를 관리하고, 컷씬 이벤트에 반응합니다.
/// </summary>
public class CutsceneUIManager : MonoBehaviour
{
    public static CutsceneUIManager Instance { get; private set; }

    [Header("관리 대상 UI")]
    [Tooltip("컷씬이 아닐 때, 평소에 활성화되어야 할 UI 오브젝트 목록")]
    public List<GameObject> gameplayUiObjects;

    void Awake()
    {
        // 싱글톤 설정
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnEnable()
    {
        CutsceneEvents.OnCutsceneStart += HandleCutsceneStart;
        CutsceneEvents.OnCutsceneEnd += HandleCutsceneEnd;
    }

    void OnDisable()
    {
        CutsceneEvents.OnCutsceneStart -= HandleCutsceneStart;
        CutsceneEvents.OnCutsceneEnd -= HandleCutsceneEnd;
    }

    private void HandleCutsceneStart()
    {
        SetGameplayUiActive(false); 
    }

    private void HandleCutsceneEnd()
    {
        SetGameplayUiActive(true); 
    }

    private void SetGameplayUiActive(bool isActive)
    {
        foreach (var uiObject in gameplayUiObjects)
        {
            if (uiObject != null)
            {
                uiObject.SetActive(isActive);
            }
        }
    }
}