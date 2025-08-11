using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;

public class SaveDatabase : MonoBehaviour
{
    public static SaveDatabase Instance { get; private set; }
    private DayCycleSystem dayCycleSystem;

    private SaveData saveData;
    /// <summary> 저장시 인덱스 </summary>
    public int savePlayIndex { get; private set; }
    private Dictionary<string, UnityAction> sceneChangeEvent = new();
    private Dictionary<int, DialogueData[]> dialogs;
    private Dictionary<int, InteractiveDebate_DialogueData[]> interactiveDebateDialogs;
    private Dictionary<int, ConfrontationDebate_DialogueData[]> confrontationDebateDialogs;


    #region 씬 이동 이벤트
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        SceneManager.sceneLoaded += OnSceneLoaded; 
        Initalize();
    }

    private void Start()
    {
        if(dayCycleSystem == null)
            dayCycleSystem = DayCycleSystem.Instance;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (sceneChangeEvent != null && sceneChangeEvent.ContainsKey(scene.name))
        {
            sceneChangeEvent[scene.name]?.Invoke();
            sceneChangeEvent[scene.name] = null; // 이벤트 호출 후 제거
        }

    }

    public void ChangeScene(string sceneName, LoadSceneMode mode = LoadSceneMode.Single)
    => SceneManager.LoadScene(sceneName, mode);

    public void ChangeScene(int sceneIndex, LoadSceneMode mode = LoadSceneMode.Single)
    => SceneManager.LoadScene(sceneIndex, mode);

    public void AddSceneChangeEvent(string sceneName, UnityAction action)
    {
        if (sceneChangeEvent == null)
            sceneChangeEvent = new();
        if (sceneChangeEvent.ContainsKey(sceneName))
            sceneChangeEvent[sceneName] += action;
        else
            sceneChangeEvent.Add(sceneName, action);
    }

    private void Initalize()
    {
        dayCycleSystem?.RigisterDayChangeEvent(SaveData_SetDayNum);
        dayCycleSystem?.RigisterTimeChangeEvent(SaveData_SetGameTime);
    }

    #endregion


    #region 대화
    public DialogueData[] Get_Dialogs_NeedID(int id)
    {
        if(dialogs == null)
        {
            string text = TextLoad("Dialog");
            if (text == null)
                return null;

            var dic = JsonConvert.DeserializeObject<Dictionary<int, DialogueData[]>>(text);

            dialogs = dic;
        }

        if (dialogs.ContainsKey(id))
            return dialogs[id];

        else
            return null;
    }

    public void Add_Dialogs_NeedID(int id, DialogueData[] data)
    {
        if(dialogs == null)
            dialogs = new();

        if (dialogs.ContainsKey(id) == false)
            dialogs.Add(id, data);
        else
            dialogs[id] = data;
    }

    public void Set_Dialogs(Dictionary<int, DialogueData[]> dialogs)
    {
        if(this.dialogs != null)
        {
            foreach(var dialog in dialogs)
            {
                if (this.dialogs.ContainsKey(dialog.Key) == false)
                    this.dialogs.Add(dialog.Key, dialog.Value);
                else
                    this.dialogs[dialog.Key] = dialog.Value;
            }
        }
        else
        {
            this.dialogs = dialogs;
        }

        TextSave("Dialog.json", JsonConvert.SerializeObject(this.dialogs));
    }

    public Dictionary<int, DialogueData[]> GetDialogs() => dialogs;
    #endregion

    #region 논쟁(대화+선택지)
    public InteractiveDebate_DialogueData[] Get_DebateDialogs_NeedID(int id)
    {
        if (interactiveDebateDialogs == null)
        {
            string text = TextLoad("Dialog_InteractiveDebate");
            if (text == null)
                return null;

            var dic = JsonConvert.DeserializeObject<Dictionary<int, InteractiveDebate_DialogueData[]>>(text);

            interactiveDebateDialogs = dic;
        }

        if (interactiveDebateDialogs.ContainsKey(id))
            return interactiveDebateDialogs[id];

        else
            return null;
    }

    public void Add_InteractiveDebateDialogs_NeedID(int id, InteractiveDebate_DialogueData[] data)
    {
        if (interactiveDebateDialogs == null)
            interactiveDebateDialogs = new();

        if (interactiveDebateDialogs.ContainsKey(id) == false)
            interactiveDebateDialogs.Add(id, data);
        else
            interactiveDebateDialogs[id] = data;
    }

    public void Set_InteractiveDebateDialogs(Dictionary<int, InteractiveDebate_DialogueData[]> dialogs)
    {
        if (this.interactiveDebateDialogs != null)
        {
            foreach (var dialog in dialogs)
            {
                if (this.interactiveDebateDialogs.ContainsKey(dialog.Key) == false)
                    dialogs.Add(dialog.Key, dialog.Value);
                else
                    dialogs[dialog.Key] = dialog.Value;
            }
        }
        else
        {
            this.interactiveDebateDialogs = dialogs;
        }

        TextSave("Dialog_InteractiveDebate.json", JsonConvert.SerializeObject(this.interactiveDebateDialogs));
    }

    public ConfrontationDebate_DialogueData[] Get_ConfrontationDebateDialogs_NeedID(int id)
    {
        if (confrontationDebateDialogs == null)
        {
            string text = TextLoad("Dialog_InteractiveDebate");
            if (text == null)
                return null;

            var dic = JsonConvert.DeserializeObject<Dictionary<int, ConfrontationDebate_DialogueData[]>>(text);

            confrontationDebateDialogs = dic;
        }

        if (confrontationDebateDialogs.ContainsKey(id))
            return confrontationDebateDialogs[id];

        else
            return null;
    }

    public void Add_ConfrontationDebateDialogs_NeedID(int id, ConfrontationDebate_DialogueData[] data)
    {
        if (confrontationDebateDialogs == null)
            confrontationDebateDialogs = new();

        if (confrontationDebateDialogs.ContainsKey(id) == false)
            confrontationDebateDialogs.Add(id, data);
        else
            confrontationDebateDialogs[id] = data;
    }

    public void Set_ConfrontationDebateDialogs(Dictionary<int, ConfrontationDebate_DialogueData[]> dialogs)
    {
        if (this.confrontationDebateDialogs != null)
        {
            foreach (var dialog in dialogs)
            {
                if (this.confrontationDebateDialogs.ContainsKey(dialog.Key) == false)
                    dialogs.Add(dialog.Key, dialog.Value);
                else
                    dialogs[dialog.Key] = dialog.Value;
            }
        }
        else
        {
            this.confrontationDebateDialogs = dialogs;
        }

        TextSave("Dialog_ConfrontationDebate.json", JsonConvert.SerializeObject(this.confrontationDebateDialogs));
    }
    #endregion


    #region SaveData 관련 변수 메서드화
    public SaveData SaveData_Get() => saveData;
    public int SaveData_GetIndex() => saveData.index;

    public int SaveData_GetDayNum()
    {
        int day = dayCycleSystem.GetDays();
        saveData.dayNum = day;
        return day;
    }
    public void SaveData_SetDayNum(int dayNum) => saveData.dayNum = dayNum;

    public float SaveData_GetGameTime()
    {
        float time = dayCycleSystem.GetTimes();
        saveData.gameTime = time;
        return time;
    }
    public void SaveData_SetGameTime(float time) => saveData.gameTime = time;

    public void SaveData_AddReadDialog(string key, int id)
    {
        if (saveData.localPlayerData.readDialogs == null)
            saveData.localPlayerData.readDialogs = new Dictionary<string, List<int>>();
        if (saveData.localPlayerData.readDialogs.ContainsKey(key) == false)
            saveData.localPlayerData.readDialogs.Add(key, new() { id });
        else
            saveData.localPlayerData.readDialogs[key].Add(id);
    }
    public Dictionary<string, List<int>> SaveData_GetReadDialog() => saveData.localPlayerData.readDialogs;
    public List<int> SaveData_GetReadDialog_NeedKey(string key) => saveData.localPlayerData.readDialogs[key];

    public void SaveData_AddSelectDialog(string key, int id)
    {
        if (saveData.localPlayerData.selectDialogs == null)
            saveData.localPlayerData.selectDialogs = new Dictionary<string, List<int>>();
        if (saveData.localPlayerData.selectDialogs.ContainsKey(key) == false)
            saveData.localPlayerData.selectDialogs.Add(key, new() { id });
        else
            saveData.localPlayerData.selectDialogs[key].Add(id);
    }
    public Dictionary<string, List<int>> SaveData_GetSelectDialog() => saveData.localPlayerData.selectDialogs;
    public List<int> SaveData_GetSelectDialog_NeedKey(string key) => saveData.localPlayerData.selectDialogs[key];

    public int SaveData_GetNowDialogID() => saveData.localPlayerData.nowDialog_id;
    public void SaveData_SetNowDialogID(int id) => saveData.localPlayerData.nowDialog_id = id;

    public int SaveData_GetNowDialogIndex() => saveData.localPlayerData.nowDialog_index;
    public void SaveData_SetNowDialogIndex(int index) => saveData.localPlayerData.nowDialog_index = index;

    public int SaveData_GetPosType() => saveData.localPlayerData.posType;
    public void SaveData_SetPosType(int type) => saveData.localPlayerData.posType = type;

    public Vector2 SaveData_GetPosition() => saveData.localPlayerData.position;
    public void SaveData_SetPosition(Vector2 pos) => saveData.localPlayerData.position = pos;

    public Dictionary<int, int> SaveData_GetItems() => saveData.localPlayerData.itmes;
    public void SaveData_SetItems(Dictionary<int, int> items) => saveData.localPlayerData.itmes = items;
    
    public Dictionary<string, CharAttributeData> SaveData_GetCharsData() => saveData.charsData;
    public CharAttributeData SaveData_GetCharData(string key)
    {
        if (saveData.charsData == null || saveData.charsData.ContainsKey(key) == false)
            return null;
        return saveData.charsData[key];
    }
    public GaugeInt SaveData_GetCharData_GetGauge(string key, CharAttributeData.CharAttributeType gauge)
    {
        if (saveData.charsData == null || saveData.charsData.ContainsKey(key) == false)
            return new();
        CharAttributeData charData = saveData.charsData[key];

        if(gauge == CharAttributeData.CharAttributeType.TRUST || gauge == CharAttributeData.CharAttributeType.SUSPICION)
            return charData.trust;
        else if(gauge == CharAttributeData.CharAttributeType.MENTAL || gauge == CharAttributeData.CharAttributeType.LIKE)
            return charData.mental;
        else
            return new GaugeInt(); // 기본값 반환
    }
    public void SaveData_SetCharData(string key, CharAttributeData data)
    {
        if (saveData.charsData == null)
            saveData.charsData = new Dictionary<string, CharAttributeData>();
        if (saveData.charsData.ContainsKey(key) == false)
            saveData.charsData.Add(key, data);
        else
            saveData.charsData[key] = data;
    }
    public void SaveData_SetCharData_SetGauge(string key, CharAttributeData.CharAttributeType gauge, int value)
    {
        if (saveData.charsData == null)
            saveData.charsData = new Dictionary<string, CharAttributeData>();
        if (saveData.charsData.ContainsKey(key) == false)
            saveData.charsData.Add(key, new CharAttributeData());
        CharAttributeData charData = saveData.charsData[key];

        if (gauge == CharAttributeData.CharAttributeType.TRUST || gauge == CharAttributeData.CharAttributeType.SUSPICION)
            charData.trust.value = value;
        else if (gauge == CharAttributeData.CharAttributeType.MENTAL || gauge == CharAttributeData.CharAttributeType.LIKE)
            charData.mental.value = value;

        saveData.charsData[key] = charData;
    }

    #endregion


    #region 내부 저장

    public void AutoSave(UnityAction _callback = null)
    {
        saveData.saveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        PlayerPrefs.SetString($"SaveData_Auto", JsonConvert.SerializeObject(saveData));
        _callback?.Invoke();
    }

    public SaveData AutoLoad()
    {
        string str = PlayerPrefs.GetString($"SaveData_Auto", null);
        if(str != null)
        {
            try
            {
                var data = JsonConvert.DeserializeObject<SaveData>(str);
                return data;
            }
            catch (Exception e)
            {
                return new SaveData { index = -1 }; // 자동 저장이 잘못된 경우
            }

        }
        else
        {
            return new SaveData { index = -1 }; // 자동 저장이 없을 경우
        }
    }

    public void Save(int _index, UnityAction _callback = null)
    {
        savePlayIndex = _index;
        saveData.index = _index;
        saveData.saveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        PlayerPrefs.SetString($"SaveData_{_index}", JsonConvert.SerializeObject(saveData));
        _callback?.Invoke();
    }

    public SaveData Load(int _index, bool isSet = true)
    {
        savePlayIndex = _index;
        string data = PlayerPrefs.GetString($"SaveData_{_index}", null);
        if (data != null)
        {
            try
            {
                var _data = JsonConvert.DeserializeObject<SaveData>(data);
                if (isSet)
                {
                    saveData = _data;
                    return saveData;
                }
                else
                    return _data;
            }
            catch (Exception e)
            {
                return new SaveData { index = -1 }; // 잘못된 저장 데이터
            }
        }
        else
            return new SaveData { index = -1 };
    }


    /// <summary> Resources 폴더에 저장 </summary>
    /// <param name="key"> 확장자 필요 </param>
    /// <param name="value">Dictionary 또는 List인경우 JsonConvert, 나머지는 JsonUtillity</param>
    public void TextSave(string key, string value)
    {
        string path = Application.dataPath;
        File.WriteAllText($"{path}/Resources/{key}", value);
    }

    /// <summary> Resources 폴더에서 가져오기 </summary>
    /// <param name="key"></param> 
    public string TextLoad(string key)
    {
         return Resources.Load<TextAsset>(key)?.text;
    }
    #endregion

    public IEnumerator AfterAction(UnityAction action, float delay = 0f)
    {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }
}


