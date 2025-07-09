using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;

public class GoogleSheetLoader : MonoBehaviour
{
    public string apiKey = "AIzaSyCYF6AGzi8Fe0HhVew-t0LOngxs0IOZIuc";
    public string spreadsheetId = "1N2Z-yXGz8rUvUBwLfkeB9GYIOWhMrfs6lWok9lcNIjk";
    public string sheetName = "INTRO";

    public Test dialogueSystem; // 대화 시스템 참조

    void Start()
    {
        // StartCoroutine(LoadGoogleSheet()); // 삭제 또는 주석 처리
    }

    // 외부에서 호출할 수 있게 public 메서드로 만듦
    public void LoadDialoguesFromSheet()
    {
        StartCoroutine(LoadGoogleSheet());
    }

    IEnumerator LoadGoogleSheet()
    {
        string range = $"{sheetName}!A1:K100"; // K열까지 포함해서 가져오기
        string url = $"https://sheets.googleapis.com/v4/spreadsheets/{spreadsheetId}/values/{range}?key={apiKey}";

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("시트 불러오기 실패: " + www.error);
        }
        else
        {
            List<Dialogue> dialogueList = new List<Dialogue>();
            var json = JSON.Parse(www.downloadHandler.text);
            var values = json["values"];

            for (int i = 2; i < values.Count; i++) // i=1: 헤더 생략
            {
                var row = values[i];
                string dialogueText = row.Count > 10 ? row[10] : ""; // K열 (DIALOGUE)

                Debug.Log($"[{i}] 대사: {dialogueText}");

                Dialogue d = new Dialogue
                {
                    dialogue = dialogueText,
                    cg = null // 나중에 이미지 열 추가하고 싶다면 여기에 넣기
                };

                dialogueList.Add(d);
            }

            dialogueSystem.SetDialogue(dialogueList.ToArray());
            dialogueSystem.ShowDialogue();
        }
    }
}
