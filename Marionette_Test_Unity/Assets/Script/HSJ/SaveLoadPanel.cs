using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SaveLoadPanel : MonoBehaviour
{
    public static SaveLoadPanel instance;
    [SerializeField] SaveDatabase database;
    [SerializeField] TextMeshProUGUI titleLabel;
    [SerializeField] Button[] btns;
    public UnityAction onLoadAction;

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

    public void Open(bool isSavePanel)
    {
        if (!gameObject.activeSelf)
            gameObject.SetActive(true);
        if(database == null)
            database = SaveDatabase.Instance;
        titleLabel.text = isSavePanel ? "Save File" : "Load File";

        for (int i = 0; i < btns.Length; i++)
        {
            Debug.Log(database == null);
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
            if(i != 0 && isSavePanel)
                btns[i].onClick.AddListener(() => { Save(i); gameObject.SetActive(false); });
            else if(!isSavePanel)
                btns[i].onClick.AddListener(() => { Load(i);  gameObject.SetActive(false); });
                }
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
}
