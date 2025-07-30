using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;


public class DialogueManager : MonoBehaviour
{
    private Dictionary<(int ID, int index), DialogueData> dialogueDictByIDAndIndex;


    [System.Serializable]
    public class CharacterStatus
    {

        public string name;
        public string head;
        public string body;
        public Dialog_CharPos position;
    }

    [SerializeField] private GoogleSheetLoader sheetLoader;  // 에디터에서 할당 필요
    [SerializeField] private GameObject cutsceneImageObject; // UI Image or SpriteRenderer
    [SerializeField] private SpriteRenderer[] characterRenderers;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [SerializeField] private DialogEffectManager effectManager;
    [SerializeField] private DialogSoundManager soundManager;

    // 예: 0 = Left, 1 = Center, 2 = Right
    [SerializeField] private SpriteRenderer[] sprite_Heads;  // 머리
    [SerializeField] private SpriteRenderer[] sprite_Bodies; // 몸

    [SerializeField] private SpriteRenderer sprite_BG;
    [SerializeField] private SpriteRenderer sprite_DialogueBox;
    [SerializeField] private SpriteRenderer sprite_CharacterNameBox;

    [SerializeField] private TextMeshProUGUI txt_Dialogue;
    [SerializeField] private TextMeshProUGUI txt_CharacterName;

    [SerializeField] private CharacterPositionManager characterPositionManager;

    [Header("배경 스프라이트 등록")]
    [SerializeField] private List<Sprite> backgroundSprites;
    private Dictionary<string, Sprite> backgroundSpriteDict;


    [Header("캐릭터 스프라이트 등록")]
    [SerializeField] private List<Sprite> characterSprites; // 이름: 파일 이름과 동일
    private Dictionary<string, Sprite> characterSpriteDict;


    private Dictionary<string, string> characterNameMap = new Dictionary<string, string>()
{
    { "주한", "JUHAN" },
    { "미래", "MIRAE" },
    { "계란", "EGG" }
    // 필요한 만큼 추가
};
    [SerializeField] private GameObject choicePanel;        // 선택지 전체 UI
    [SerializeField] private Button[] choiceButtons;          // 선택지 버튼들
    [SerializeField] private TextMeshProUGUI[] choiceButtonTexts; // 버튼 텍스트




    private int currentID = 1000;
    private int nextDialogueID = -1;  // 다음 대화 ID 저장용
    private int currentIndex = 1;    // 현재 대사 인덱스
    private bool canInput = false;
    private int lastEffectIndex = -1;
    private bool inputQueuedBeforeChoice = false; // 선택지 전 입력 저장용
    private bool waitingForChoiceDisplay = false; // 선택지 뜨기 전 상태



    //
    // 캐릭터 이름과 상태 매핑
    //
    private void Awake()
    {
        characterSpriteDict = new Dictionary<string, Sprite>();
        foreach (var sprite in characterSprites)
        {
            characterSpriteDict[sprite.name] = sprite;
        }

        backgroundSpriteDict = new Dictionary<string, Sprite>();
        foreach (var bg in backgroundSprites)
        {
            backgroundSpriteDict[bg.name] = bg;
        }

        
    }

    //
    // 대사 데이터 설정 함수
    //
    public void SetDialogue(DialogueData[] newDialogue)
    {
        if (newDialogue == null)
        {
            Debug.LogError("SetDialogue 호출 시 전달된 newDialogue가 null입니다!");
            return;
        }

        dialogue = newDialogue;

        if (dialogueDictByIDAndIndex == null)
            dialogueDictByIDAndIndex = new Dictionary<(int, int), DialogueData>();
        else
            dialogueDictByIDAndIndex.Clear();

        foreach (var d in dialogue)
        {
            if (d == null)
            {
                Debug.LogWarning("대사 배열 내에 null 요소가 있습니다.");
                continue;
            }
            dialogueDictByIDAndIndex[(d.ID, d.index)] = d;
        }

        Debug.Log($"SetDialogue 완료 - 총 대사 개수: {dialogueDictByIDAndIndex.Count}");

        if (!isDialogue)
        {
            currentIndex = 1;
            isDialogue = true;
        }
    }




