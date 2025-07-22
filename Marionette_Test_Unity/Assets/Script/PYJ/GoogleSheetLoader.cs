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


        if (www.downloadHandler.text.StartsWith("{") == false)
        {
            string csv = www.downloadHandler.text;
            string[] column = csv.Split('\n');
            Debug.Log(nameof(column) + $" - CSV 길이: {column.Length}");
            for (int i = 0; i < column.Length; i++)
            {
                string[] row = column[i].Split(',');
                Debug.Log(nameof(row) + $"[{i}] 길이: {row.Length} - 내용: {column[i]}");

                DialogueData d = new DialogueData(row);  // ← row를 직접 처리하는 생성자가 정의되어 있어야 함
                dialogueList.Add(d);
            }
        }
        else
        {
            var json = JSON.Parse(www.downloadHandler.text);
            var values = json["values"];

            for (int i = 2; i < values.Count; i++) // 0:헤더, 1:설명 등 스킵
            {
                var row = values[i];

                DialogueData d = new DialogueData(row);  // ← row를 직접 처리하는 생성자가 정의되어 있어야 함
                dialogueList.Add(d);
            }
        }



        dialogueSystem.SetDialogue(dialogueList.ToArray());
        dialogueSystem.ShowDialogue();

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
