using UnityEngine;

/// (임시 조치용) 컷씬 이벤트에 반응하여 메인 카메라의 Orthographic Size를 조절
public class CutsceneCameraFix : MonoBehaviour
{
    [Header("컷씬 카메라 임시 설정")]
    [Tooltip("컷씬이 재생될 때 강제로 설정할 Orthographic Size")]
    public float cutsceneOrthographicSize = 10.0f;

    private float originalOrthographicSize;
    private bool isOriginalSizeStored = false;

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
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("메인카메라 null체크");
            return;
        }

        if (!isOriginalSizeStored)
        {
            originalOrthographicSize = mainCamera.orthographicSize;
            isOriginalSizeStored = true;
        }

        mainCamera.orthographicSize = cutsceneOrthographicSize;
    }

    private void HandleCutsceneEnd()
    {
        if (isOriginalSizeStored)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCamera.orthographicSize = originalOrthographicSize;
            }
            isOriginalSizeStored = false;
        }
    }
}