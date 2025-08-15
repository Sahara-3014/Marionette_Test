using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public enum AudioMixerType { BGM, SFX, Dialog, Master}
public class InteractDebate_PausePanel : MonoBehaviour
{
    public static InteractDebate_PausePanel instance;
    [SerializeField] Slider bgm;
    [SerializeField] TextMeshProUGUI bgmLabel;
    [SerializeField] Slider sfx;
    [SerializeField] TextMeshProUGUI sfxLabel;
    [SerializeField] SaveLoadPanel saveLoadPanel;
    SoundSettingValue setting;
    public string sceneName = "HSJ_Lobby";
    AudioMixerGroup audioMixerGroup;
    DialogSoundManager soundManager;

    // Req Setting Manager

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

    private void Start()
    {
        if(saveLoadPanel == null)
            saveLoadPanel = SaveLoadPanel.instance;
        if(InteractiveDebate_UIManager.instance != null && saveLoadPanel.onLoadAction == null)
            saveLoadPanel.onLoadAction = () => InteractiveDebate_UIManager.instance.Loaded_DataSet();
        else if(InteractiveDebate_UIManager.instance != null)
            saveLoadPanel.onLoadAction += () => InteractiveDebate_UIManager.instance.Loaded_DataSet();
        // TODO Setting Manager : Value Setting
        setting = SoundSettingValue.instance;
        OnEnable();
    }

    private void OnEnable()
    {
        if(setting == null)
            setting = SoundSettingValue.instance;
        if (setting == null)
            return;
        // TODO bgm setting
        bgm.maxValue = 1f;
        bgm.minValue = 0.0001f;
        bgm.value = setting.BGMVolume;
        bgmLabel.text = $"{Mathf.FloorToInt(bgm.value * 100f)} %";
        var mixer = GetAudioMixer();
        mixer.SetFloat(AudioMixerType.BGM.ToString(), Mathf.Log10(setting.BGMVolume)*20);
        // TODO sfx setting
        sfx.maxValue = 1f;
        sfx.minValue = 0.0001f;
        sfx.value = setting.SFXVolume;
        sfxLabel.text = $"{Mathf.FloorToInt(sfx.value * 100f)} %";
        mixer.SetFloat(AudioMixerType.SFX.ToString(), Mathf.Log10(setting.SFXVolume)*20);
    }

    private void OnDisable()
    {
        if (soundManager == null)
            soundManager = DialogSoundManager.Instance;
        if (soundManager.bgmSource.isPlaying)
            soundManager.bgmSource.Stop();
        if (soundManager.seSource2.isPlaying)
            soundManager.seSource2.Stop();

        if (InteractiveDebate_UIManager.instance != null)
            saveLoadPanel.onLoadAction -= () => InteractiveDebate_UIManager.instance.Loaded_DataSet();
    }

    public void ChangeSlider(bool isBGM)
    {
        Slider slider = isBGM ? bgm : sfx;
        var label = isBGM ? bgmLabel : sfxLabel;

        float cal = Mathf.FloorToInt(slider.value * 100f);
        label.text = $"{cal.ToString()} %";

        if(isBGM)
            setting.BGMVolume = slider.value; 
        else
            setting.SFXVolume = slider.value;

        var mixer = GetAudioMixer();
        mixer.SetFloat(isBGM ? AudioMixerType.BGM.ToString() : AudioMixerType.SFX.ToString(), Mathf.Log10(isBGM ? setting.BGMVolume : setting.SFXVolume)*20);

        if (soundManager == null)
            soundManager = DialogSoundManager.Instance;
        if (isBGM)
        {
            if(!soundManager.bgmSource.isPlaying)
                soundManager.bgmSource.Play();
        }
        else
        {
            if(soundManager.seSource2.isPlaying)
                soundManager.seSource2.Stop();
            soundManager.seSource2.Play();
        }
    }

    public void OpenSaveLoadPanel(SaveLoadPanel.SaveTpye saveTpye)
    {
        saveLoadPanel.gameObject.SetActive(true);
        saveLoadPanel.Open(saveTpye);
    }

    public void SceneMove()
    {
        SaveDatabase.Instance.ChangeScene(sceneName);
    }

    public void OpenSaveLoad(bool isSave)
    {
        saveLoadPanel.Open(isSave ? SaveLoadPanel.SaveTpye.Save : SaveLoadPanel.SaveTpye.Load);
    }

    public AudioMixer GetAudioMixer()
    {
        if (audioMixerGroup == null)
            audioMixerGroup = Resources.Load<AudioMixerGroup>("MasterAudioMixer");
        return audioMixerGroup.audioMixer;
    }
}