    private bool isDialogue = false;
    private bool isTyping = false;

    [SerializeField] private DialogueData[] dialogue;

    private Coroutine typingCoroutine;




    //
    //대사 보여주는 함수
    //
    public void ShowDialogue(int id, int index)
    {
        Debug.Log($"[ShowDialogue] 호출됨 - id={id}, index={index}");

        var key = (id, index);
        if (!dialogueDictByIDAndIndex.ContainsKey(key))
        {
            Debug.LogWarning($"대사 데이터 없음: ID={id}, index={index}");
            return;
        }

        currentID = id;
        currentIndex = index;

        var currentData = dialogueDictByIDAndIndex[key];
        Debug.Log($"[ShowDialogue] 대사 내용: {currentData.dialogue}");

        isTyping = true;

        StartCoroutine(TypeText(currentData.dialogue, currentData.index));

    }








    //
    //캐릭터 보여주는 함수
    //
    private void ShowCharacter(string name, string head, string body, Dialog_CharPos pos, Dialog_CharEffect effect)
    {
        int posIndex = (int)pos;
        if (posIndex < 0 || posIndex >= sprite_Heads.Length || posIndex >= sprite_Bodies.Length) return;

        var headRenderer = sprite_Heads[posIndex];
        var bodyRenderer = sprite_Bodies[posIndex];

        string englishName = characterNameMap.ContainsKey(name) ? characterNameMap[name] : name;
        string headKey = $"{englishName}_{head}";
        string bodyKey = $"{englishName}_{body}";

        // 머리 출력
        Debug.Log($"headKey = '{headKey}', 등록 여부 = {characterSpriteDict.ContainsKey(headKey)}");

        if (!string.IsNullOrEmpty(headKey) && characterSpriteDict.ContainsKey(headKey))
        {
            headRenderer.sprite = characterSpriteDict[headKey];
            headRenderer.gameObject.SetActive(true);
        }
        else
        {
            headRenderer.sprite = null;
            headRenderer.gameObject.SetActive(false);
            Debug.LogWarning($"[머리 스프라이트 미적용] {headKey}는 등록되지 않았습니다.");
        }

        // 몸 출력
        if (!string.IsNullOrEmpty(bodyKey) && characterSpriteDict.ContainsKey(bodyKey))
        {
            bodyRenderer.sprite = characterSpriteDict[bodyKey];
            bodyRenderer.gameObject.SetActive(true);
        }
        else
        {
            bodyRenderer.sprite = null;
            bodyRenderer.gameObject.SetActive(false);
            Debug.LogWarning($"[몸통 스프라이트 미적용] {bodyKey}는 등록되지 않았습니다.");
        }

        if (characterPositionManager != null)
        {
            characterPositionManager.SetCharacter(headRenderer, pos);
            characterPositionManager.SetCharacter(bodyRenderer, pos);
        }

        if (effect != Dialog_CharEffect.None)
        {
            StartCoroutine(effectManager.RunCharacterEffect(effect, headRenderer));
            StartCoroutine(effectManager.RunCharacterEffect(effect, bodyRenderer));
        }
    }


