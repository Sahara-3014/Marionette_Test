using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DanielLochner.Assets.SimpleScrollSnap;
using UnityEngine.EventSystems;

public class InteractiveDebate_UIManager : MonoBehaviour
{
    public static InteractiveDebate_UIManager instance;
    SaveDatabase database;
    DayCycleSystem dayCycleSystem;
    [SerializeField] InteractiveDebate_DialogManager dialogManager;

    [Header("Default UI Values")]
    public SpriteRenderer BG;
    [SerializeField] TextMeshProUGUI timerLabel;
    [SerializeField] InventoryManager inventoryManager;
    public SpriteRenderer target;
    public Image answer;
    [SerializeField] RectTransform[] targetAbilityGauges;

    [Space(10)]
    [SerializeField] SimpleScrollSnap scrollSnap;
    [SerializeField] GameObject choicePanel;
    [SerializeField] List<InteractiveDebate_ChoiceBtn> choiceBtns;
    [SerializeField] GameObject dialogPrefab;

    [HideInInspector]
    public UnityAction skipAction = null;


    private void Awake()
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
        // 초기화
        database = SaveDatabase.Instance;
        dayCycleSystem = DayCycleSystem.Instance;

        CloseChoicePanel();
        while(scrollSnap.Content.childCount > 0)
        {
            scrollSnap.Remove(0);
        }

        // 데이터 셋팅
        Loaded_DataSet();
    }

    private void Update()
    {
        //인풋처리
        #if UNITY_ANDROID


        #elif UNITY_STANDALONE_WIN


        #endif
    }

    /// <summary> Load했을때 이전 데이터 뿌려주기 </summary>
    public void Loaded_DataSet()
    {
        Loaded_TextSet();
        RefreshTimer();
    }

    /// <summary> 대화 내용 뿌려주기 </summary>
    void Loaded_TextSet()
    {
        int dialog_id = database.SaveData_GetNowDialogID();
        int dialog_index = database.SaveData_GetNowDialogIndex();
        DialogueData[] data = database.Get_Dialogs_NeedID(dialog_id);

        for(int i=0;i<dialog_index;i++)
        {
            AddDialog(data[i].speaker, data[i].dialogue);
            skipAction?.Invoke();
        }
    }

    public void AddDialog(string name, string text, UnityAction callback = null)
    {
        // 대화 프리팹 생성
        GameObject dialogObj = Instantiate(dialogPrefab, scrollSnap.Content);
        TextMeshProUGUI nameLabel = dialogObj.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI dialogLabel = dialogObj.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        
        nameLabel.text = name;
        dialogLabel.text = string.Empty; // 초기화
        TextTypingEffect(dialogLabel, text, callback: () => callback?.Invoke());

        scrollSnap.GoToPanel(scrollSnap.Content.childCount - 1);
    }

    async void TextTypingEffect(TextMeshProUGUI tmp, string text, float lettersDelay = .02f, UnityAction callback = null)
    {
        skipAction = () =>
        {
            CancelInvoke(nameof(TextTypingEffect)); // 텍스트 타이핑 중지
            tmp.text = text;
            skipAction = null; // 스킵 액션 초기화
        };
        TimeSpan delay = TimeSpan.FromSeconds(lettersDelay);
        for(int i=0;i<=text.Length;i++)
        {
            tmp.text += text.Substring(i, 1);
            await Task.Delay(delay);
        }
        skipAction = null;
        callback?.Invoke();
    }

    /// <summary> 타겟의 감정 게이지 </summary>
    /// <param name="gaugeType"></param>
    public void ChangeAbilityGauge(CharAttributeData.CharAttributeType attributeType)
    {
        float max = targetAbilityGauges[(int)attributeType].parent.gameObject.GetComponent<RectTransform>().rect.width;
        max -= targetAbilityGauges[(int)attributeType].localPosition.x * 2f;
        try 
        { 
            float value = database.SaveData_GetCharData_GetGauge(dialogManager.debateData.TARGET_NAME, attributeType).value;

            targetAbilityGauges[(int)attributeType].DOKill();
            Vector2 delta = targetAbilityGauges[(int)attributeType].sizeDelta;
            delta.x = (value / 100) * max/100f;
            targetAbilityGauges[(int)attributeType].DOSizeDelta(delta, .5f);
        }
        catch (Exception e)
        {
            Debug.LogError($"ChangeAbilityGauge Error: {e.Message}");
            return;
        }
    }

    /// <summary> 타이머 텍스트 </summary>
    public void RefreshTimer()
    {
        if (dayCycleSystem == null) return;
        int days = dayCycleSystem.GetDays();
        timerLabel.text = $"D-{7-days}";
    }

    /// <summary> 선택지 UI 셋팅 </summary>
    public void OpenChoicePanel(List<(int, string)> choices)
    {
        Debug.Log($"OpenChoicePanel {choices.Count}");
        choicePanel.SetActive(true);
        while (choiceBtns.Count < choices.Count)
        {
            var newBtn = Instantiate(choiceBtns[0], choiceBtns[0].transform.parent);
            choiceBtns.Add(newBtn);
        }
        Debug.Log("OpenChoicePanel foreach");
        int _index = 0;
        foreach (var (key, value) in choices)
        {
            Debug.Log($"OpenChoicePanel foreach {_index}");
            choiceBtns[_index].gameObject.SetActive(true);
            Debug.Log($"OpenChoicePanel foreach {choiceBtns[_index].gameObject.activeSelf}");
            choiceBtns[_index].GetComponentInChildren<TextMeshProUGUI>().text = value;
            Debug.Log($"OpenChoicePanel foreach {choiceBtns[_index].GetComponentInChildren<TextMeshProUGUI>().text}");
            Debug.Log($"OpenChoicePanel foreach Remove");
            // 버튼 누르면 선택창 닫고 대화 시작
            choiceBtns[_index].onPressAction = () =>
            {
                CloseChoicePanel();
                if (key == -1)
                {
                    dialogManager.currentIndex++;
                    dialogManager.Play();
                }
                else
                    dialogManager.SetDialogs(key, true, true);
            };
            Debug.Log($"OpenChoicePanel foreach onClick");
            _index++;
        }
    }

    public bool IsChoicePanelOpened()
    {
        return choicePanel.activeSelf;
    }

    /// <summary> 선택창 닫기 </summary>
    public void CloseChoicePanel()
    {
        choicePanel.SetActive(false);
        foreach (var btn in choiceBtns)
        {
            btn.gameObject.SetActive(false);
        }
    }

}
