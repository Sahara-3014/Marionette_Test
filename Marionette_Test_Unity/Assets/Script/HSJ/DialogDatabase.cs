using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class DialogDatabase : MonoBehaviour
{
    public static DialogDatabase Instance { get; private set; }
    private DayCycleSystem dayCycleSystem;

    private SaveData saveData;
    /// <summary> ����� �ε��� </summary>
    public int savePlayIndex { get; private set; }
    private Dictionary<string, UnityAction> sceneChangeEvent = new();
    private Dictionary<string, Dictionary<int, List<Dialogue>>> dialogs;


    #region �� �̵� �̺�Ʈ
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
            sceneChangeEvent[scene.name] = null; // �̺�Ʈ ȣ�� �� ����
        }

    }

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
        dayCycleSystem.RigisterDayChangeEvent(SaveData_SetDayNum);
        dayCycleSystem.RigisterTimeChangeEvent(SaveData_SetGameTime);
    }

    #endregion


    #region ��ȭ
    public List<Dialogue> GetDialogs_NeedID(string key, int id)
    {
        if(dialogs == null || dialogs.ContainsKey(key) == false)
        {
            string text = TextLoad(key);
            if (text == null)
                return null;

            var dic = JsonConvert.DeserializeObject<Dictionary<int, List<Dialogue>>>(text);

            if (dialogs == null)
                dialogs = new();

            dialogs.Add(key, dic);

            if(dic.ContainsKey(id) == false)
                return null;
            else
                return dic[id];
        }
        else
        {
            if(dialogs[key].ContainsKey(id))
                return dialogs[key][id];

            else
                return null;
            
        }
    }

    public Dictionary<int, List<Dialogue>> GetDialogs_NeedKey(string key)
    {
        if (dialogs == null || dialogs.ContainsKey(key) == false)
        {
            string text = TextLoad(key);
            if (text == null)
                return null;

            var dic = JsonConvert.DeserializeObject<Dictionary<int, List<Dialogue>>>(text);

            if (dialogs == null)
                dialogs = new();

            dialogs.Add(key, dic);

            return dialogs[key];
        }
        else
        {
            if (dialogs.ContainsKey(key))
                return dialogs[key];

            else
                return null;

        }
    }

    public Dictionary<string, Dictionary<int, List<Dialogue>>> GetDialogs() => dialogs;
    #endregion




    #region SaveData ���� ���� �޼���ȭ
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
        if (saveData.readDialogs == null)
            saveData.readDialogs = new Dictionary<string, List<int>>();
        if (saveData.readDialogs.ContainsKey(key) == false)
            saveData.readDialogs.Add(key, new() { id });
        else
            saveData.readDialogs[key].Add(id);
    }
    public Dictionary<string, List<int>> SaveData_GetReadDialog() => saveData.readDialogs;
    public List<int> SaveData_GetReadDialog_NeedKey(string key) => saveData.readDialogs[key];

    public void SaveData_AddSelectDialog(string key, int id)
    {
        if (saveData.selectDialogs == null)
            saveData.selectDialogs = new Dictionary<string, List<int>>();
        if (saveData.selectDialogs.ContainsKey(key) == false)
            saveData.selectDialogs.Add(key, new() { id });
        else
            saveData.selectDialogs[key].Add(id);
    }
    public Dictionary<string, List<int>> SaveData_GetSelectDialog() => saveData.selectDialogs;
    public List<int> SaveData_GetSelectDialog_NeedKey(string key) => saveData.selectDialogs[key];

    public int SaveData_GetNowDialogID() => saveData.nowDialog_id;
    public void SaveData_SetNowDialogID(int id) => saveData.nowDialog_id = id;

    public int SaveData_GetNowDialogIndex() => saveData.nowDialog_index;
    public void SaveData_SetNowDialogIndex(int index) => saveData.nowDialog_index = index;

    public int SaveData_GetPosType() => saveData.posType;
    public void SaveData_SetPosType(int type) => saveData.posType = type;

    public Vector2 SaveData_GetPosition() => saveData.position;
    public void SaveData_SetPosition(Vector2 pos) => saveData.position = pos;

    public Dictionary<int, int> SaveData_GetItems() => saveData.itmes;
    public void SaveData_SetItems(Dictionary<int, int> items) => saveData.itmes = items;
    #endregion


    #region ���� ����

    public void AutoSave(UnityAction _callback = null)
    {
        saveData.saveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        PlayerPrefs.SetString($"SaveData_Auto", JsonConvert.SerializeObject(saveData));
        _callback?.Invoke();
    }

    public void Save(int _index, UnityAction _callback = null)
    {
        savePlayIndex = _index;
        saveData.index = _index;
        saveData.saveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        PlayerPrefs.SetString($"SaveData_{_index}", JsonConvert.SerializeObject(saveData));
        _callback?.Invoke();
    }

    public void Load(int _index, UnityAction<SaveData> _callback = null)
    {
        savePlayIndex = _index;
        string data = PlayerPrefs.GetString($"SaveData_{_index}", null);
        if (data != null)
        {
            saveData = JsonConvert.DeserializeObject<SaveData>(data);
            _callback?.Invoke(saveData);
        }
        else
            _callback?.Invoke(new SaveData { index = -1 });
    }

    /// <summary> Resources ������ ���� </summary>
    /// <param name="key"> Ȯ���� �ʿ� </param>
    /// <param name="value">Dictionary �Ǵ� List�ΰ�� JsonConvert, �������� JsonUtillity</param>
    public void TextSave(string key, string value)
    {
        string path = Application.dataPath;
        File.WriteAllText($"{path}/Resources/{key}", value);
    }

    /// <summary> Resources �������� �������� </summary>
    /// <param name="key"></param> 
    public string TextLoad(string key)
    {
         return Resources.Load<TextAsset>(key)?.text;
    }
    #endregion
}


