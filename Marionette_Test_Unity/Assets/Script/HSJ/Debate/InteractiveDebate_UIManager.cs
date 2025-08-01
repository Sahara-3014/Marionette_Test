using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractiveDebate_UIManager : MonoBehaviour
{
    SaveDatabase database;
    DayCycleSystem dayCycleSystem;

    [Header("Default UI Values")]
    [SerializeField] TextMeshProUGUI timerLabel;
    [SerializeField] InventoryManager inventoryManager;
    [SerializeField] Transform inventoryViewer;
    [SerializeField] ItemSlot itemSlotPrefab;
    List<GameObject> itemSlots = new();
    [SerializeField] InteractiveDebate_DialogManager dialogManager;

    [Space(10)]
    [Header("Target's Values")]
    [SerializeField] SpriteRenderer target;
    [SerializeField] TextMeshProUGUI targetNameLabel;
    [SerializeField] Transform targetLogViewer;
    [SerializeField] GameObject targetTextPrefab;
    [SerializeField] RectTransform[] targetAbilityGauges;

    [SerializeField] Debate_TargetHeartGraphController heartGraph;

    [Space(10)]
    [Header("Debate Answer's Values")]
    [SerializeField] SpriteRenderer[] answers;
    [SerializeField] Transform logViewer;
    [SerializeField] GameObject textPrefab;

    [Space(10)]
    [Header("Choice Values")]
    [SerializeField] GameObject choicePanel;
    [SerializeField] List<Button> choiceBtns;


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

        // 데이터 셋팅
        Loaded_DataSet();
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
        DialogueData[] data = database.GetDialogs_NeedID(dialog_id);
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

    public void Add_TargetText(string text)
    {
        GameObject textObj = Instantiate(targetTextPrefab, targetLogViewer);
        textObj.GetComponent<TextMeshProUGUI>().text = text;
    }

    public void Add_OtherAnswerText(string text)
    {
        GameObject textObj = Instantiate(textPrefab, logViewer);
        textObj.GetComponent<TextMeshProUGUI>().text = text;
    }

    public void ChangeTarget(string targetName)
    {
        //target.sprite = database.GetTargetSprite(targetName);
        targetNameLabel.text = targetName;
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

    /// <summary> 타이머 텍스트 </summary>
    public void RefreshTimer()
    {
        if (dayCycleSystem == null) return;
        int days = dayCycleSystem.GetDays();
        float times = dayCycleSystem.GetTimes();
        timerLabel.text = $"D-{7-days}\n<color=yellow>{times:0.00}</color>";
    }

    /// <summary> 선택지 UI 셋팅 </summary>
    public void OpenChoicePanel(Dictionary<int, string> Choices)
    {
        choicePanel.SetActive(true);
        while(choiceBtns.Count < Choices.Count)
        {
            Button newBtn = Instantiate(choiceBtns[0], choiceBtns[0].transform.parent);
            choiceBtns.Add(newBtn);
        }

        int _index = 0;
        foreach (KeyValuePair<int, string> keyValuePair in Choices)
        {
            choiceBtns[_index].gameObject.SetActive(true);
            choiceBtns[_index].GetComponentInChildren<TextMeshProUGUI>().text = keyValuePair.Value;
            choiceBtns[_index].onClick.RemoveAllListeners();
            // 버튼 누르면 선택창 닫고 대화 시작
            choiceBtns[_index].onClick.AddListener(() =>
            {
                CloseChoicePanel();
                dialogManager.SetDialogs(keyValuePair.Key, true, true);
            });
            _index++;
        }
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
