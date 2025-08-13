using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SaveLoadPanel : MonoBehaviour
{
    public enum SaveTpye { Save = 0, Load = 1, New = 2 }

    public static SaveLoadPanel instance;
    [SerializeField] private SaveDatabase database;
    [SerializeField] TextMeshProUGUI titleLabel;
    [SerializeField] Button[] btns;
    [SerializeField] Button backBtn;
    public UnityAction onLoadAction;
    public UnityAction onNewAction;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        database = SaveDatabase.Instance;
    }

    public void Open(SaveTpye saveType)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        if(database == null)
            database = SaveDatabase.Instance;
        titleLabel.text = saveType == SaveTpye.Save ? "Save File" : saveType == SaveTpye.Load ? "Load File" : "New File";

        for (int i = 0; i < btns.Length; i++)
        {
            var data = i == 0 ? database.AutoLoad() : database.Load(i, false);

            TextMeshProUGUI tmp = btns[i].GetComponentInChildren<TextMeshProUGUI>();

            DateTime date;
            if (i == 0 && data.index != -1)
            {
                date = DateTime.Parse(data.saveDate);
                tmp.text = $"AutoFile {date.Year}-{date.Month}-{date.Day} {date.Hour}:{date.Minute}:{date.Second} {data.dayNum} Days";
            }
            else if(i== 0 && data.index == -1)
                tmp.text = "AutoFile Empty";
            else if (data.index == -1)
                tmp.text = $"File {i} Empty";
            else
            {
                date = DateTime.Parse(data.saveDate);
                tmp.text = $"File {i} {date.Year}-{date.Month}-{date.Day} {date.Hour}:{date.Minute}:{date.Second} {data.dayNum} Days";
            }

            btns[i].onClick.RemoveAllListeners();
            if (i != 0 && saveType == SaveTpye.Save)
                btns[i].onClick.AddListener(() => { Save(i); });
            else if (saveType == SaveTpye.Load)
                btns[i].onClick.AddListener(() => { Load(i); });
            else if (i != 0 && saveType == SaveTpye.New)
                btns[i].onClick.AddListener(() => { NewGame(i); });
        }
    }

    public void NewGame(int index)
    {
        var data = database.SaveData_Get();
        data.index = index;
        data.saveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        data.dayNum = 0;
        onNewAction?.Invoke();
    }

    public void Save(int index)
    {
        database.Save(index);
        var data = database.SaveData_Get();
        var date = DateTime.Parse(data.saveDate);
        btns[index].GetComponentInChildren<TextMeshProUGUI>().text = $"File {index} {date.Year}-{date.Month}-{date.Day} {date.Hour}:{date.Minute}:{date.Second} {data.dayNum} Days";
    }

    public void Load(int index)
    {
        var data = database.Load(index, false);
        if (data.index == -1)
            return;
        onLoadAction?.Invoke();
    }

    public void BtnsDisable()
    {
        foreach(var btn in btns)
        {
            btn.interactable = false;
        }
        if(backBtn != null)
            backBtn.interactable = false;
    }

    public void BtnsEnable()
    {
        foreach (var btn in btns)
        {
            btn.interactable = true;
        }
        if (backBtn != null)
            backBtn.interactable = true;
    }
}
