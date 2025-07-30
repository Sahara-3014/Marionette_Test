using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class GoogleSheetLoader : MonoBehaviour
{
    public DialogueManager dialogueManager;

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
                // 필요시 대화 종료 처리 추가
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
        string range = $"{sheetName}!A1:AI100";
        string url = $"https://sheets.googleapis.com/v4/spreadsheets/{spreadsheetId}/values/{range}?key={apiKey}";

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("시트 불러오기 실패: " + www.error);
            yield break;
        }

        var json = JSON.Parse(www.downloadHandler.text);
        var values = json["values"];

        if (values == null || values.Count < 3)
        {
            Debug.LogWarning($"시트 '{sheetName}' 데이터가 충분하지 않습니다.");
            yield break;
        }

        List<DialogueData> dialogueList = new List<DialogueData>();

        for (int i = 2; i < values.Count; i++)
        {
            var row = values[i];
            DialogueData d = new DialogueData(row);

            Debug.Log($"로드된 대사: ID={d.ID}, index={d.index}, 대사='{d.dialogue}'");

            dialogueList.Add(d);
        }

        dialogueSystem.SetDialogue(dialogueList.ToArray());

        // ShowDialogue 및 NextDialogue 호출을 코루틴 끝에 넣거나 약간 지연해서 호출
        yield return null; // 한 프레임 대기

        dialogueSystem.ShowDialogue(1000, 1);

        // NextDialogue는 ShowDialogue 내부 또는 UI가 준비된 후 호출하는 게 좋음
        dialogueSystem.NextDialogue();
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
