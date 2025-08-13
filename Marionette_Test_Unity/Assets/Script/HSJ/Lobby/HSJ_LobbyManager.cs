using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HSJ_LobbyManager : MonoBehaviour
{
    [SerializeField] SaveLoadPanel saveLoadPanel;
    SoundSettingValue setting;
    [SerializeField] Slider[] soundSliders;
    [SerializeField] TextMeshProUGUI[] soundLabels;
    [SerializeField] GameObject[] panels;               // 0 : Main, 1: SaveLoad, 2: Setting

    [SerializeField] string sceneName = "Intro"; // The name of the scene to load

    private void Start()
    {
        if(saveLoadPanel.onLoadAction == null)
        {
            saveLoadPanel.onLoadAction = SceneLoad;
            saveLoadPanel.onNewAction = SceneLoad;
        }
        else
        {
            saveLoadPanel.onLoadAction += SceneLoad;
            saveLoadPanel.onNewAction += SceneLoad;
        }
        setting = SoundSettingValue.instance;

        Transform effect = EffectManager.Instance.gameObject.transform;
        DontDestroyOnLoad(effect.parent.gameObject);
    }

    private void OnDisable()
    {
        saveLoadPanel.onLoadAction -= SceneLoad;
        saveLoadPanel.onNewAction -= SceneLoad;
    }

    public void SceneLoad()
    {
        float delay = 3f;
        SaveDatabase.Instance.AddSceneChangeEvent(sceneName , () =>
        {
            var so = EffectManager.Instance.directionSetList[6];
            EffectManager.Instance.PlayDirectionSet(so);
            SaveDatabase.Instance.StartCoroutine(SaveDatabase.Instance.AfterAction(() =>
            {
                var so = EffectManager.Instance.directionSetList[6];
                EffectManager.Instance.StopDirectionSet(so);
            }, delay));
        });

        saveLoadPanel.BtnsDisable();

        var so = EffectManager.Instance.directionSetList[5];
        EffectManager.Instance.PlayDirectionSet(so);
        
        //EffectManager.Instance.PlayEffect(idx);
        SaveDatabase.Instance.StartCoroutine(SaveDatabase.Instance.AfterAction(() =>
        {
            var so = EffectManager.Instance.directionSetList[5];
            SaveDatabase.Instance.ChangeScene(sceneName);
            //EffectManager.Instance.StopEffect(idx);
            EffectManager.Instance.StopDirectionSet(so);
        }, delay));
    }

    public void ChangeSoundSlider(bool isBGM)
    {
        Slider slider = isBGM ? soundSliders[0] : soundSliders[1];
        TextMeshProUGUI label = isBGM ? soundLabels[0] : soundLabels[1];

        if (isBGM)
            setting.BGMVolume = slider.value;

        else
            setting.SFXVolume = slider.value;
        
        int value = Mathf.FloorToInt(slider.value * 100);
        label.text = $"{value}%";
    }

    public void OpenSavePanel(bool isNew)
    {
        saveLoadPanel.Open(isNew ? SaveLoadPanel.SaveTpye.New : SaveLoadPanel.SaveTpye.Load);
        panels[0].SetActive(false);
    }

    public void CloseSavePanel()
    {
        panels[0].SetActive(true);
        panels[1].SetActive(false);
    }

    public void OpenSettingPanel()
    {
        panels[0].SetActive(false);
        panels[2].SetActive(true);

        soundSliders[0].value = setting.BGMVolume;
        soundLabels[0].text = $"{Mathf.FloorToInt(setting.BGMVolume * 100)}%";

        soundSliders[1].value = setting.SFXVolume;
        soundLabels[1].text = $"{Mathf.FloorToInt(setting.SFXVolume * 100)}%";
    }

    public void CloseSettingPanel()
    {
        panels[0].SetActive(true);
        panels[2].SetActive(false);
    }
}
