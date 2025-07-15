using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public string characterName;     // ��� ȭ�� �̸�
    public string status;            // ĳ���� ���� (��: ����, �г� ��)
    public string background;        // ��� Ű ��

    [TextArea]
    public string dialogue;          // ���� ��� �ؽ�Ʈ

    public Sprite cg;                // ���� �̹��� (CG ��)

    [HideInInspector] public int screenEffectIndex;  // �ܺ� �Է¿�
    [HideInInspector] public int charEffectIndex;

    public Dialog_ScreenEffect screenEffect = Dialog_ScreenEffect.None;
    public Dialog_CharEffect charEffect = Dialog_CharEffect.None;

    public float delay = 0.05f;
    public float duration = 0f;

    public DialogSE se1;
    public DialogSE se2;
    public DialogSE bgm;

    public Dialog_CharPos charPos;


    /// <summary>
    /// �ܺο��� ��ȣ�� ���� ä�� ��, ȣ���ؼ� enum�� ����
    /// </summary>
    public void ApplyEffectIndices()
    {
        if (System.Enum.IsDefined(typeof(Dialog_ScreenEffect), screenEffectIndex))
            screenEffect = (Dialog_ScreenEffect)screenEffectIndex;
        else
            screenEffect = Dialog_ScreenEffect.None;

        if (System.Enum.IsDefined(typeof(Dialog_CharEffect), charEffectIndex))
            charEffect = (Dialog_CharEffect)charEffectIndex;
        else
            charEffect = Dialog_CharEffect.None;
    }
}