    public void NextDialogue()
    {
        Debug.Log($"NextDialogue 호출 - currentID: {currentID}, currentIndex: {currentIndex}");

        OnOff(true);

        if (dialogueDictByIDAndIndex == null)
        {
            OnOff(false);
            return;
        }

        if (!dialogueDictByIDAndIndex.TryGetValue((currentID, currentIndex), out var currentDialogue) || currentDialogue == null)
        {
            OnOff(false);
            return;
        }




        if (lastEffectIndex >= 0)
        {
            EffectManager.Instance.StopEffect(lastEffectIndex, true);
        }

        // 현재 대사의 이펙트 재생
        if (dialogueDictByIDAndIndex.TryGetValue((currentID, currentIndex), out var dd))
        {
            int currentEffectIdx = dd.screenEffectIndex;
            if (currentEffectIdx >= 0)
            {
                EffectManager.Instance.PlayEffect(currentEffectIdx);
                lastEffectIndex = currentEffectIdx;
            }
            else
            {
                lastEffectIndex = -1;
            }
        }
        else
        {
            lastEffectIndex = -1;
        }


        var key = (currentID, currentIndex);
        if (dialogueDictByIDAndIndex.ContainsKey(key))
        {
            var d = dialogueDictByIDAndIndex[key];
            Debug.Log($"대사 index={currentIndex}, choices 존재 여부={d.choices != null && d.choices.Length > 0}");
        }
        else
        {
            Debug.LogWarning($"대사 index {currentIndex} 없음");
            OnOff(false);
            return;
        }


        Debug.Log($"현재 대사 index: {currentIndex}, 다음 index: {currentDialogue.nextID}");

        // 캐릭터 등장 위치 추적
        bool[] posUsed = new bool[sprite_Heads.Length];

        if (currentDialogue.characters != null)
        {
            foreach (var ch in currentDialogue.characters)
            {
                int pos = (int)ch.position;
                if (pos >= 0 && pos < posUsed.Length)
                {
                    posUsed[pos] = true;
                    ShowCharacter(ch.name, ch.head, ch.body, ch.position, ch.effect);
                }
                else
                {
                    Debug.LogWarning($"[오류] 유효하지 않은 캐릭터 위치: {pos}");
                }
            }
        }

        for (int i = 0; i < sprite_Heads.Length; i++)
        {
            if (!posUsed[i])
            {
                sprite_Heads[i].sprite = null;
                sprite_Heads[i].gameObject.SetActive(false);

                sprite_Bodies[i].sprite = null;
                sprite_Bodies[i].gameObject.SetActive(false);
            }
        }
        // 컷씬 처리: 여기가 추가되는 부분!
        if (!string.IsNullOrEmpty(currentDialogue.cutscene))
        {
            ShowCutscene(currentDialogue.cutscene);
        }
        else
        {
            HideCutscene();
        }

        // 이름/배경
        txt_CharacterName.text = currentDialogue.speaker;

        string bgKey = currentDialogue.background;
        if (!string.IsNullOrEmpty(bgKey) && backgroundSpriteDict.ContainsKey(bgKey))
        {
            sprite_BG.sprite = backgroundSpriteDict[bgKey];
        }
        else
        {
            sprite_BG.sprite = null;
        }

        // 화면 효과
        if (currentDialogue.screenEffect != Dialog_ScreenEffect.None && sprite_BG != null)
        {
            StartCoroutine(effectManager.RunScreenEffect(currentDialogue.screenEffect, sprite_BG));
        }

        // 사운드
        if (currentDialogue.bgm != null)
            soundManager.PlayDialogSE(currentDialogue.bgm);
        if (currentDialogue.se1 != null)
            soundManager.PlayDialogSE(currentDialogue.se1);
        if (currentDialogue.se2 != null)
            soundManager.PlayDialogSE(currentDialogue.se2);

        typingCoroutine = StartCoroutine(TypeText(currentDialogue.dialogue, currentIndex));


        int nextIDNum = currentDialogue.nextID;  // 이미 int라면 바로 사용 가능


        // 선택지가 있으면 nextDialogueID는 -1 (직접 선택지에서 분기 처리)
        if (currentDialogue.choices != null && currentDialogue.choices.Length > 0)
        {
            isDialogue = true;
            nextDialogueID = -1;  // 선택지가 있으니 자동 진행용 ID는 -1로
            ShowChoices(currentDialogue.choices); // **이 부분이 꼭 호출되어야 선택지가 뜹니다!**
        }
        else
        {
            isDialogue = false;

            int? nextIDNullable = currentDialogue.nextID;

            if (currentDialogue.nextID > 0)
            {
                nextDialogueID = currentDialogue.nextID;

                if (nextDialogueID == currentID)
                {
                    Debug.LogWarning("nextDialogueID가 currentID와 같음. 다음 대화 ID를 변경하세요.");
                    nextDialogueID = -1;
                }
            }
            else
            {
                nextDialogueID = -1;
            }
        }


    }



