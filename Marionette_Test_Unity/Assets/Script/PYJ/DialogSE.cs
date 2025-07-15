
using UnityEngine;

public enum SEType
{
    None = 0,
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


    // 생성자 정의
    public DialogSE(SEType type = SEType.SE, AudioClip clip = null, int loopCount = 1, float volume = 0.5f)
    {
        this.type = type;
        this.clip = clip;
        this.loopCount = loopCount;
        this.volume = volume;
    }

    public DialogSE() { } // 매개변수 없는 생성자 (초기화용)

}
