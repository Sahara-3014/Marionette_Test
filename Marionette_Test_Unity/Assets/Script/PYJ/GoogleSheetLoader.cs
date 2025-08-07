using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Linq;

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

        yield return null; // 한 프레임 대기

        // 시트 이름에 따른 시작 ID 가져오기
        int startID = GetStartIDFromSheetName(sheetName);

        // 시작 ID, 인덱스 1부터 대사 보여주기
        dialogueSystem.ShowDialogue(startID, 1);

        // NextDialogue 호출해서 대사 진행
        dialogueSystem.NextDialogue();

    }

    private int GetStartIDFromSheetName(string sheetName)
    {
        switch (sheetName)
        {
            case "INTRO": return 1000;
            case "START": return 2000;
            case "CHAPTER1": return 3000;
            // 필요하면 추가
            default: return 1000; // 기본값
        }
    }



    public AudioClip LoadAudioClipByName(string clipName)
    {
        if (string.IsNullOrEmpty(clipName)) return null;
        AudioClip clip = Resources.Load<AudioClip>($"Audio/{clipName}");
        if (clip == null)
            Debug.LogWarning($"AudioClip '{clipName}'를 Resources/Audio 폴더에서 찾을 수 없습니다.");
        return clip;
    }

    async public void LoadInteractiveDebate()
    {
        string range = "DEBATE1.2!A3:AI100";
        // Google Sheets URL 설정
        string url = //$"https://docs.google.com/spreadsheets/d/{spreadsheetId}/export?format=csv&id={spreadsheetId}";
            $"https://sheets.googleapis.com/v4/spreadsheets/{spreadsheetId}/values/{range}?key={apiKey}";

        if (url.Contains("csv"))
        {
            // 파라미터 설정
            url += "&gid=935233836"; // 시트 ID (gid) 설정
            url += "&range=A3:AI100"; // 데이터 범위 설정
        }


        // Google Sheets 데이터 불러오기
        UnityWebRequest www = UnityWebRequest.Get(url);
        var operation = www.SendWebRequest();
        while (!operation.isDone)
        {
            await System.Threading.Tasks.Task.Yield(); // 비동기 대기
        }

        // 요청 결과 확인
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("데이터 불러오기 실패: " + www.error);
            return;
        }
        Debug.Log("데이터 성공: " + www.result + "\n" + www.downloadHandler.text);

        List<InteractiveDebate_DialogueData> list = new();
        // CSV 데이터 처리
        if (url.Contains("csv"))
        {
            var csvData = www.downloadHandler.text;

            // CSV 데이터를 줄 단위로 분리하고 InteractiveDebate_DialogueData 객체로 변환
            string[] lines = csvData.Split('\n');


            // 두번째 줄까지 헤더로 간주하고 건너뜀
            for (int i = 0; i < lines.Length; i++)
            {
                //Debug.LogFormat($"{i} : {lines[i]}");
                string[] columns = lines[i].Split(',');
                if (columns[0].Trim() == string.Empty || columns[0].Trim() == "")
                    continue; // 빈 줄 건너뛰기
                InteractiveDebate_DialogueData _data = new(columns);
                list.Add(_data);
                if (_data.NEXT_ID == -100)
                    break;
            }
        }
        else
        // Json 데이터 처리
        {
            var jsonData = JSON.Parse(www.downloadHandler.text);
            var values = jsonData["values"];

            for (int i = 2; i < values.Count; i++)
            {
                var row = values[i];
                InteractiveDebate_DialogueData d = new InteractiveDebate_DialogueData(row);

                list.Add(d);
                if (d.NEXT_ID == -100)
                    break; // NEXT_ID가 -100인 경우 대화 종료
            }
        }

        // ID별로 그룹화하고 index 순서대로 정렬
        Dictionary<int, List<InteractiveDebate_DialogueData>> dic = new();
        foreach (var item in list)
        {
            if (!dic.ContainsKey(item.ID))
            {
                dic.Add(item.ID, new() { item });
            }
            else
            {
                dic[item.ID].Add(item);
                dic[item.ID].OrderBy(x => x.INDEX);
            }
        }

        // SaveDatabase 에 저장할 데이터 구조로 변환
        Dictionary<int, InteractiveDebate_DialogueData[]> data = new();
        foreach (var kvp in dic)
            data.Add(kvp.Key, kvp.Value.ToArray());

        // 데이터 저장
        SaveDatabase.Instance.Set_InteractiveDebateDialogs(data);
    }
}