using UnityEngine;

[System.Serializable]
public class DialogueData
{
    public string characterName;     // 대사 화자 이름
    public string status;            // 캐릭터 상태 (예: 웃음, 분노 등)
    public string background;        // 배경 키 값

    [TextArea]
    public string dialogue;          // 실제 대사 텍스트

    public Sprite cg;                // 삽입 이미지 (CG 등)

    [HideInInspector] public int screenEffectIndex;  // 외부 입력용
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
    /// 외부에서 번호로 값을 채운 뒤, 호출해서 enum에 적용
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
