using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System;
using static DialogueManager;

public class GoogleSheetLoader : MonoBehaviour
{
    public string apiKey = "AIzaSyCYF6AGzi8Fe0HhVew-t0LOngxs0IOZIuc";
    public string spreadsheetId = "1N2Z-yXGz8rUvUBwLfkeB9GYIOWhMrfs6lWok9lcNIjk";
    public string sheetName = "INTRO";

    public DialogueManager dialogueSystem; // 대화 시스템 참조

    // 구글 시트 불러오기 호출용 함수
    public void LoadDialoguesFromSheet()
    {
        StartCoroutine(LoadGoogleSheet());
    }

    IEnumerator LoadGoogleSheet()
    {
        string range = $"{sheetName}!A1:Z100"; // N열까지 확장 (bgm 컬럼 포함)
        string url = $"https://sheets.googleapis.com/v4/spreadsheets/{spreadsheetId}/values/{range}?key={apiKey}";

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("시트 불러오기 실패: " + www.error);
            yield break;
        }

        List<DialogueData> dialogueList = new List<DialogueData>();
        var json = JSON.Parse(www.downloadHandler.text);
        var values = json["values"];

        for (int i = 2; i < values.Count; i++) // 0:헤더, 1:설명 등 스킵
        {
            var row = values[i];
            Dialog_CharPos pos1, pos2;



            string posStr1 = (row.Count > 2 && row[2] != null) ? row[2].Value.Trim() : "None";
            string posStr2 = (row.Count > 3 && row[3] != null) ? row[3].Value.Trim() : "None";
            string background = (row.Count > 4 && row[4] != null) ? row[4].Value.Trim() : "";
            string BG_EFFECT = (row.Count > 6 && row[6] != null) ? row[6].Value.Trim() : "";
            string CH_EFFECT = (row.Count > 7 && row[7] != null) ? row[7].Value.Trim() : "";

            string characterName1 = (row.Count > 8 && row[8] != null) ? row[8].Value.Trim() : "";
            string characterName2 = (row.Count > 9 && row[9] != null) ? row[9].Value.Trim() : "";
            string status_head_1 = (row.Count > 10 && row[10] != null) ? row[10].Value.Trim() : "";
            string status_body_1 = (row.Count > 11 && row[11] != null) ? row[11].Value.Trim() : "";
            string status_head_2 = (row.Count > 12 && row[12] != null) ? row[12].Value.Trim() : "";
            string status_body_2 = (row.Count > 13 && row[13] != null) ? row[13].Value.Trim() : "";

            string dialogueText = (row.Count > 14 && row[14] != null) ? row[14].Value.Trim() : "";
            string bgmName = (row.Count > 15 && row[15] != null) ? row[15].Value.Trim() : "";
            string sfx1Name = (row.Count > 16 && row[16] != null) ? row[16].Value.Trim() : "";
            string sfx2Name = (row.Count > 17 && row[17] != null) ? row[17].Value.Trim() : "";

            string speaker = (row.Count > 18 && row[18] != null) ? row[18].Value.Trim() : characterName1;

            List<CharacterStatus> characters = new List<CharacterStatus>();

            // 먼저 posStr1 → pos1
            if (!Enum.TryParse(posStr1, out pos1))
            {
                if (int.TryParse(posStr1, out int intPos1) && Enum.IsDefined(typeof(Dialog_CharPos), intPos1))
                    pos1 = (Dialog_CharPos)intPos1;
                else
                    pos1 = Dialog_CharPos.None;
            }

            if (!Enum.TryParse(posStr2, out pos2))
            {
                if (int.TryParse(posStr2, out int intPos2) && Enum.IsDefined(typeof(Dialog_CharPos), intPos2))
                    pos2 = (Dialog_CharPos)intPos2;
                else
                    pos2 = Dialog_CharPos.None;
            }

            // 그런 다음 캐릭터 추가
            if (!string.IsNullOrEmpty(characterName1))
            {
                characters.Add(new CharacterStatus
                {
                    name = characterName1,
                    head = status_head_1,
                    body = status_body_1,
                    position = pos1
                });
            }

            if (!string.IsNullOrEmpty(characterName2))
            {
                characters.Add(new CharacterStatus
                {
                    name = characterName2,
                    head = status_head_2,
                    body = status_body_2,
                    position = pos2
                });
            }

            // BG_EFFECT → Dialog_ScreenEffect 변환
            Dialog_ScreenEffect screenEffect;
            if (!Enum.TryParse(BG_EFFECT, out screenEffect))
            {
                screenEffect = Dialog_ScreenEffect.None;
            }

            // CH_EFFECT → Dialog_CharEffect 변환
            Dialog_CharEffect charEffect;
            if (!Enum.TryParse(CH_EFFECT, out charEffect))
            {
                charEffect = Dialog_CharEffect.None;
            }



            if (!Enum.TryParse(posStr1, out pos1))
            {
                if (int.TryParse(posStr1, out int intPos1) && Enum.IsDefined(typeof(Dialog_CharPos), intPos1))
                    pos1 = (Dialog_CharPos)intPos1;
                else
                    pos1 = Dialog_CharPos.None;
            }

            // posStr2 → pos2
            if (!Enum.TryParse(posStr2, out pos2))
            {
                if (int.TryParse(posStr2, out int intPos2) && Enum.IsDefined(typeof(Dialog_CharPos), intPos2))
                    pos2 = (Dialog_CharPos)intPos2;
                else
                    pos2 = Dialog_CharPos.None;
            }



            // BGM 처리
            DialogSE bgmSE = null;
            if (!string.IsNullOrEmpty(bgmName))
            {
                bgmSE = new DialogSE
                {
                    type = SEType.BGM,
                    clip = LoadAudioClipByName(bgmName),
                    volume = 1f,
                    loopCount = 1
                };
            }

            // SFX1 처리
            DialogSE sfx1SE = null;
            if (!string.IsNullOrEmpty(sfx1Name))
            {
                sfx1SE = new DialogSE
                {
                    type = SEType.SE,
                    clip = LoadAudioClipByName(sfx1Name),
                    volume = 1f,
                    loopCount = 1
                };
            }

            // SFX2 처리
            DialogSE sfx2SE = null;
            if (!string.IsNullOrEmpty(sfx2Name))
            {
                sfx2SE = new DialogSE
                {
                    type = SEType.SE,
                    clip = LoadAudioClipByName(sfx2Name),
                    volume = 1f,
                    loopCount = 1
                };
            }

            DialogueData d = new DialogueData
            {
                characters = characters.ToArray(),
                background = background,
                dialogue = dialogueText,
                cg = null,

                bgm = bgmSE,
                se1 = sfx1SE,
                se2 = sfx2SE,

                charPos1 = pos1,
                charPos2 = pos2,

                screenEffect = screenEffect,
                charEffect = charEffect,

                speaker = speaker
            };

            dialogueList.Add(d);
        }

        dialogueSystem.SetDialogue(dialogueList.ToArray());
        dialogueSystem.ShowDialogue();


    }

    // Resources/Audio 폴더 내 오디오클립 로드 함수
    public AudioClip LoadAudioClipByName(string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
            return null;

        AudioClip clip = Resources.Load<AudioClip>($"Audio/{clipName}");
        if (clip == null)
        {
            Debug.LogWarning($"AudioClip '{clipName}'를 Resources/Audio 폴더에서 찾을 수 없습니다.");
        }
        return clip;
    }
}
