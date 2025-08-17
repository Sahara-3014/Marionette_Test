using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using SimpleJSON;
using System.Linq;
using UnityEngine.Events;


public class GoogleSheetLoader : MonoBehaviour
{
    public static GoogleSheetLoader Instance { get; private set; }
    public DialogueManager dialogueManager;
    public event System.Action OnSheetLoaded;

    public string apiKey = "AIzaSyCYF6AGzi8Fe0HhVew-t0LOngxs0IOZIuc";
    public string spreadsheetId = "1N2Z-yXGz8rUvUBwLfkeB9GYIOWhMrfs6lWok9lcNIjk";
    public List<string> fixedSheetSequence = new List<string> { "INTRO", "START", "CHAPTER1" };
    public int firstIDOfCurrentSheet;
    private int currentFixedIndex = 0;
    public bool usingBranching = false;
    public Dictionary<string, float> progress;

    private string currentSheet = "";

    public DialogueManager dialogueSystem; // 대화 시스템 참조
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 중복 제거
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴되지 않음
    }
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
        string range = $"{sheetName}!A1:AI400";
        string url = $"https://sheets.googleapis.com/v4/spreadsheets/{spreadsheetId}/values/{range}?key={apiKey}";

        UnityWebRequest www = UnityWebRequest.Get(url);
        var operation = www.SendWebRequest();
        if (progress != null && progress.ContainsKey($"LoadGoogleSheet[{sheetName}]"))
        {
            progress[$"LoadGoogleSheet[{sheetName}]"] = operation.progress;
        }
        else
        {
            if(progress == null)
                progress = new();

            progress.Add($"LoadGoogleSheet[{sheetName}]", operation.progress);
        }

        while (!operation.isDone)
        {
            progress[$"LoadGoogleSheet[{sheetName}]"] = operation.progress;
            yield return null; // 비동기 대기
        }

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("시트 불러오기 실패: " + www.error);
            yield break;
        }
        progress[$"LoadGoogleSheet[{sheetName}]"] = 1f;

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

            int bgmColumnIndex = 23;      // DialogueData 생성자 기준 BGM 열 (예: 23)
            int commandsColumnIndex = 24; // commands 열 (예: 24)

            string bgmCommand = row[bgmColumnIndex].Value.Trim();
            string extraCommand = row[commandsColumnIndex].Value.Trim();
            dialogueList.Add(d);
        }



        // ID별 그룹화 후 index 정렬하여 Dictionary 생성
        var dict = dialogueList
            .GroupBy(d => d.ID)
            .ToDictionary(g => g.Key, g => g.OrderBy(x => x.index).ToArray());

        yield return null; // 기존 데이터 안전하게 가져오기 위해 한 프레임 대기

        // 기존 저장된 대사 가져오기 (없으면 빈 딕셔너리)
        var existingDialogs = SaveDatabase.Instance.GetDialogs() ?? new Dictionary<int, DialogueData[]>();

        // 기존 딕셔너리 복사본 생성 (새로운 객체)
        var combined = new Dictionary<int, DialogueData[]>(existingDialogs);

        // 새로 로드한 대사로 덮어쓰기 (조건에 따라 변경 가능)
        foreach (var kvp in dict)
        {
            combined[kvp.Key] = kvp.Value;
        }

        // 합쳐진 딕셔너리를 저장
        SaveDatabase.Instance.Set_Dialogs(combined);

        // 대화 시스템 딕셔너리 갱신
        dialogueSystem?.RefreshDialogueDict();

        // --- 추가된 부분 끝 ---

        // 시트 이름에 따른 시작 ID 가져오기
        int startID = dialogueList[0].ID;
        firstIDOfCurrentSheet = startID;
        OnSheetLoaded?.Invoke();
        progress.Remove($"LoadGoogleSheet[{sheetName}]"); // 로딩 완료 후 진행률 제거
    }

    async public void LoadInteractiveDebate()
    {
        string range = "CH1_DEBATE1.2!A3:AI100";
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
        if (progress != null && progress.ContainsKey("LoadInteractiveDebate"))
        {
            progress["LoadInteractiveDebate"] = operation.progress;
        }
        else
        {
            if (progress == null)
                progress = new();
            progress.Add("LoadInteractiveDebate", operation.progress);
        }
        while (!operation.isDone)
        {
            progress["LoadInteractiveDebate"] = operation.progress;
            await System.Threading.Tasks.Task.Yield(); // 비동기 대기
        }

        // 요청 결과 확인
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("데이터 불러오기 실패: " + www.error);
            return;
        }
        progress["LoadInteractiveDebate"] = 1f;
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
            }
        }
        else
        // Json 데이터 처리
        {
            var jsonData = JSON.Parse(www.downloadHandler.text);
            var values = jsonData["values"];

            for (int i = 0; i < values.Count; i++)
            {
                var row = values[i];
                InteractiveDebate_DialogueData d = new InteractiveDebate_DialogueData(row);

                list.Add(d);
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
        progress.Remove("LoadInteractiveDebate"); // 로딩 완료 후 진행률 제거
    }

    async public void LoadInvestigate()
    {
        string range = "CH1_Investigate!A3:S175";
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
        if (progress != null && progress.ContainsKey("LoadInvestigate"))
        {
            progress["LoadInvestigate"] = operation.progress;
        }
        else
        {
            if (progress == null)
                progress = new();
            progress.Add("LoadInvestigate", operation.progress);
        }
        while (!operation.isDone)
        {
            progress["LoadInvestigate"] = operation.progress;
            await System.Threading.Tasks.Task.Yield(); // 비동기 대기
        }

        // 요청 결과 확인
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("데이터 불러오기 실패: " + www.error);
            return;
        }
        progress["LoadInvestigate"] = 1f;
        Debug.Log("데이터 성공: " + www.result + "\n" + www.downloadHandler.text);

        List<Investigate_DialogueData> list = new();
        // CSV 데이터 처리
        if (url.Contains("csv"))
        {
            var csvData = www.downloadHandler.text;

            // CSV 데이터를 줄 단위로 분리하고 Investigate_DialogueData 객체로 변환
            string[] lines = csvData.Split('\n');

            // 두번째 줄까지 헤더로 간주하고 건너뜀
            for (int i = 0; i < lines.Length; i++)
            {
                //Debug.LogFormat($"{i} : {lines[i]}");
                string[] columns = lines[i].Split(',');
                if (columns[0].Trim() == string.Empty || columns[0].Trim() == "")
                    continue; // 빈 줄 건너뛰기
                Investigate_DialogueData _data = new(columns);
                list.Add(_data);
            }
        }
        else
        // Json 데이터 처리
        {
            var jsonData = JSON.Parse(www.downloadHandler.text);
            var values = jsonData["values"];

            for (int i = 0; i < values.Count; i++)
            {
                var row = values[i];
                Investigate_DialogueData d = new Investigate_DialogueData(row);

                list.Add(d);
            }
        }

        // ID별로 그룹화하고 index 순서대로 정렬
        Dictionary<int, List<Investigate_DialogueData>> dic = new();
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
        Dictionary<int, Investigate_DialogueData[]> data = new();
        foreach (var kvp in dic)
            data.Add(kvp.Key, kvp.Value.ToArray());

        // 데이터 저장
        SaveDatabase.Instance.Set_InvestigateDialogs(data);
        progress.Remove("LoadInvestigate"); // 로딩 완료 후 진행률 제거
    }

    async public void LoadInvestigate2()
    {
        while(IsInvoking(nameof(LoadInvestigate)))
        {
            await System.Threading.Tasks.Task.Yield();
        }

        string range = "CH1_Investigate_2!A3:S175";
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
        if (progress != null && progress.ContainsKey("LoadInvestigate2"))
        {
            progress["LoadInvestigate2"] = operation.progress;
        }
        else
        {
            if (progress == null)
                progress = new();
            progress.Add("LoadInvestigate2", operation.progress);
        }
        while (!operation.isDone)
        {
            progress["LoadInvestigate2"] = operation.progress;
            await System.Threading.Tasks.Task.Yield(); // 비동기 대기
        }

        // 요청 결과 확인
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("데이터 불러오기 실패: " + www.error);
            return;
        }
        progress["LoadInvestigate2"] = 1f;
        Debug.Log("데이터 성공: " + www.result + "\n" + www.downloadHandler.text);

        List<Investigate_DialogueData> list = new();
        // CSV 데이터 처리
        if (url.Contains("csv"))
        {
            var csvData = www.downloadHandler.text;

            // CSV 데이터를 줄 단위로 분리하고 Investigate_DialogueData 객체로 변환
            string[] lines = csvData.Split('\n');

            // 두번째 줄까지 헤더로 간주하고 건너뜀
            for (int i = 0; i < lines.Length; i++)
            {
                //Debug.LogFormat($"{i} : {lines[i]}");
                string[] columns = lines[i].Split(',');
                if (columns[0].Trim() == string.Empty || columns[0].Trim() == "")
                    continue; // 빈 줄 건너뛰기
                Investigate_DialogueData _data = new(columns);
                list.Add(_data);
            }
        }
        else
        // Json 데이터 처리
        {
            var jsonData = JSON.Parse(www.downloadHandler.text);
            var values = jsonData["values"];

            for (int i = 0; i < values.Count; i++)
            {
                var row = values[i];
                Investigate_DialogueData d = new Investigate_DialogueData(row);

                list.Add(d);
            }
        }

        // ID별로 그룹화하고 index 순서대로 정렬
        Dictionary<int, List<Investigate_DialogueData>> dic = new();
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
        Dictionary<int, Investigate_DialogueData[]> data = new();
        foreach (var kvp in dic)
            data.Add(kvp.Key, kvp.Value.ToArray());

        // 데이터 저장
        SaveDatabase.Instance.Set_InvestigateDialogs(data);
        progress.Remove("LoadInvestigate2"); // 로딩 완료 후 진행률 제거
    }

    async public void LoadConfrontationDebate()
    {
        string range = "CH1_DEBATE3!A3:AI100";
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
        if (progress != null && progress.ContainsKey("LoadConfrontationDebate"))
        {
            progress["LoadConfrontationDebate"] = operation.progress;
        }
        else
        {
            if (progress == null)
                progress = new();
            progress.Add("LoadConfrontationDebate", operation.progress);
        }
        while (!operation.isDone)
        {
            progress["LoadConfrontationDebate"] = operation.progress;
            await System.Threading.Tasks.Task.Yield(); // 비동기 대기
        }

        // 요청 결과 확인
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("데이터 불러오기 실패: " + www.error);
            return;
        }
        progress["LoadConfrontationDebate"] = 1f;
        Debug.Log("데이터 성공: " + www.result + "\n" + www.downloadHandler.text);

        List<ConfrontationDebate_DialogueData> list = new();
        // CSV 데이터 처리
        if (url.Contains("csv"))
        {
            var csvData = www.downloadHandler.text;

            // CSV 데이터를 줄 단위로 분리하고 ConfrontationDebate_DialogueData 객체로 변환
            string[] lines = csvData.Split('\n');

            // 두번째 줄까지 헤더로 간주하고 건너뜀
            for (int i = 0; i < lines.Length; i++)
            {
                //Debug.LogFormat($"{i} : {lines[i]}");
                string[] columns = lines[i].Split(',');
                if (columns[0].Trim() == string.Empty || columns[0].Trim() == "")
                    continue; // 빈 줄 건너뛰기
                ConfrontationDebate_DialogueData _data = new(columns);
                list.Add(_data);
            }
        }
        else
        // Json 데이터 처리
        {
            var jsonData = JSON.Parse(www.downloadHandler.text);
            var values = jsonData["values"];

            for (int i = 0; i < values.Count; i++)
            {
                var row = values[i];
                ConfrontationDebate_DialogueData d = new ConfrontationDebate_DialogueData(row);

                list.Add(d);
            }
        }

        // ID별로 그룹화하고 index 순서대로 정렬
        Dictionary<int, List<ConfrontationDebate_DialogueData>> dic = new();
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
        Dictionary<int, ConfrontationDebate_DialogueData[]> data = new();
        foreach (var kvp in dic)
            data.Add(kvp.Key, kvp.Value.ToArray());

        // 데이터 저장
        SaveDatabase.Instance.Set_ConfrontationDebateDialogs(data);
        progress.Remove("LoadConfrontationDebate"); // 로딩 완료 후 진행률 제거
    }

}
