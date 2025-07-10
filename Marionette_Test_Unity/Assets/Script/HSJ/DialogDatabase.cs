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

    string filePath = "Assets/Resources";
    public void TextSave(string key, string value)
    {
        File.WriteAllText($"{filePath}/{key}", value);
    }

    public string TextLoad(string key)
    {
         return Resources.Load<TextAsset>(key)?.text;
    }
}
