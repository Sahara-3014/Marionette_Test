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
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        saveLoadPanel = SaveLoadPanel.instance;
        // TODO Setting Manager : Value Setting
    }

    private void OnEnable()
    {
        // TODO bgm setting

        // TODO sfx setting

    }

    public void ChangeSlider(bool isBGM)
    {
        Slider slider = isBGM ? bgm : sfx;
        var label = isBGM ? bgmLabel : sfxLabel;

        float cal = Mathf.FloorToInt(slider.value * 100f);
        label.text = $"{cal.ToString()} %"; 
    }

    public void OpenSaveLoadPanel(bool isSavePanel)
    {
        saveLoadPanel.gameObject.SetActive(true);
        saveLoadPanel.Open(isSavePanel);
    }
}
