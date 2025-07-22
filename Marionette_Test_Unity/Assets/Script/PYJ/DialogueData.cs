using SimpleJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
public class DialogueChoice
{
    public string choiceText;
    public string nextIndex; // 선택 후 이동할 대사 배열 내 인덱스
}




[System.Serializable]
public class DialogueData
{
    public string choiceText;
    public string nextIndex;
    public int index;

    public DialogueChoice[] choices;
    public CharacterStatus[] characters;

    public string characterName;
    public string status;
    public Dialog_CharPos charPos;


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





    // ========== 생성자: string[] 로부터 ========== //
    public DialogueData(JSONNode node)
    {
        index = int.TryParse(GetText(node, 0), out int idx) ? idx : 0;          // INDEX
        nextIndex = GetText(node, 1);                                          // NEXTINDEX
        speaker = GetText(node, 2);                                            // SPEAKER

        // 캐릭터1 관련
        string name1 = GetText(node, 3);
        string pos1Str = GetText(node, 4);
        string chEffect1Str = GetText(node, 5);
        string head1 = GetText(node, 6);
        string body1 = GetText(node, 7);

        // 캐릭터2 관련
        string name2 = GetText(node, 8);
        string pos2Str = GetText(node, 9);
        string chEffect2Str = GetText(node, 10);
        string head2 = GetText(node, 11);
        string body2 = GetText(node, 12);

        string bgEffectStr = GetText(node, 13);
        background = GetText(node, 14);
        dialogue = GetText(node, 15);

        // 선택지 (최대 3개)
        choices = new DialogueChoice[0];
        List<DialogueChoice> choiceList = new List<DialogueChoice>();
        for (int i = 0; i < 3; i++)
        {
            string choiceText = GetText(node, 16 + i * 2);
            string choiceNext = GetText(node, 17 + i * 2);
            if (!string.IsNullOrEmpty(choiceText))
            {
                choiceList.Add(new DialogueChoice { choiceText = choiceText, nextIndex = choiceNext });
            }
        }
        choices = choiceList.ToArray();

        // 효과음 및 BGM
        string bgmName = GetText(node, 22);
        string sfx1Name = GetText(node, 23);
        string sfx2Name = GetText(node, 24);

        // 컷씬
        cutscene = GetText(node, 25);

        // 열거형 파싱
        if (!Enum.TryParse(bgEffectStr, out screenEffect))
            screenEffect = Dialog_ScreenEffect.None;

        if (!Enum.TryParse(chEffect1Str, out Dialog_CharEffect chEffect1))
            chEffect1 = Dialog_CharEffect.None;

        if (!Enum.TryParse(chEffect2Str, out Dialog_CharEffect chEffect2))
            chEffect2 = Dialog_CharEffect.None;

        // 캐릭터 위치 파싱 함수
        charPos1 = ParseCharPos(pos1Str);
        charPos2 = ParseCharPos(pos2Str);

        // 캐릭터 상태 세팅
        characters = new CharacterStatus[2];
        characters[0] = new CharacterStatus
        {
            name = name1,
            head = head1,
            body = body1,
            position = charPos1,
            effect = chEffect1,
            cutscene = cutscene
        };
        characters[1] = new CharacterStatus
        {
            name = name2,
            head = head2,
            body = body2,
            position = charPos2,
            effect = chEffect2,
            cutscene = cutscene
        };

        // 오디오 클립 로드
        if (!string.IsNullOrEmpty(bgmName))
        {
            bgm = new DialogSE
            {
                type = SEType.BGM,
                clip = LoadAudioClipByName(bgmName),
                volume = 1f,
                loopCount = 1
            };
        }
        if (!string.IsNullOrEmpty(sfx1Name))
        {
            se1 = new DialogSE
            {
                type = SEType.SE,
                clip = LoadAudioClipByName(sfx1Name),
                volume = 1f,
                loopCount = 1
            };
        }
        if (!string.IsNullOrEmpty(sfx2Name))
        {
            se2 = new DialogSE
            {
                type = SEType.SE,
                clip = LoadAudioClipByName(sfx2Name),
                volume = 1f,
                loopCount = 1
            };
        }
    }

    // 헬퍼 함수
    private Dialog_CharPos ParseCharPos(string posStr)
    {
        if (Enum.TryParse(posStr, out Dialog_CharPos pos))
            return pos;
        if (int.TryParse(posStr, out int num) && Enum.IsDefined(typeof(Dialog_CharPos), num))
            return (Dialog_CharPos)num;
        return Dialog_CharPos.None;
    }
    public DialogueData(string[] row)
    {
        index = (row.Length > 0 && int.TryParse(row[0], out int idx)) ? idx : 0;
        nextIndex = (row.Length > 1) ? row[1].Trim() : "";
        // CSV 각 칼럼을 인덱스로 직접 파싱
        // 예: background = row.Length > 4 ? row[4].Trim() : "";
        background = (row.Length > 4) ? row[4].Trim() : "";

        // 화면 효과, 캐릭터 효과
        string BG_EFFECT = (row.Length > 6) ? row[6].Trim() : "";
        string CH_EFFECT = (row.Length > 7) ? row[7].Trim() : "";

        if (!Enum.TryParse(BG_EFFECT, out screenEffect))
            screenEffect = Dialog_ScreenEffect.None;

        if (!Enum.TryParse(CH_EFFECT, out charEffect))
            charEffect = Dialog_CharEffect.None;

        // 캐릭터 이름 및 상태
        characterName = (row.Length > 8) ? row[8].Trim() : "";
        status = (row.Length > 9) ? row[9].Trim() : "";

        // 대사 텍스트
        dialogue = (row.Length > 10) ? row[10].Trim() : "";

        // 오디오 클립 이름
        string bgmName = (row.Length > 11) ? row[11].Trim() : "";
        string sfx1Name = (row.Length > 12) ? row[12].Trim() : "";
        string sfx2Name = (row.Length > 13) ? row[13].Trim() : "";

        // 캐릭터 위치 파싱 (row[3])
        string posStr = (row.Length > 3) ? row[3].Trim() : "None";

        if (!Enum.TryParse(posStr, out charPos))
        {
            if (int.TryParse(posStr, out int intPos) && Enum.IsDefined(typeof(Dialog_CharPos), intPos))
            {
                charPos = (Dialog_CharPos)intPos;
            }
            else
            {
                charPos = Dialog_CharPos.None;
            }
        }

        // 오디오 클립 로드
        bgm = new DialogSE(SEType.BGM, LoadAudioClipByName(bgmName));
        se1 = new DialogSE(SEType.SE, LoadAudioClipByName(sfx1Name));
        se2 = new DialogSE(SEType.SE, LoadAudioClipByName(sfx2Name));

    }


    private AudioClip LoadAudioClipByName(string clipName)
    {
        return Resources.Load<AudioClip>($"Audio/{clipName}");
    }


    private string GetText(JSONNode node, int index)
    {
        return (node.Count > index && node[index] != null) ? node[index].Value.Trim() : "";
    }
}
