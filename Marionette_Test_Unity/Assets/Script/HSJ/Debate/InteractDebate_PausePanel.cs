using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        bgm.value = setting.BGMVolume;
        bgmLabel.text = $"{Mathf.FloorToInt(bgm.value * 100f)} %";
        // TODO sfx setting
        sfx.maxValue = 1f;
        sfx.value = setting.SFXVolume;
        sfxLabel.text = $"{Mathf.FloorToInt(sfx.value * 100f)} %";
    }

    private void OnDisable()
    {
        if(InteractiveDebate_UIManager.instance != null)
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
}
