using UnityEngine;

public class SoundSettingValue : MonoBehaviour
{
    public static SoundSettingValue instance;
    protected float bgmVolume = 1f;
    public float BGMVolume
    {
        get { return bgmVolume; }
        set { bgmVolume = value; PlayerPrefs.SetFloat("PlayBGMVolume", value); }
    }
    protected float sfxVolume = 1f;
    public float SFXVolume
    {
        get { return sfxVolume; }
        set { sfxVolume = value; PlayerPrefs.SetFloat("PlaySFXVolume", value); }
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        LoadData();
    }

    void LoadData()
    {
        bgmVolume = PlayerPrefs.GetFloat("PlayBGMVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("PlaySFXVolume", 1f);
    }


}
