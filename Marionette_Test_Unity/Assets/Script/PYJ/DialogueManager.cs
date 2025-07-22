using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;


public class DialogueManager : MonoBehaviour
{


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





    private Dictionary<int, DialogueData> dialogueDict;
    private int currentIndex = 1;    // 현재 대사 인덱스
    private int nextDialogueIndex = -1;  // 다음 대사 인덱스 저장용
    private bool canInput = false;


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
        dialogue = newDialogue;

        dialogueDict = new Dictionary<int, DialogueData>();
        foreach (var d in dialogue)
        {
            dialogueDict[d.index] = d;
        }

        if (!isDialogue) // 대화 중이 아니면 초기화
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
    public void ShowDialogue()
    {
        if (!dialogueDict.ContainsKey(1))
        {
            Debug.LogError("초기 인덱스 1번 대사가 존재하지 않습니다!");
            return;
        }

        OnOff(true);
        currentIndex = 1;
        NextDialogue();
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




    //
    //다음 대화로 넘어가는 함수
    //
    private void NextDialogue()
    {
        Debug.Log($"NextDialogue 호출 currentIndex = {currentIndex}");
        if (dialogueDict.ContainsKey(currentIndex))
        {
            var d = dialogueDict[currentIndex];
            Debug.Log($"대사 index={currentIndex}, choices 존재 여부={d.choices != null && d.choices.Length > 0}");
        }
        else
        {
            Debug.LogWarning($"대사 index {currentIndex} 없음");
        }
        if (!dialogueDict.ContainsKey(currentIndex))
        {
            Debug.LogWarning($"대사 인덱스 {currentIndex} 없음. 대사키 목록: [{string.Join(",", dialogueDict.Keys.Select(k => k.ToString()))}]");
            OnOff(false);
            return;
        }

        var currentDialogue = dialogueDict[currentIndex];
        Debug.Log($"현재 대사 index: {currentIndex}, 다음 index: {currentDialogue.nextIndex}");

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


        string nextIndexStr = currentDialogue.nextIndex?.Trim() ?? "";
        int nextIndexNum;
        bool isNumeric = int.TryParse(nextIndexStr, out nextIndexNum);

        if (currentDialogue.choices == null || currentDialogue.choices.Length == 0)
        {
            if (isNumeric)
            {
                if (nextIndexNum == 0)
                {
                    OnOff(false);
                    return;
                }
                if (!dialogueDict.ContainsKey(nextIndexNum))
                {
                    Debug.LogWarning($"대사 인덱스 {nextIndexNum} 없음.");
                    OnOff(false);
                    return;
                }
                nextDialogueIndex = nextIndexNum;  // 현재 대사를 바로 바꾸지 말고 다음 대사 인덱스를 저장
                isDialogue = true;
            }

            else
            {
                if (nextIndexStr == "END" || nextIndexStr == "-1")
                {
                    OnOff(false);
                    return;
                }
                else if (!string.IsNullOrEmpty(nextIndexStr))
                {
                    // nextIndexStr을 시트명으로 간주하고 시트 로드 시도
                    StartDialogueSheet(nextIndexStr);
                    return;
                }
                else
                {
                    Debug.LogWarning($"알 수 없는 nextIndex 값: {nextIndexStr}");
                    OnOff(false);
                    return;
                }
            }

        }
        else
        {
            isDialogue = false;
        }



    }


    private void StartDialogueSheet(string sheetName)
    {
        Debug.Log($"StartDialogueSheet called with sheetName: {sheetName}");

        if (sheetLoader == null)
        {
            Debug.LogError("GoogleSheetLoader가 할당되어 있지 않습니다!");
            return;
        }

        // GoogleSheetLoader에게 해당 시트명으로 대사 로드 요청
        sheetLoader.LoadDialoguesFromSheet(sheetName);
    }





    //
    // 대사 텍스트를 타이핑 효과로 보여주는 코루틴
    //
    private IEnumerator TypeText(string sentence, int dialogueIndex)
    {
        isTyping = true;
        canInput = false;
        txt_Dialogue.text = "";

        foreach (char letter in sentence)
        {
            while (isPaused)
                yield return null;

            txt_Dialogue.text += letter;
            yield return new WaitForSeconds(0.05f);
        }

        isTyping = false;

        if (dialogueDict[dialogueIndex].choices != null && dialogueDict[dialogueIndex].choices.Length > 0)
        {
            ShowChoices(dialogueDict[dialogueIndex].choices);
            canInput = false;
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
        if (choicePanel.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isProcessingInput && (canInput || isTyping))
            {
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

        if (isTyping)
        {
            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            txt_Dialogue.text = dialogueDict[currentIndex].dialogue;

            isTyping = false;
            canInput = true;
        }
        else if (canInput)
        {
            canInput = false;

            if (nextDialogueIndex > 0)
            {
                currentIndex = nextDialogueIndex;
                NextDialogue();
            }
            else
            {
                OnOff(false);
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
    public void OnChoiceSelected(int nextIndex)
    {
        choicePanel.SetActive(false);
        currentIndex = nextIndex;
        isDialogue = true;
        NextDialogue();
    }





    //
    // 선택지 보여주는 함수
    //
    private void ShowChoices(DialogueChoice[] choices)
    {
        choicePanel.SetActive(true);

        int countChoices = Mathf.Min(choices.Length, choiceButtons.Length, choiceButtonTexts.Length);

        for (int i = 0; i < countChoices; i++)
        {
            choiceButtons[i].gameObject.SetActive(true);
            choiceButtonTexts[i].text = choices[i].choiceText;

            // nextIndex string → int 변환
            string nextIndexStr = choices[i].nextIndex;
            int nextIndex;
            if (!int.TryParse(nextIndexStr, out nextIndex))
            {
                Debug.LogWarning($"nextIndex '{nextIndexStr}'를 int로 변환하지 못했습니다. 기본값 -1로 설정합니다.");
                nextIndex = -1;
            }

            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(nextIndex));
        }

        for (int i = countChoices; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].gameObject.SetActive(false);
        }
    }






}
