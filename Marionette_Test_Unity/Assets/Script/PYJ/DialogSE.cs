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
    public SEType type;          // BGM���� SE����
    public AudioClip clip;       // ���� ����� Ŭ��
    public int loopCount = 1;    // �ݺ� Ƚ�� (0�̸� ���� �ݺ�)
    public float volume = 1.0f;  // ���� (0~1)


    // ������ ����
    public DialogSE(AudioClip clip = null, SEType type = SEType.SE, int loopCount = 1, float volume = 0.5f)
    {
        this.clip = clip;
        this.type = type;
        this.loopCount = loopCount;
        this.volume = volume;
    }

    public DialogSE() { } // �Ű����� ���� ������ (�ʱ�ȭ��)

}
