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

    public Test dialogueSystem; // ��ȭ �ý��� ����

    void Start()
    {
        // StartCoroutine(LoadGoogleSheet()); // ���� �Ǵ� �ּ� ó��
    }

    // �ܺο��� ȣ���� �� �ְ� public �޼���� ����
    public void LoadDialoguesFromSheet()
    {
        StartCoroutine(LoadGoogleSheet());
    }

    IEnumerator LoadGoogleSheet()
    {
        string range = $"{sheetName}!A1:K100"; // K������ �����ؼ� ��������
        string url = $"https://sheets.googleapis.com/v4/spreadsheets/{spreadsheetId}/values/{range}?key={apiKey}";

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("��Ʈ �ҷ����� ����: " + www.error);
        }
        else
        {
            List<Dialogue> dialogueList = new List<Dialogue>();
            var json = JSON.Parse(www.downloadHandler.text);
            var values = json["values"];
            for (int i = 2; i < values.Count; i++)
            {
                var row = values[i];

                string background = (row.Count > 4 && row[4] != null) ? row[4].Value.Trim() : "";
                string characterName = (row.Count > 8 && row[8] != null) ? row[8].Value.Trim() : "";
                string status = (row.Count > 9 && row[9] != null) ? row[9].Value.Trim() : "";
                string dialogueText = (row.Count > 10 && row[10] != null) ? row[10].Value.Trim() : "";

                Debug.Log($"[{i}] {characterName} / {status} / {background} : {dialogueText}");

                Dialogue d = new Dialogue
                {
                    characterName = characterName,
                    status = status,
                    background = background,
                    dialogue = dialogueText,
                    cg = null
                };

                dialogueList.Add(d);
            }



            dialogueSystem.SetDialogue(dialogueList.ToArray());
            dialogueSystem.ShowDialogue();
        }
    }
}
