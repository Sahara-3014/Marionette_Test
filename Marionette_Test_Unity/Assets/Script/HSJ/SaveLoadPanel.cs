using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoadPanel : MonoBehaviour
{
    public static SaveLoadPanel instance;
    SaveDatabase database;
    [SerializeField] TextMeshProUGUI titleLabel;
    [SerializeField] Button[] btns;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        database = SaveDatabase.Instance;
    }

    public void Open(bool isSavePanel)
    {
        titleLabel.text = isSavePanel ? "Save File" : "Load File";

        for (int i = 0; i < btns.Length; i++)
        {
            var data = database.Load(i, false);

            TextMeshProUGUI tmp = btns[i].GetComponentInChildren<TextMeshProUGUI>();
            DateTime date = DateTime.Parse(data.saveDate);
            if (i == 0)
                tmp.text = $"AutoFile {date.Year}-{date.Month}-{date.Day} {date.Hour}:{date.Minute}:{date.Second} {data.dayNum} Days";
            else if (data.index == -1)
                tmp.text = $"File {i} Empty";
            else
                tmp.text = $"File {i} {date.Year}-{date.Month}-{date.Day} {date.Hour}:{date.Minute}:{date.Second} {data.dayNum} Days";

            btns[i].onClick.RemoveAllListeners();
            if(isSavePanel)
                btns[i].onClick.AddListener(() => Save(i));
            else
                btns[i].onClick.AddListener(() => Load(i));
        }
    }

    public void Save(int index)
    {
        database.Save(index);
        btns[index].GetComponentInChildren<TextMeshProUGUI>().text = $"File {index} Saved";
    }

    public void Load(int index)
    {
        var data = database.Load(index, false);
        if (data.index == -1)
            return;

        database.Load(index);
    }
}
