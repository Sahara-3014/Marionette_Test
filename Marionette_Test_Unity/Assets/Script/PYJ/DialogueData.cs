using UnityEngine;
using static DialogueManager;


[System.Serializable]
public class CharacterStatus
{
    public string name;
    public string head;
    public string body;
    public Dialog_CharPos position;
    public Dialog_CharEffect effect; // ← 개별 효과 필드 추가!
    public string cutscene;
}


[System.Serializable]
public class DialogueData
{
    public CharacterStatus[] characters;


    public string nextSheet;
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

    public Dialog_CharPos charPos1;
    public Dialog_CharPos charPos2;



    public string speaker; // 대사 주인공 (characterName1 또는 characterName2 중 하나)
    internal string cutscene;





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
