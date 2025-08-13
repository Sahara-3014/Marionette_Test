using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum CameraCommandType
{
    MoveTo,
    ZoomTo,
    MoveAndZoomTo,
    CutTo,
    CutAndZoomTo,
    Wait,
    RestoreInitial
}

[System.Serializable]
public class CameraStep
{
    public CameraCommandType command;
    public Transform target;
    [Tooltip("줌 비율(%). 100%가 기본, 50%는 2배 확대, 200%는 2배 축소.")]
    [Range(10, 400)]
    public float zoomPercent = 100f;
    public float duration = 1.0f;
}

public class CameraSequencer : MonoBehaviour
{
    public List<CameraStep> sequenceSteps;

    private Vector3 initialCameraPosition;
    private float initialCameraZoom;
    private bool isInitialized = false;

    void Start()
    {
        if (EffectCameraController.Instance != null && sequenceSteps != null && sequenceSteps.Count > 0)
        {
            StartCoroutine(PlaySequence());
        }
        else
        {
            Debug.LogWarning("카메라 시퀀스를 실행할 수 없습니다.");
            Destroy(gameObject);
        }
    }

    private void StoreInitialState()
    {
        if (isInitialized || EffectCameraController.Instance == null) return;
        Transform currentRoot = EffectCameraController.Instance.GetCurrentCameraRoot();
        initialCameraZoom = EffectCameraController.Instance.GetInitialOrthographicSize();
        if (currentRoot != null)
        {
            initialCameraPosition = currentRoot.position;
            isInitialized = true;
        }
    }

    private void RestoreInitialState(float duration)
    {
        if (!isInitialized || EffectCameraController.Instance == null) return;
        EffectCameraController.Instance.MoveAndZoomTo(initialCameraPosition, initialCameraZoom, duration);
    }

    private IEnumerator PlaySequence()
    {
        StoreInitialState();
        yield return null;

        foreach (var step in sequenceSteps)
        {
            yield return StartCoroutine(ExecuteStep(step));
        }

        Destroy(gameObject);
    }

    private IEnumerator ExecuteStep(CameraStep step)
    {
        var controller = EffectCameraController.Instance;
        if (controller == null) yield break;
        float targetSize = initialCameraZoom * (100f / step.zoomPercent);

        switch (step.command)
        {
            case CameraCommandType.MoveTo:
                if (step.target != null) controller.MoveTo(step.target.position, step.duration);
                yield return new WaitForSeconds(step.duration);
                break;
            case CameraCommandType.ZoomTo:
                controller.ZoomTo(targetSize, step.duration);
                yield return new WaitForSeconds(step.duration);
                break;
            case CameraCommandType.MoveAndZoomTo:
                if (step.target != null) controller.MoveAndZoomTo(step.target.position, targetSize, step.duration);
                yield return new WaitForSeconds(step.duration);
                break;

            case CameraCommandType.CutTo:
                if (step.target != null) controller.SetPosition(step.target.position);
                break;
            case CameraCommandType.CutAndZoomTo:
                if (step.target != null) controller.SetPositionAndZoom(step.target.position, targetSize);
                break;

            case CameraCommandType.Wait:
                yield return new WaitForSeconds(step.duration);
                break;
            case CameraCommandType.RestoreInitial:
                RestoreInitialState(step.duration);
                yield return new WaitForSeconds(step.duration);
                break;
        }
    }
}