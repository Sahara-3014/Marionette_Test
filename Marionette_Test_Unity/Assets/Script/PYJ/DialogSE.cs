// DialogSE.cs
using UnityEngine;

public enum SEType
{
    BGM,
    SE
}

[System.Serializable]
public class DialogSE
{
    public SEType type;          // BGM인지 SE인지
    public AudioClip clip;       // 실제 오디오 클립
    public int loopCount = 1;    // 반복 횟수 (0이면 무한 반복)
    public float volume = 1.0f;  // 볼륨 (0~1)
}
