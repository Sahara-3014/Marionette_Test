using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[RequireComponent(typeof(PlayableDirector))]
public class TimelineBindingResolver : MonoBehaviour
{
    private PlayableDirector director;

    void Awake()
    {
        director = GetComponent<PlayableDirector>();
       // ResolveBindings();
    }

    private void Start()
    {
        ResolveBindings();
    }

    private void ResolveBindings()
    {
        if (director.playableAsset == null) return;

        if (EffectManager.Instance == null)
        {
            Debug.LogWarning("EffectManager.Instance가 아직 준비되지 않아 타임라인 바인딩을 해결할 수 없습니다.", this);
            return;
        }

        foreach (var track in director.playableAsset.outputs)
        {
            if (track.sourceObject is SignalTrack)
            {
                var currentBinding = director.GetGenericBinding(track.sourceObject);

                if (currentBinding == null)
                {
                    director.SetGenericBinding(track.sourceObject, EffectManager.Instance.gameObject);
                    Debug.Log($"타임라인 트랙 '{track.streamName}'의 비어있는 Signal 바인딩을 EffectManager에 자동으로 연결했습니다.");
                }
            }
        }
    }
}