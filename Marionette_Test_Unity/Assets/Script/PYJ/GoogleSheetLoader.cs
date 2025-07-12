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

    public DialogueManager dialogueSystem; // ��ȭ �ý��� ����

    // ���� ��Ʈ �ҷ����� ȣ��� �Լ�
    public void LoadDialoguesFromSheet()
    {
        StartCoroutine(LoadGoogleSheet());
    }

    IEnumerator LoadGoogleSheet()
    {
        string range = $"{sheetName}!A1:L100"; // L������ Ȯ�� (bgm �÷� ����)
        string url = $"https://sheets.googleapis.com/v4/spreadsheets/{spreadsheetId}/values/{range}?key={apiKey}";

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("��Ʈ �ҷ����� ����: " + www.error);
            yield break;
        }

        List<Dialogue> dialogueList = new List<Dialogue>();
        var json = JSON.Parse(www.downloadHandler.text);
        var values = json["values"];

        for (int i = 2; i < values.Count; i++) // 0:���, 1:���� �� ��ŵ
        {
            var row = values[i];

            string background = (row.Count > 4 && row[4] != null) ? row[4].Value.Trim() : "";
            string characterName = (row.Count > 8 && row[8] != null) ? row[8].Value.Trim() : "";
            string status = (row.Count > 9 && row[9] != null) ? row[9].Value.Trim() : "";
            string dialogueText = (row.Count > 10 && row[10] != null) ? row[10].Value.Trim() : "";

            string bgmName = (row.Count > 11 && row[11] != null) ? row[11].Value.Trim() : "";
            string sfx1Name = (row.Count > 12 && row[12] != null) ? row[12].Value.Trim() : "";
            string sfx2Name = (row.Count > 13 && row[13] != null) ? row[13].Value.Trim() : "";

            // BGM ó��
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

            // SFX1 ó��
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

            // SFX2 ó��
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

            Dialogue d = new Dialogue
            {
                characterName = characterName,
                status = status,
                background = background,
                dialogue = dialogueText,
                cg = null,

                // �߰�
                bgm = bgmSE,
                se1 = sfx1SE,
                se2 = sfx2SE
            };

            dialogueList.Add(d);
        }

        dialogueSystem.SetDialogue(dialogueList.ToArray());
        dialogueSystem.ShowDialogue();
    }

    // Resources/Audio ���� �� �����Ŭ�� �ε� �Լ�
    public AudioClip LoadAudioClipByName(string clipName)
    {
        if (string.IsNullOrEmpty(clipName))
            return null;

        AudioClip clip = Resources.Load<AudioClip>($"Audio/{clipName}");
        if (clip == null)
        {
            Debug.LogWarning($"AudioClip '{clipName}'�� Resources/Audio �������� ã�� �� �����ϴ�.");
        }
        return clip;
    }
}
