using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.X86.Avx;

public class InteractiveDebate_UIManager : MonoBehaviour
{
    SaveDatabase database;
    DayCycleSystem dayCycleSystem;

    [Header("Default UI Values")]
    public SpriteRenderer BG;
    [SerializeField] TextMeshProUGUI timerLabel;
    [SerializeField] InventoryManager inventoryManager;
    [SerializeField] Transform inventoryViewer;
    [SerializeField] ItemSlot itemSlotPrefab;
    List<GameObject> itemSlots = new();
    int selectItemIndex = -1;
    bool isUploading = false;
    [SerializeField] RectTransform uploadGauge;
    [SerializeField] InteractiveDebate_DialogManager dialogManager;

    [Space(10)]
    [Header("Target's Values")]
    public Image target;
    [SerializeField] TextMeshProUGUI targetNameLabel;
    [SerializeField] Transform targetLogViewer;
    [SerializeField] GameObject targetTextPrefab;
    [SerializeField] RectTransform[] targetAbilityGauges;

    [SerializeField] Debate_TargetHeartGraphController heartGraph;

    [Space(10)]
    [Header("Debate Answer's Values")]
    public Image[] answers;
    [SerializeField] Transform logViewer;
    [SerializeField] GameObject textPrefab;

    [Space(10)]
    [Header("Choice Values")]
    [SerializeField] GameObject choicePanel;
    [SerializeField] List<Button> choiceBtns;

    [HideInInspector]
    public UnityAction skipAction = null;


