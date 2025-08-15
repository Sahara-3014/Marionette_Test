using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class HSJ_LobbyManager : MonoBehaviour
{
    [SerializeField] SaveLoadPanel saveLoadPanel;
    SoundSettingValue setting;
    [SerializeField] Slider[] soundSliders;
    [SerializeField] TextMeshProUGUI[] soundLabels;
    [SerializeField] GameObject[] panels;               // 0 : Main, 1: SaveLoad, 2: Setting

    [SerializeField] string startSceneName = "Intro"; // The name of the scene to load
    [SerializeField] string loadSceneName = "Intro"; // The name of the scene to load

    AudioMixerGroup audioMixerGroup;
    AudioMixer audioMixer;
    DialogSoundManager soundManager;

    private void Start()
    {
        if(saveLoadPanel.onLoadAction == null)
        {
            saveLoadPanel.onLoadAction = SceneLoad;
            saveLoadPanel.onNewAction = SceneStart;
        }
        else
        {
            saveLoadPanel.onLoadAction += SceneLoad;
            saveLoadPanel.onNewAction += SceneStart;
        }
        setting = SoundSettingValue.instance;

        Transform effect = EffectManager.Instance.gameObject.transform;
        DontDestroyOnLoad(effect.parent.gameObject);
    }

    private void OnDisable()
    {
        saveLoadPanel.onLoadAction -= SceneLoad;
        saveLoadPanel.onNewAction -= SceneStart;
    }

    public void SceneStart()
    {
        float delay = 3f;
        SaveDatabase.Instance.AddSceneChangeEvent(startSceneName, () =>
        {
            var so = EffectManager.Instance.directionSetList[6];
            EffectManager.Instance.PlayDirectionSet(so);
            SaveDatabase.Instance.StartCoroutine(SaveDatabase.Instance.AfterAction(() =>
            {
                var so = EffectManager.Instance.directionSetList[6];
                EffectManager.Instance.StopDirectionSet(so);
                DialogueManager.Instance.SetDialogue(SaveDatabase.Instance.Get_Dialogs_NeedID(1000));
                DialogueManager.Instance.ShowDialogue(1000, 1);
                DialogueManager.Instance.NextDialogue();
            }, delay));
        });

        saveLoadPanel.BtnsDisable();

        var so = EffectManager.Instance.directionSetList[5];
        EffectManager.Instance.PlayDirectionSet(so);

        //EffectManager.Instance.PlayEffect(idx);
        SaveDatabase.Instance.StartCoroutine(SaveDatabase.Instance.AfterAction(() =>
        {
            var so = EffectManager.Instance.directionSetList[5];
            SaveDatabase.Instance.ChangeScene(startSceneName);
            //EffectManager.Instance.StopEffect(idx);
            EffectManager.Instance.StopDirectionSet(so);
        }, delay));
    }

    public void SceneLoad()
    {
        float delay = 3f;
        SaveDatabase.Instance.AddSceneChangeEvent(loadSceneName , () =>
        {
            var invData = SaveDatabase.Instance.SaveData_GetItems();
            for (int i = 0; i < 25; i++)
                InventoryManager.Instance.itemSlot[i].EmptyItem();
            if (invData != null && invData.Count > 0)
            {
                foreach (var item in invData)
                {
                    InventoryManager.Instance.AddItem(item.Key, item.Value);
                }
            }


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
            SaveDatabase.Instance.ChangeScene(loadSceneName);
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

        var mixer = GetAudioMixer();
        mixer.SetFloat(isBGM ? AudioMixerType.BGM.ToString() : AudioMixerType.SFX.ToString(), Mathf.Log10(isBGM ? setting.BGMVolume : setting.SFXVolume)*20);

        if (soundManager == null)
            soundManager = DialogSoundManager.Instance;
        if (isBGM)
        {
            if (!soundManager.bgmSource.isPlaying)
                soundManager.bgmSource.Play();
        }
        else
        {
            if (soundManager.seSource2.isPlaying)
                soundManager.seSource2.Stop();
            soundManager.seSource2.Play();
        }
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

    public AudioMixer GetAudioMixer()
    {
        if(audioMixerGroup == null)
            audioMixerGroup = Resources.Load<AudioMixerGroup>("MasterAudioMixer");
        if (audioMixer == null)
            audioMixer = audioMixerGroup.audioMixer;
        return audioMixer;
    }
}
