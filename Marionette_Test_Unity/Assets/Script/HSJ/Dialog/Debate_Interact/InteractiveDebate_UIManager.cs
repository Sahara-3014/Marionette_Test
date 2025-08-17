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
    public SpriteRenderer target;
    public SpriteRenderer answer;
    [SerializeField] InventoryManager inventoryManager;
    [SerializeField] RectTransform[] targetAbilityGauges;

    [Space(10)]
    [SerializeField] SimpleScrollSnap scrollSnap;
    [SerializeField] GameObject choicePanel;
    [SerializeField] List<InteractiveDebate_ChoiceBtn> choiceBtns;
    [SerializeField] GameObject dialogPrefab;

    [HideInInspector]
    public UnityAction skipAction = null;

    public float autoPlayDelay = 0.1f; // 자동 재생 딜레이
    float autoPlayTimer = 0f;

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
        ClearDialog();

        // 데이터 셋팅
        Loaded_DataSet();
    }

    private void FixedUpdate()
    {
        //인풋처리
#if UNITY_STANDALONE_WIN
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("KeyUp");
            autoPlayTimer = 0f; // 자동 재생 타이머 초기화
        }

        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log("KeyDown");
            autoPlayTimer += Time.fixedDeltaTime;
            if (autoPlayTimer >= autoPlayDelay)
            {
                dialogManager.Play();
                autoPlayTimer = 0f; // 자동 재생 타이머 초기화
            }
        }
        
        

#endif
    }

    /// <summary> Load했을때 이전 데이터 뿌려주기 </summary>
    public void Loaded_DataSet()
    {
        while(scrollSnap.Content.childCount > 0)
            scrollSnap.Remove(0);
        Loaded_TextSet();


        var slots = inventoryManager.itemSlot;
        //for (int i = 0; i < slots.Length; i++)
        //    Destroy(slots[i].gameObject);
        //inventoryManager.itemSlot = null;
        Loaded_InventorySet();

    }

    /// <summary> 대화 내용 뿌려주기 </summary>
    void Loaded_TextSet()
    {
        int dialog_id = database.SaveData_GetNowDialogID();
        int dialog_index = database.SaveData_GetNowDialogIndex();
        InteractiveDebate_DialogueData[] data = database.Get_DebateDialogs_NeedID(dialog_id);

        for(int i=0;i<dialog_index;i++)
        {
            AddDialog(data[i].SPEAKER, data[i].DIALOGUE);
            skipAction?.Invoke();
        }
    }

    void Loaded_InventorySet()
    {
        Dictionary<int, int> items = database.SaveData_GetItems();
        if(items == null || items.Count == 0) return;
        foreach (var item in items)
        {
            inventoryManager.AddItem(item.Key, item.Value);
        }
    }

    public void ClearDialog()
    {
        while (scrollSnap.Content.childCount > 0)
        {
            scrollSnap.Remove(0);
        }
    }

    public void AddDialog(string name, string text, UnityAction callback = null)
    {
        // 대화 프리팹 생성
        int index = scrollSnap.NumberOfPanels;
        scrollSnap.Add(dialogPrefab, index, true);
        GameObject dialogObj = scrollSnap.Content.GetChild(index).gameObject;
        TextMeshProUGUI nameLabel = dialogObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
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
            tmp.text = text.Substring(0, i);
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