    private void Start()
    {
        // 초기화
        database = SaveDatabase.Instance;
        dayCycleSystem = DayCycleSystem.Instance;

        foreach (Transform child in inventoryViewer)
            Destroy(child.gameObject);
        foreach (Transform child in targetLogViewer)
            Destroy(child.gameObject);
        foreach (Transform child in logViewer)
            Destroy(child.gameObject);

        CloseChoicePanel();
        Vector2 delta = uploadGauge.sizeDelta;
        delta.x = 0f;
        uploadGauge.sizeDelta = delta;

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
    void Loaded_DataSet()
    {
        Loaded_TextSet();
        Loaded_InventortySet();
        RefreshTimer();
    }

    /// <summary> 대화 내용 뿌려주기 </summary>
    void Loaded_TextSet()
    {
        int dialog_id = database.SaveData_GetNowDialogID();
        int dialog_index = database.SaveData_GetNowDialogIndex();
        DialogueData[] data = database.Get_Dialogs_NeedID(dialog_id);
        for (int i = 0; i < dialog_index; i++)
        {
            GameObject textObj;
            
            ////타겟일 경우
            //textObj = Instantiate(targetTextPrefab, targetLogViewer);
            //textObj.GetComponent<TextMeshProUGUI>().text = data[i].dialogue;
            
            //아닌경우
            textObj = Instantiate(textPrefab, logViewer);
            textObj.GetComponent<TextMeshProUGUI>().text = $"{data[i].dialogue} : {data[i].characterName}";
        }
    }

    public void Add_TargetText(string text, UnityAction callback = null)
    {
        GameObject textObj = Instantiate(targetTextPrefab, targetLogViewer);
        var tmp = textObj.GetComponent<TextMeshProUGUI>();
        TextTypingEffect(tmp, text, callback: callback);

        ScrollRect scroll = targetLogViewer.parent.parent.GetComponent<ScrollRect>();
        scroll.verticalScrollbarSpacing = 1f; // 스크롤을 맨 아래로 이동
    }

    public void Add_OtherAnswerText(string name, string text, UnityAction callback = null)
    {
        GameObject textObj = Instantiate(textPrefab, logViewer);
        var tmp = textObj.GetComponent<TextMeshProUGUI>();
        tmp.text = $" : {name}";
        TextTypingEffect(tmp, text, isReverse: true, callback: callback);

        ScrollRect scroll = logViewer.parent.parent.GetComponent<ScrollRect>();
        Debug.Log(scroll == null);
        scroll.verticalNormalizedPosition = 0; // 스크롤을 맨 아래로 이동
    }

    async void TextTypingEffect(TextMeshProUGUI tmp, string text, float lettersDelay = .02f, bool isReverse = false, UnityAction callback = null)
    {
        string txt = tmp.text;
        skipAction = () => tmp.text = isReverse ? text + txt : text;
        TimeSpan delay = TimeSpan.FromSeconds(lettersDelay);
        for(int i=0;i<=text.Length;i++)
        {
            if(isReverse)
                tmp.text = text.Substring(0, i) + txt;
            else
                tmp.text += text.Substring(i, 1);
            await Task.Delay(delay);
        }
        skipAction = null;
        callback?.Invoke();
    }

    public void ChangeTarget(string targetName)
    {
        //target.sprite = database.GetTargetSprite(targetName);
        targetNameLabel.text = targetName;
    }

    /// <summary> 타겟의 감정 게이지 </summary>
    /// <param name="gaugeType"></param>
    public void ChangeAbilityGauge(CharAttributeData.CharAttributeType attributeType)
    {
        float max = targetAbilityGauges[(int)attributeType].parent.gameObject.GetComponent<RectTransform>().rect.width;
        max -= targetAbilityGauges[(int)attributeType].localPosition.x * 2f;
        try 
        { 
            float value = database.SaveData_GetCharData_GetGauge(attributeType.ToString(), InteractiveDebate_DialogManager.instance.debateData.TARGET_NAME).value;

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


    /// <summary> 인벤토리 아이템 뿌려주기 </summary>
    void Loaded_InventortySet()
    {
        Dictionary<int, int> items = database.SaveData_GetItems();
        foreach(KeyValuePair<int, int> item in items)
        {
            ItemSlot itemSlot = Instantiate(itemSlotPrefab, inventoryViewer);
            //일단 임시
            itemSlot.AddItem(item.Key.ToString(), item.Value, null, "");
            itemSlots.Add(itemSlot.gameObject);
        }
    }

    /// <summary> 인벤토리 위 탭 누르면 작동하는 곳 </summary>
    public void SelectInventoryTab(int itemType)
    {

    }

    /// <summary>  </summary>
    public void SelectItemSlot(int itemIndex)
    {
        this.selectItemIndex = itemIndex;
    }

    /// <summary> 선택한 아이템 </summary>
    public void SelectItemUpload()
    {
        if(isUploading) return;

        isUploading = true;

        uploadGauge.sizeDelta = new Vector2(0f, uploadGauge.sizeDelta.y);

        float max = uploadGauge.parent.gameObject.GetComponent<RectTransform>().rect.width;
        max -= uploadGauge.localPosition.x * 2f;

        Vector2 delta = uploadGauge.sizeDelta;
        delta.x = 0f;
        uploadGauge.DOSizeDelta(delta, 2f).OnComplete(()=>
        {
            isUploading = false;
            //uploadGauge.sizeDelta = new Vector2(0f, uploadGauge.sizeDelta.y);
        });

    }

    /// <summary> 타이머 텍스트 </summary>
    public void RefreshTimer()
    {
        if (dayCycleSystem == null) return;
        int days = dayCycleSystem.GetDays();
        float times = dayCycleSystem.GetTimes();
        timerLabel.text = $"D-{7-days}\n<color=yellow>{times:0:00}</color>";
    }

    /// <summary> 선택지 UI 셋팅 </summary>
    public void OpenChoicePanel(List<(int, string)> choices)
    {
        Debug.Log($"OpenChoicePanel {choices.Count}");
        choicePanel.SetActive(true);
        while(choiceBtns.Count < choices.Count)
        {
            Button newBtn = Instantiate(choiceBtns[0], choiceBtns[0].transform.parent);
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
            choiceBtns[_index].onClick.RemoveAllListeners();
            Debug.Log($"OpenChoicePanel foreach Remove");
            // 버튼 누르면 선택창 닫고 대화 시작
            choiceBtns[_index].onClick.AddListener(() =>
            {
                CloseChoicePanel();
                if (key == -1)
                {
                    dialogManager.currentIndex++;
                    dialogManager.Play();
                }
                else
                    dialogManager.SetDialogs(key, true, true);
            });
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
        foreach (Button btn in choiceBtns)
        {
            btn.gameObject.SetActive(false);
        }
    }

}