    private IEnumerator TypeText(string sentence, int dialogueIndex)
    {
        Debug.Log($"TypeText 호출: 대사 인덱스={dialogueIndex}, 텍스트={sentence}");
        isTyping = true;
        canInput = false;
        txt_Dialogue.text = "";

        bool hasChoice = false;
        DialogueData currentDialogue = null;
        var key = (currentID, dialogueIndex);
        if (dialogueDictByIDAndIndex.ContainsKey(key))
        {
            currentDialogue = dialogueDictByIDAndIndex[key];
            hasChoice = currentDialogue.choices != null && currentDialogue.choices.Length > 0;
        }

        if (hasChoice)
            waitingForChoiceDisplay = true;

        foreach (char letter in sentence)
        {
            while (isPaused)
                yield return null;

            txt_Dialogue.text += letter;
            yield return new WaitForSeconds(0.05f);
        }

        isTyping = false;

        if (hasChoice && currentDialogue != null)
        {
            yield return new WaitForSeconds(0.1f);  // 선택지 뜨기 전 살짝 대기
            waitingForChoiceDisplay = false;

            ShowChoices(currentDialogue.choices);

            // 선택지는 자동 진행 안 함
            canInput = true;
        }
        else
        {
            canInput = true;
        }
    }



    private void OnOff(bool flag)
    {
        sprite_DialogueBox.gameObject.SetActive(flag);

        // 머리 오브젝트 켜거나 끔
        foreach (var head in sprite_Heads)
        {
            head.gameObject.SetActive(flag);
        }

        // 몸 오브젝트 켜거나 끔
        foreach (var body in sprite_Bodies)
        {
            body.gameObject.SetActive(flag);
        }

        txt_Dialogue.gameObject.SetActive(flag);
        sprite_CharacterNameBox.gameObject.SetActive(flag);
        txt_CharacterName.gameObject.SetActive(flag);
        sprite_BG.gameObject.SetActive(true);

        isDialogue = flag;
    }

    private bool isProcessingInput = false;