/// <summary> ������ ������ ���� ������ </summary>
[Serializable]
public struct SaveData
{
    /// <summary> ���� ���� </summary>
    public int index;
    /// <summary> ������ �����ð� </summary>
    public string saveDate;
    /// <summary> �ΰ��� N���� </summary>
    public int dayNum;
    /// <summary> �ΰ��� �ð� </summary>
    public float gameTime;
    /// <summary> ���� ��ȭ ID ��� </summary>
    public Dictionary<string, List<int>> readDialogs;
    /// <summary> ������ ��� </summary>
    public Dictionary<string, List<int>> selectDialogs;
    /// <summary> ���� ��ȭ ID </summary>
    public int nowDialog_id;
    /// <summary> ���� ��ȭ �ε��� </summary>
    public int nowDialog_index;
    /// <summary> ���� ������ Ÿ�� 0: Ž��, 1: ��ȭ, 2: ���� 3:��ǥ, 3:����, 4:�޽� </summary>
    public int posType;
    /// <summary> ĳ���� ��ġ </summary>
    public Vector2 position;
    /// <summary> �κ��丮 ������ </summary>
    public Dictionary<int, int> itmes;

    public SaveData(int index, int dayNum, string nowDate, float gameTime, 
        Dictionary<string, List<int>> readDialogs, Dictionary<string, List<int>> selectDialogs, 
        int nowDialog_id, int nowDialog_index, int posType, Vector2 position, 
        Dictionary<int, int> inventoryItems)
    {
        this.index = index;
        this.saveDate = nowDate;
        this.dayNum = dayNum;
        this.gameTime = gameTime;
        this.readDialogs = readDialogs;
        this.selectDialogs = selectDialogs;
        this.nowDialog_id = nowDialog_id;
        this.nowDialog_index = nowDialog_index;
        this.posType = posType;
        this.position = position;
        this.itmes = inventoryItems;
    }
}

/// <summary> ���������� �������־���ϴ� ���ӵ����� </summary>
[Serializable]
public struct GlobalData
{
    /// <summary> ������ Ƚ�� </summary>
    public int clearCount;
    /// <summary> ��û�� ���� </summary>
    public List<int> watchedEnding;
}