using UnityEngine;
using System.Collections;

public class EffectCameraController : MonoBehaviour
{
    public static EffectCameraController Instance { get; private set; }

    private Transform cameraRoot;
    private Camera mainCamera;
    private float initialOrthographicSize;
    private bool isInitialSizeStored = false;

    private Coroutine runningCoroutine;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    public Transform GetCurrentCameraRoot()
    {
        EnsureCameraTargets();
        return this.cameraRoot;
    }

    public Camera GetCurrentCamera()
    {
        EnsureCameraTargets();
        return this.mainCamera;
    }

    public float GetInitialOrthographicSize() 
    { 
        EnsureCameraTargets(); 
        return this.initialOrthographicSize; 
    }

    public void SetTargetCamera(Camera cameraToControl)
    {
        if (cameraToControl != null)
        {
            this.mainCamera = cameraToControl;
            this.cameraRoot = cameraToControl.transform.parent ?? cameraToControl.transform;

            if (!isInitialSizeStored)
            {
                initialOrthographicSize = mainCamera.orthographicSize;
                isInitialSizeStored = true;
            }
        }
        else
        {
            Debug.LogWarning("[EffectCameraController] 카메가가 null 상태임.");
        }
    }

    #region 외부 공개 함수

    public void MoveTo(Vector3 targetPosition, float duration)
    {
        if (!EnsureCameraTargets()) return;
        Vector3 targetWithZ = new Vector3(targetPosition.x, targetPosition.y, cameraRoot.position.z);
        StartMoveZoomCoroutine(targetWithZ, null, duration);
    }

    public void ZoomTo(float targetOrthographicSize, float duration)
    {
        if (!EnsureCameraTargets()) return;
        StartMoveZoomCoroutine(null, targetOrthographicSize, duration);
    }

    public void MoveAndZoomTo(Vector3 targetPosition, float targetOrthographicSize, float duration)
    {
        if (!EnsureCameraTargets()) return;
        Vector3 targetWithZ = new Vector3(targetPosition.x, targetPosition.y, cameraRoot.position.z);
        StartMoveZoomCoroutine(targetWithZ, targetOrthographicSize, duration);
    }

    public void SetPositionAndZoom(Vector3 targetPosition, float targetOrthographicSize)
    {
        if (!EnsureCameraTargets()) return;
        if (runningCoroutine != null) StopCoroutine(runningCoroutine);

        Vector3 targetWithZ = new Vector3(targetPosition.x, targetPosition.y, cameraRoot.position.z);
        cameraRoot.position = targetWithZ;
        mainCamera.orthographicSize = targetOrthographicSize;
    }

    public void SetPosition(Vector3 targetPosition)
    {
        if (!EnsureCameraTargets()) return;
        if (runningCoroutine != null) StopCoroutine(runningCoroutine);

        Vector3 targetWithZ = new Vector3(targetPosition.x, targetPosition.y, cameraRoot.position.z);
        cameraRoot.position = targetWithZ;
    }

    #endregion

    #region 내부 구현
    private void StartMoveZoomCoroutine(Vector3? targetPos, float? targetZoom, float duration)
    {
        if (runningCoroutine != null)
        {
            StopCoroutine(runningCoroutine);
        }
        runningCoroutine = StartCoroutine(MoveZoomCoroutine(targetPos, targetZoom, duration));
    }

    private IEnumerator MoveZoomCoroutine(Vector3? targetPos, float? targetZoom, float duration)
    {
        if (!EnsureCameraTargets()) yield break;

        Transform camRootTransform = this.cameraRoot;
        Vector3 startPos = camRootTransform.position;
        float startZoom = this.mainCamera.orthographicSize;
        float time = 0f;

        if (duration <= 0.01f)
        {
            if (targetPos.HasValue) camRootTransform.position = targetPos.Value;
            if (targetZoom.HasValue) mainCamera.orthographicSize = targetZoom.Value;
            yield break;
        }

        while (time < duration)
        {
            float t = time / duration;
            t = t * t * (3f - 2f * t); // SmoothStep 보간

            if (targetPos.HasValue)
            {
                camRootTransform.position = Vector3.Lerp(startPos, targetPos.Value, t);
            }
            if (targetZoom.HasValue)
            {
                mainCamera.orthographicSize = Mathf.Lerp(startZoom, targetZoom.Value, t);
            }

            time += Time.deltaTime;
            yield return null;
        }

        if (targetPos.HasValue) camRootTransform.position = targetPos.Value;
        if (targetZoom.HasValue) mainCamera.orthographicSize = targetZoom.Value;
    }

    private bool EnsureCameraTargets()
    {
        if (mainCamera == null || cameraRoot == null)
        {
            var cam = Camera.main;
            if (cam != null)
            {
                SetTargetCamera(cam);
                return true;
            }
            else
            {
                Debug.LogError("[EffectCameraController]제어할 Main Camera를 찾을 수 없습니다!!!");
                return false;
            }
        }
        return true;
    }
    #endregion
}