    //
    // 매 프레임마다 입력 처리
    //
    void Update()
    {
        if (isPaused) return;

        // 선택지 패널이 열려 있으면 입력 무시
        if (choicePanel.activeInHierarchy)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (waitingForChoiceDisplay)
            {
                // 선택지 나오기 전 입력이면 → 대사만 다 보여주고 멈춘다
                inputQueuedBeforeChoice = true;
                Debug.Log("선택지 뜨기 전 입력 감지 → 대사만 출력하고 멈춤");
                return;  // 추가: 대사만 보여주고 다음 대사로 안 넘김
            }

            if (isTyping)
            {
                if (!isProcessingInput)
                    StartCoroutine(ProcessInputWithCooldown());
            }
            else if (canInput)
            {
                if (!isProcessingInput)
                    StartCoroutine(ProcessInputWithCooldown());
            }
        }
    }






    //
    // 입력 처리와 쿨타임 적용
    //
    private IEnumerator ProcessInputWithCooldown()
    {
        isProcessingInput = true;

        SkipDialogue();

        yield return new WaitForSeconds(0.2f);  // 0.2초 입력 쿨타임

        isProcessingInput = false;
    }



    //
    // 대화 건너뛰기 함수
    //
    public void SkipDialogue()
    {
        if (isPaused) return;
        if (choicePanel.activeInHierarchy) return; // 선택지 열려있으면 무시
        if (waitingForChoiceDisplay)
        {
            inputQueuedBeforeChoice = true;
            return;
        }

        if (isTyping)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);
            txt_Dialogue.text = dialogueDictByIDAndIndex[(currentID, currentIndex)].dialogue;
            isTyping = false;
            canInput = true;
            return;
        }

        if (canInput)
        {
            canInput = false;

            if (nextDialogueID > 0)
            {
                // 선택지 분기 등으로 다음 ID가 지정된 경우
                currentID = nextDialogueID;
                currentIndex = 1;
                nextDialogueID = -1;
                NextDialogue();
            }
            else
            {
                // 다음 인덱스 자동 진행
                int tryNextIndex = currentIndex + 1;
                if (dialogueDictByIDAndIndex.ContainsKey((currentID, tryNextIndex)))
                {
                    currentIndex = tryNextIndex;
                    NextDialogue();
                }
                else
                {
                    // 더 이상 대사 없음
                    OnOff(false);
                }
            }
        }
    }




    //
    //대사 정지,재개 함수
    //
    private bool isPaused = false;

    public void TogglePauseDialogue()
    {
        isPaused = !isPaused;
        Debug.Log(isPaused ? " 일시정지됨" : " 다시 재생됨");
    }
    private void ShowCutscene(string cutsceneName)
    {
        if (cutsceneImageObject == null)
        {
            Debug.LogError("cutsceneImageObject가 할당되어 있지 않습니다!");
            return;
        }

        var sr = cutsceneImageObject.GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("cutsceneImageObject에 SpriteRenderer 컴포넌트가 없습니다!");
            return;
        }

        Debug.Log("ShowCutscene 호출됨: " + cutsceneName);
        Sprite cutsceneSprite = Resources.Load<Sprite>($"Cutscenes/{cutsceneName}");
        if (cutsceneSprite == null)
        {
            Debug.LogError($"컷씬 이미지 '{cutsceneName}'가 Resources/Cutscenes 폴더에 없습니다!");
            return;
        }

        foreach (var chr in characterRenderers)
        {
            chr.gameObject.SetActive(false);
        }

        cutsceneImageObject.SetActive(true);
        sr.sprite = cutsceneSprite;
    }


    //
    //컷씬 숨기기 함수
    //
    private void HideCutscene()
    {
        cutsceneImageObject.SetActive(false);
    }



    //
    // 선택지 선택 시 호출되는 함수
    //
    public void OnChoiceSelected(int nextID, int nextIndex)
    {
        choicePanel.SetActive(false);

        if (nextID > 0)
        {
            currentID = nextID;
            currentIndex = nextIndex > 0 ? nextIndex : 1;
            nextDialogueID = -1;
            Debug.Log($"선택지 선택: currentID={currentID}, currentIndex={currentIndex}");
        }
        else
        {
            if (nextIndex == -1)
            {
                currentIndex += 1;
            }
            else if (nextIndex > 0)
            {
                currentIndex = nextIndex;
            }
            else
            {
                currentIndex += 1;
            }
            nextDialogueID = currentID;
            Debug.Log($"nextID가 0 이하, currentID 유지, currentIndex 증가: {currentIndex}");
        }

        NextDialogue();  // 여기에선 ShowDialogue 호출하지 말고 NextDialogue만 호출
    }










    //
    // 선택지 보여주는 함수
    //
    private void ShowChoices(DialogueChoice[] choices)
    {

        canInput = false;  // 혹시 모를 입력 방지용
        choicePanel.SetActive(true);


        int countChoices = Mathf.Min(choices.Length, choiceButtons.Length, choiceButtonTexts.Length);

        for (int i = 0; i < countChoices; i++)
        {
            Debug.Log($"선택지[{i}] 텍스트='{choices[i].choiceText}', nextID={choices[i].nextID}, nextIndex={choices[i].nextIndex}");

            int localNextID = choices[i].nextID;
            int localNextIndex = choices[i].nextIndex;

            choiceButtons[i].gameObject.SetActive(true);
            choiceButtonTexts[i].text = choices[i].choiceText;

            choiceButtons[i].onClick.RemoveAllListeners();

            int capturedNextID = localNextID;
            int capturedNextIndex = localNextIndex;
            choiceButtons[i].onClick.AddListener(() => {
                Debug.Log($"선택지 클릭: nextID={capturedNextID}, nextIndex={capturedNextIndex}");
                OnChoiceSelected(capturedNextID, capturedNextIndex);
            });
        }




        for (int i = countChoices; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].gameObject.SetActive(false);
        }
    }


}
