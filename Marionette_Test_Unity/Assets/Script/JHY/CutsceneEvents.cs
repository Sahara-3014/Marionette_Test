using System;

public static class CutsceneEvents
{
    public static event Action OnCutsceneStart;
    public static event Action OnCutsceneEnd;

    public static void TriggerCutsceneStart()
    {
        OnCutsceneStart?.Invoke();
    }
    public static void TriggerCutsceneEnd()
    {
        OnCutsceneEnd?.Invoke();
    }
}