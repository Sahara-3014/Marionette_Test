using UnityEngine;
using UnityEngine.Playables; 

public class TimelineAutoDestroy : MonoBehaviour
{
    private PlayableDirector director;

    void Awake()
    {
        director = GetComponent<PlayableDirector>();
    }

    void OnEnable()
    {
        director.stopped += DestroyRootObject;
    }

    void OnDisable()
    {
        director.stopped -= DestroyRootObject;
    }

    private void DestroyRootObject(PlayableDirector director)
    {
        if (transform.root != null)
        {
            Destroy(transform.root.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnValidate()
    {
        PlayableDirector dir = GetComponent<PlayableDirector>();
        if (dir != null && dir.extrapolationMode == DirectorWrapMode.Loop)
        {
            Debug.LogWarning("TimelineAutoDestroy: 실패", this);
        }
    }
}