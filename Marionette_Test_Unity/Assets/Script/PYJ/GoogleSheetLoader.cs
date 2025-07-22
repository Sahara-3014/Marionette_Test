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
    public List<string> fixedSheetSequence = new List<string> { "INTRO", "START", "CHAPTER1" };

    private int currentFixedIndex = 0;
    public bool usingBranching = false;

    private string currentSheet = "";

    public DialogueManager dialogueSystem; // 대화 시스템 참조

    public void StartDialogue()
    {
        currentFixedIndex = 0;
        usingBranching = false;
        LoadDialoguesFromSheet(fixedSheetSequence[currentFixedIndex]);
    }

    public void LoadNextSheet(string nextSheetFromData)
    {
        if (!usingBranching && currentFixedIndex + 1 < fixedSheetSequence.Count)
        {
            currentFixedIndex++;
            LoadDialoguesFromSheet(fixedSheetSequence[currentFixedIndex]);
        }
        else
        {
            usingBranching = true;
            if (!string.IsNullOrEmpty(nextSheetFromData))
            {
                LoadDialoguesFromSheet(nextSheetFromData);
            }
            else
            {
                Debug.Log("대화 종료 또는 분기 종료: 다음 시트 없음");
            }
        }
    }

    public void LoadDialoguesFromSheet(string sheetName)
    {
        currentSheet = sheetName;
        StartCoroutine(LoadGoogleSheet(sheetName));
    }

    IEnumerator LoadGoogleSheet(string sheetName)
    {
        string range = $"{sheetName}!A1:Z100";
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

            string characterName1 = (row.Count > 3) ? row[3].Value.Trim() : "";
            string posStr1 = (row.Count > 4) ? row[4].Value.Trim() : "None";
            string CH_EFFECT_1 = (row.Count > 5) ? row[5].Value.Trim() : "";
            string status_head_1 = (row.Count > 6) ? row[6].Value.Trim() : "";
            string status_body_1 = (row.Count > 7) ? row[7].Value.Trim() : "";

            string characterName2 = (row.Count > 8) ? row[8].Value.Trim() : "";
            string posStr2 = (row.Count > 9) ? row[9].Value.Trim() : "None";
            string CH_EFFECT_2 = (row.Count > 10) ? row[10].Value.Trim() : "";
            string status_head_2 = (row.Count > 11) ? row[11].Value.Trim() : "";
            string status_body_2 = (row.Count > 12) ? row[12].Value.Trim() : "";

            string BG_EFFECT = (row.Count > 13) ? row[13].Value.Trim() : "";
            string background = (row.Count > 14) ? row[14].Value.Trim() : "";

            string dialogueText = (row.Count > 15) ? row[15].Value.Trim() : "";


            string choice1 = (row.Count > 16) ? row[16].Value.Trim() : "";
            string next1Str = (row.Count > 17) ? row[17].Value.Trim() : "";

            string choice2 = (row.Count > 18) ? row[18].Value.Trim() : "";
            string next2Str = (row.Count > 19) ? row[19].Value.Trim() : "";

            string choice3 = (row.Count > 20) ? row[20].Value.Trim() : "";
            string next3Str = (row.Count > 21) ? row[21].Value.Trim() : "";


            string bgmName = (row.Count >22) ? row[22].Value.Trim() : "";
            string sfx1Name = (row.Count > 23) ? row[23].Value.Trim() : "";
            string sfx2Name = (row.Count > 24) ? row[24].Value.Trim() : "";

            string currentindex = (row.Count > 0) ? row[0].Value.Trim() : "";
            int parsedIndex = -1;
            if (!string.IsNullOrEmpty(currentindex))
            {
                int.TryParse(currentindex, out parsedIndex);
            }
            string nextIndexStr = (row.Count > 1) ? row[1].Value.Trim() : ""; // 예를 들면 "다음_인덱스" 열 위치 맞춰서 조정
            int nextIndexValue = 0;
            if (!string.IsNullOrEmpty(nextIndexStr) && int.TryParse(nextIndexStr, out int parsedNextIndex))
            {
                nextIndexValue = parsedNextIndex;
            }
            else
            {
                nextIndexValue = 0; // 없으면 0 또는 종료 표시값
            }
            string speaker = (row.Count > 2 && row[2] != null) ? row[2].Value.Trim() : characterName1;

            string cutscene = (row.Count > 25) ? row[25].Value.Trim() : "";
            List<DialogueChoice> choices = new List<DialogueChoice>();
            int next1Index = -1;
            if (!string.IsNullOrEmpty(next1Str))
                int.TryParse(next1Str, out next1Index);

            if (!string.IsNullOrEmpty(choice1))
                choices.Add(new DialogueChoice { choiceText = choice1, nextIndex = next1Index });

            if (!string.IsNullOrEmpty(choice2))
            {
                int next2Index = -1;
                if (!string.IsNullOrEmpty(next2Str))
                    int.TryParse(next2Str, out next2Index);
                choices.Add(new DialogueChoice { choiceText = choice2, nextIndex = next2Index });
            }

            if (!string.IsNullOrEmpty(choice3))
            {
                int next3Index = -1;
                if (!string.IsNullOrEmpty(next3Str))
                    int.TryParse(next3Str, out next3Index);
                choices.Add(new DialogueChoice { choiceText = choice3, nextIndex = next3Index });
            }




            Dialog_CharPos pos1 = ParseCharPos(posStr1);
            Dialog_CharPos pos2 = ParseCharPos(posStr2);
            Dialog_ScreenEffect screenEffect = ParseEnum<Dialog_ScreenEffect>(BG_EFFECT);
            Dialog_CharEffect charEffect1 = ParseEnum<Dialog_CharEffect>(CH_EFFECT_1);
            Dialog_CharEffect charEffect2 = ParseEnum<Dialog_CharEffect>(CH_EFFECT_2);

            List<CharacterStatus> characters = new List<CharacterStatus>();
            if (!string.IsNullOrEmpty(characterName1))
            {
                characters.Add(new CharacterStatus
                {
                    name = characterName1,
                    head = status_head_1,
                    body = status_body_1,
                    position = pos1,
                    effect = charEffect1
                });
            }

            if (!string.IsNullOrEmpty(characterName2))
            {
                characters.Add(new CharacterStatus
                {
                    name = characterName2,
                    head = status_head_2,
                    body = status_body_2,
                    position = pos2,
                    effect = charEffect2
                });
            }

            DialogueData d = new DialogueData
            {
                characters = characters.ToArray(),
                background = background,
                dialogue = dialogueText,
                speaker = speaker,
                cg = null,
                bgm = CreateSE(SEType.BGM, bgmName),
                se1 = CreateSE(SEType.SE, sfx1Name),
                se2 = CreateSE(SEType.SE, sfx2Name),
                charPos1 = pos1,
                charPos2 = pos2,
                screenEffect = screenEffect,
                charEffect = Dialog_CharEffect.None,
                nextIndex = nextIndexValue,
                cutscene = cutscene,
                choices = choices.ToArray(),
                index = parsedIndex
            };

            dialogueList.Add(d);
        }

        JSONArray jsonRows = values.AsArray;
        dialogueSystem.SetDialogue(dialogueList.ToArray());
        dialogueSystem.ShowDialogue();

    }


    private Dialog_CharPos ParseCharPos(string str)
    {
        if (Enum.TryParse(str, out Dialog_CharPos pos))
            return pos;

        if (int.TryParse(str, out int num) && Enum.IsDefined(typeof(Dialog_CharPos), num))
            return (Dialog_CharPos)num;

        return Dialog_CharPos.None;
    }

    private T ParseEnum<T>(string str) where T : struct
    {
        return Enum.TryParse(str, out T result) ? result : default;
    }

    private DialogSE CreateSE(SEType type, string name)
    {
        if (string.IsNullOrEmpty(name)) return null;
        return new DialogSE
        {
            type = type,
            clip = LoadAudioClipByName(name),
            volume = 1f,
            loopCount = 1
        };
    }

    public AudioClip LoadAudioClipByName(string clipName)
    {
        if (string.IsNullOrEmpty(clipName)) return null;
        AudioClip clip = Resources.Load<AudioClip>($"Audio/{clipName}");
        if (clip == null)
            Debug.LogWarning($"AudioClip '{clipName}'를 Resources/Audio 폴더에서 찾을 수 없습니다.");
        return clip;
    }
}
