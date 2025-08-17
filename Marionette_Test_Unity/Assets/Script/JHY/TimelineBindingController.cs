using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

// 이 스크립트는 PlayableDirector와 함께 있어야 합니다.
[RequireComponent(typeof(PlayableDirector))]
public class TimelineBindingController : MonoBehaviour
{
    // 인스펙터에서 바인딩할 오브젝트의 태그를 직접 지정할 수 있도록 변수화합니다.
    [Header("바인딩 대상 설정")]
    [Tooltip("시그널을 수신할 게임 오브젝트의 태그를 입력하세요.")]
    public string targetTag = "EventManager";

    private PlayableDirector playableDirector;

    void Awake()
    {
        playableDirector = GetComponent<PlayableDirector>();

        // 타임라인 에셋이 할당되었는지 확인합니다.
        TimelineAsset timelineAsset = playableDirector.playableAsset as TimelineAsset;
        if (timelineAsset == null)
        {
            Debug.LogWarning("PlayableDirector에 타임라인 에셋이 할당되지 않았습니다.", this.gameObject);
            return;
        }

        // --- 태그를 사용하여 동적 바인딩 수행 ---

        // 지정된 태그로 씬에서 게임 오브젝트를 찾습니다.
        GameObject targetObject = GameObject.FindWithTag(targetTag);

        // 오브젝트를 찾았는지 확인합니다.
        if (targetObject == null)
        {
            Debug.LogError($"'{targetTag}' 태그를 가진 오브젝트를 씬에서 찾을 수 없습니다! 시그널 바인딩에 실패했습니다.", this.gameObject);
            return;
        }

        // 타임라인의 모든 출력 트랙을 순회합니다.
        foreach (var track in timelineAsset.GetOutputTracks())
        {
            // 만약 트랙이 SignalTrack이라면, 해당 트랙에 찾은 오브젝트를 바인딩합니다.
            if (track is SignalTrack)
            {
                // SetGenericBinding을 사용하여 런타임에 트랙과 오브젝트를 연결합니다.
                playableDirector.SetGenericBinding(track, targetObject);
                Debug.Log($"타임라인 트랙 '{track.name}'을 '{targetObject.name}' 오브젝트에 성공적으로 바인딩했습니다.", this.gameObject);
            }
            // 다른 종류의 트랙(예: AnimationTrack, AudioTrack 등)에 대한 바인딩 로직도
            // 필요한 경우 여기에 추가할 수 있습니다.
            // else if (track is AnimationTrack) { ... }
        }
    }
}