/// <summary> 선택한 슬롯의 게임 데이터 </summary>
[Serializable]
public struct SaveData
{
    /// <summary> 저장 슬롯 </summary>
    public int index;
    /// <summary> 저장한 실제시간 </summary>
    public string saveDate;
    /// <summary> 인게임 N일차 </summary>
    public int dayNum;
    /// <summary> 인게임 시간 </summary>
    public float gameTime;
    /// <summary> 내 현재 플레이 데이터 </summary>
    public LocalPlayerData localPlayerData;
    /// <summary> 등장인물 데이터 </summary>
    public Dictionary<string, CharAttributeData> charsData;

    public SaveData(int index, int dayNum, string nowDate, float gameTime, 
        LocalPlayerData playData, Dictionary<string, CharAttributeData> charsData)
    {
        this.index = index;
        this.saveDate = nowDate;
        this.dayNum = dayNum;
        this.gameTime = gameTime;
        this.localPlayerData = playData;
        this.charsData = charsData;
    }
}

public struct LocalPlayerData
{
    /// <summary> 현재 대화 ID </summary>
    public int nowDialog_id;
    /// <summary> 현재 대화 인덱스 </summary>
    public int nowDialog_index;
    /// <summary> 현재 포지션 타입 0: 탐색, 1: 대화, 2: 논쟁 3:투표, 3:순찰, 4:휴식 </summary>
    public int posType;
    /// <summary> 캐릭터 위치 </summary>
    public Vector2 position;
    /// <summary> 인벤토리 아이템 </summary>
    public Dictionary<int, int> itmes;
    /// <summary> 읽은 대화 ID 목록 </summary>
    public Dictionary<string, List<int>> readDialogs;
    /// <summary> 선택지 목록 </summary>
    public Dictionary<string, List<int>> selectDialogs;

