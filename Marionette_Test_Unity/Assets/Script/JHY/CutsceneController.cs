// CutsceneController.cs
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class CutsceneController : MonoBehaviour
{
    void OnEnable()
    {
        CutsceneEvents.TriggerCutsceneStart();
    }

    void OnDisable()
    {
        CutsceneEvents.TriggerCutsceneEnd();
    }
}