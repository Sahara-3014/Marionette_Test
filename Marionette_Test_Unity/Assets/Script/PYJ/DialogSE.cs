
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

    public SEType type;          // BGM���� SE����
    public AudioClip clip;       // ���� ����� Ŭ��
    public int loopCount = 1;    // �ݺ� Ƚ�� (0�̸� ���� �ݺ�)
    public float volume = 1.0f;  // ���� (0~1)


    // ������ ����
    public DialogSE(SEType type = SEType.SE, AudioClip clip = null, int loopCount = 1, float volume = 0.5f)
    {
        this.type = type;
        this.clip = clip;
        this.loopCount = loopCount;
        this.volume = volume;
    }

    public DialogSE() { } // �Ű����� ���� ������ (�ʱ�ȭ��)

}
