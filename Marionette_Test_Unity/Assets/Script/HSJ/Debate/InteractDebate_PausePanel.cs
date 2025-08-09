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
    SaveLoadPanel saveLoadPanel;

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
        saveLoadPanel = SaveLoadPanel.instance;
        if(saveLoadPanel.onLoadAction == null)
            saveLoadPanel.onLoadAction = () => InteractiveDebate_UIManager.instance.Loaded_DataSet();
        else
            saveLoadPanel.onLoadAction += () => InteractiveDebate_UIManager.instance.Loaded_DataSet();
        // TODO Setting Manager : Value Setting
    }

    private void OnEnable()
    {
        // TODO bgm setting

        // TODO sfx setting

    }

    private void OnDisable()
    {
        saveLoadPanel.onLoadAction -= () => InteractiveDebate_UIManager.instance.Loaded_DataSet();
    }

    public void ChangeSlider(bool isBGM)
    {
        Slider slider = isBGM ? bgm : sfx;
        var label = isBGM ? bgmLabel : sfxLabel;

        float cal = Mathf.FloorToInt(slider.value * 100f);
        label.text = $"{cal.ToString()} %"; 
    }

    public void OpenSaveLoadPanel(SaveLoadPanel.SaveTpye saveTpye)
    {
        saveLoadPanel.gameObject.SetActive(true);
        saveLoadPanel.Open(saveTpye);
    }
}
