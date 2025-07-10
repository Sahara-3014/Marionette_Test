using System;
using System.IO;
using UnityEngine;

public class DialogDatabase : MonoBehaviour
{
    public static DialogDatabase Instance { get; private set; }

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
    }
    public void TextSave(string key, string value)
    {
        string path = Application.dataPath;
        File.WriteAllText($"{path}/Resources/{key}", value);
    }

    public string TextLoad(string key)
    {
         return Resources.Load<TextAsset>(key)?.text;
    }
}