    public LocalPlayerData(int nowDialog_id, int nowDialog_index, int posType, Vector2 position,
        Dictionary<int, int> itmes, Dictionary<string, List<int>> readDialogs,
        Dictionary<string, List<int>> selectDialogs)
    {
        this.nowDialog_id = nowDialog_id;
        this.nowDialog_index = nowDialog_index;
        this.posType = posType;
        this.position = position;
        this.itmes = itmes;
        this.readDialogs = readDialogs;
        this.selectDialogs = selectDialogs;
    }
}

[Serializable]
public class CharAttributeData
{
    public enum CharAttributeType
    {
        TRUST = 1,       // 신뢰도
        SUSPICION = 3,   // 의심도
        MENTAL = 0,      // 정신력
        LIKE = 2         // 호감도
    }
    /// <summary> 신뢰도 </summary>
    public GaugeInt trust = new();
    /// <summary> 정신력 </summary>
    public GaugeInt mental = new();
}

public struct GaugeFloat
{
    public float value;
    public float maxValue;

    public GaugeFloat(float value = 10f, float maxValue = 100f)
    {
        this.value = value;
        this.maxValue = maxValue;
    }
}

public struct GaugeInt
{
    public int value;
    public int maxValue;

    public GaugeInt(int value = 10, int maxValue = 100)
    {
        this.value = value;
        this.maxValue = maxValue;
    }
}

/// <summary> 내부적으로 가지고있어야하는 게임데이터 </summary>
[Serializable]
public struct GlobalData
{
    /// <summary> 엔딩본 횟수 </summary>
    public int clearCount;
    /// <summary> 시청한 엔딩 </summary>
    public List<int> watchedEnding;
}