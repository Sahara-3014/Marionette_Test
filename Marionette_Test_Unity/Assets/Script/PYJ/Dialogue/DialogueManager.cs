using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;


public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;
    private Dictionary<(int ID, int index), DialogueData> dialogueDictByIDAndIndex;
    public bool isAuto = false;  // 자동 진행 모드 여부
    public float autoDelay = 1f; // 자동으로 다음 대사 넘어가기까지 대기 시간
    private float autoTimer = 0f;



    [System.Serializable]
    public class CharacterStatus
    {

        public string name;
        public string head;
        public string body;
        public Dialog_CharPos position;
    }

    [SerializeField] private GoogleSheetLoader sheetLoader;
    [SerializeField] private GameObject cutsceneImageObject;
    [SerializeField] private SpriteRenderer[] characterRenderers;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [SerializeField] private DialogEffectManager effectManager;
    [SerializeField] private DialogSoundManager soundManager;

    [SerializeField] private SpriteRenderer[] sprite_Heads;  // 머리
    [SerializeField] private SpriteRenderer[] sprite_Bodies; // 몸

    [SerializeField] private SpriteRenderer sprite_BG;
    [SerializeField] private Image sprite_DialogueBox;
    [SerializeField] private TextMeshProUGUI txt_Dialogue;
    [SerializeField] private TextMeshProUGUI txt_CharacterName;

    [SerializeField] private CharacterPositionManager characterPositionManager;

    [Header("배경 스프라이트 등록")]
    [SerializeField] private List<Sprite> backgroundSprites;
    private Dictionary<string, Sprite> backgroundSpriteDict;


    private Dictionary<string, string> characterNameMap = new Dictionary<string, string>()
    {
{ "김주한", "JUHAN" },
{ "설은비", "EUNBI" },
{ "한아영", "AHYOUNG" },
{ "하서하", "SEOHA" },
{ "유무구", "MUGU" },
{ "정해온", "HAEWON" },
{ "도민결", "MINKYEOL" },
{ "배수경", "SUKYUNG" },
{ "권하루", "HARU" },
{ "박세진", "SEJIN" },
{ "백이후", "IHU" },
{ "강세령", "SERYEONG" },
{ "최범식", "BEOMSIK" },
{ "나율", "YUL" },
{ "이시아", "SIA" }

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

        if (Instance == null)
        {
            Instance = this;
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
    public void ShowDialogue(int id, int index)
    {

        var key = (id, index);
        if (!dialogueDictByIDAndIndex.ContainsKey(key))
        {
            Debug.LogWarning($"대사 데이터 없음: ID={id}, index={index}");
            return;
        }

        var currentData = dialogueDictByIDAndIndex[key];
        Debug.Log($"commands: '{currentData.commands}'");
        // 명령어 처리 (한 번만)
        if (!string.IsNullOrEmpty(currentData.commands))
        {
            string[] commands = currentData.commands.Split(new char[] { ' ', ',', ';' }, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var cmd in commands)
            {
                switch (cmd)
                {
                    case "BGM_SLOW":
                        if (!soundManager.bgmSource.isPlaying)
                        {
                            // bgm이 안 켜져 있으면, 재생부터 시도
                            if (soundManager.bgmSource.clip != null)
                            {
                                soundManager.PlayBGM();
                            }
                            else if (currentData.bgm != null && currentData.bgm.dialogSE.clip != null)
                            {
                                // 만약 clip도 없으면, 현재 데이터에 있는 bgm으로 PlayBGM 호출
                                soundManager.PlayBGM(currentData.bgm.dialogSE);
                            }
                        }
                        soundManager.SetBGMSpeed(0.5f);
                        break;

                    case "BGM_OFF":
                        soundManager.StopBGM();
                        break;
                    case "BGM_ON":
                        soundManager.SetBGMSpeed(1f);
                        soundManager.PlayBGM();
                        break;
                }
            }
        }

        // 1) BGM 교체 필요시 실행
        // 기존 명령어 처리 부분 아래, BGM 교체 처리 구간 대체

        if (!string.IsNullOrEmpty(currentData.bgmName))
        {
            // 먼저 DialogSE 객체 생성
            var bgmSE = new DialogSE(SEType.BGM, null);

            // clip 로드하면서 stopSE 여부도 같이 설정됨
            bgmSE.clip = DialogSoundManager.Instance.LoadAudioClipByName(currentData.bgmName, bgmSE);

            if (bgmSE.stopSE)
            {
                // -1 명령이면 BGM 끔
                DialogSoundManager.Instance.StopBGM();
            }
            else if (bgmSE.clip != null)
            {
                if (DialogSoundManager.Instance.bgmSource.clip != bgmSE.clip)
                {
                    DialogSoundManager.Instance.PlayBGM(bgmSE);
                }
            }
            else
            {
                Debug.LogWarning($"BGM 클립을 못 찾음: {currentData.bgmName}");
            }
        }



        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }

        typingCoroutine = StartCoroutine(TypeText(currentData.dialogue, index));

    }

    private void ShowCharacter(string name, string head, string body, Dialog_CharPos pos, Dialog_CharEffect effect)
    {
        int posIndex = (int)pos;
        if (posIndex < 0 || posIndex >= sprite_Heads.Length || posIndex >= sprite_Bodies.Length) return;

        var headRenderer = sprite_Heads[posIndex];
        var bodyRenderer = sprite_Bodies[posIndex];

        string englishName = characterNameMap.ContainsKey(name) ? characterNameMap[name] : name;

        string headSpriteName = $"{head}";
        Sprite headSprite = LoadSpriteForSpeaker(name, headSpriteName);
        if (headSprite != null)
        {
            headRenderer.sprite = headSprite;
            headRenderer.gameObject.SetActive(true);
        }
        else
        {
            headRenderer.sprite = null;
            headRenderer.gameObject.SetActive(false);
            Debug.LogWarning($"[머리 스프라이트 미적용] {headSpriteName}를 {name} 폴더에서 못 찾음");
        }

        string bodySpriteName = $"{body}";
        Sprite bodySprite = LoadSpriteForSpeaker(name, bodySpriteName);
        if (bodySprite != null)
        {
            bodyRenderer.sprite = bodySprite;
            bodyRenderer.gameObject.SetActive(true);
        }
        else
        {
            bodyRenderer.sprite = null;
            bodyRenderer.gameObject.SetActive(false);
            Debug.LogWarning($"[몸통 스프라이트 미적용] {bodySpriteName}를 {name} 폴더에서 못 찾음");
        }

        if (characterPositionManager != null)
        {
            Vector3 basePos = characterPositionManager.GetPositionByCharPos(pos);

            // 머리와 몸의 부모 컨테이너가 동일하다고 가정
            Transform container = headRenderer.transform.parent;
            if (container != null)
            {
                container.position = basePos;

                // 머리와 몸 localPosition을 초기값으로 고정
                headRenderer.transform.localPosition = Vector3.zero;
                bodyRenderer.transform.localPosition = Vector3.zero;
            }

            else//
            {
                Debug.LogWarning("머리 스프라이트에 부모 컨테이너가 없습니다. 위치가 이상할 수 있습니다.");
                // 부모 없으면 기존 방식 유지 (긴급 대비)
                headRenderer.transform.position = basePos;
                bodyRenderer.transform.position = basePos;
            }
        }

        if (effect != Dialog_CharEffect.None)
        {
            StartCoroutine(effectManager.RunCharacterEffect(effect, headRenderer));
            StartCoroutine(effectManager.RunCharacterEffect(effect, bodyRenderer));
        }
    }

    private Sprite LoadSpriteForSpeaker(string speakerName, string spriteName)
    {
        string folderName = speakerName;
        if (characterNameMap.ContainsKey(speakerName))
        {
            folderName = characterNameMap[speakerName];
        }

        string path = $"Sprites/Characters/{folderName}/{spriteName}";
        Sprite sprite = Resources.Load<Sprite>(path);
        if (sprite == null)
        {
            Debug.LogWarning($"[LoadSpriteForSpeaker] 스프라이트를 찾지 못함: {path}");
        }
        return sprite;
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
            SetDialogue(SaveDatabase.Instance.Get_Dialogs_NeedID(currentID));

            if (!dialogueDictByIDAndIndex.TryGetValue((currentID, currentIndex), out currentDialogue) || currentDialogue == null)
            {

                OnOff(false);
                return;
            }
            else
            {
                ShowDialogue(currentID, currentIndex);
            }
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

        //// 화면 효과
        //if (currentDialogue.screenEffect != Dialog_ScreenEffect.None && sprite_BG != null)
        //{
        //    StartCoroutine(effectManager.RunScreenEffect(currentDialogue.screenEffect, sprite_BG));
        //}


        if (currentDialogue.commands == "BGM_OFF")
            soundManager.StopBGM();
        if (soundManager.seSource1.isPlaying)
            soundManager.seSource1.Stop();
        if (soundManager.seSource2.isPlaying)
            soundManager.seSource2.Stop();



        // 사운드
        if (currentDialogue.bgm != null)
            soundManager.PlayDialogSE(currentDialogue.bgm.dialogSE);
        if (currentDialogue.se1 != null)
            soundManager.PlayDialogSE(currentDialogue.se1.dialogSE);
        if (currentDialogue.se2 != null)
            soundManager.PlayDialogSE(currentDialogue.se2.dialogSE);


        ShowDialogue(currentID, currentIndex);
        int nextIDNum = currentDialogue.nextID;  // 이미 int라면 바로 사용 가능
        // 선택지가 있으면 nextDialogueID는 -1 (직접 선택지에서 분기 처리)
        if (currentDialogue.choices != null && currentDialogue.choices.Length > 0)
        {
            isDialogue = true;
            nextDialogueID = -1;  // 선택지가 있으니 자동 진행용 ID는 -1로

        }
        else
        {
            isDialogue = false;

            int? nextIDNullable = currentDialogue.nextID;

            if (currentDialogue.nextID > 0)
            {
                nextDialogueID = currentDialogue.nextID;

                // 만약 nextSheet 값이 있으면 시트 전환
                if (!string.IsNullOrEmpty(currentDialogue.nextSheet?.Trim()))
                {
                    string nextSheetName = currentDialogue.nextSheet.Trim();
                    Debug.Log($"다음 시트로 전환: {nextSheetName}");

                    sheetLoader.LoadNextSheet(nextSheetName);

                    // 대사 초기화
                    currentID = sheetLoader.firstIDOfCurrentSheet;
                    currentIndex = 1; // 보통 1부터 시작

                    ShowDialogue(currentID, currentIndex);
                    sprite_BG.color = new Color(sprite_BG.color.r, sprite_BG.color.g, sprite_BG.color.b, 1f);

                    // UI와 진행 상태 초기화
                    isDialogue = false;
                    nextDialogueID = -1;
                    return;

                }



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
        Debug.Log($"배경키: {bgKey}, sprite_BG.sprite: {sprite_BG.sprite}, sprite_BG.color: {sprite_BG.color}, sprite_BG.activeSelf: {sprite_BG.gameObject.activeSelf}");

    }
    private IEnumerator TypeText(string sentence, int dialogueIndex)
    {
        Debug.Log($"[TypeText] 받은 문장: '{sentence}'");

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

        int i = 0;
        string visibleText = "";
        while (i < sentence.Length)
        {
            while (isPaused)
                yield return null;

            if (sentence[i] == '<') // 태그 시작
            {
                int tagEnd = sentence.IndexOf('>', i);
                if (tagEnd != -1)
                {
                    string tag = sentence.Substring(i, tagEnd - i + 1);
                    visibleText += tag; // 태그 포함
                    i = tagEnd + 1;
                    yield return null;
                    continue;
                }
            }

            visibleText += sentence[i];
            txt_Dialogue.text = visibleText;
            i++;
            yield return new WaitForSeconds(0.05f);
        }

        isTyping = false;

        // --- 선택지 출력 로직 ---
        if (hasChoice && currentDialogue != null && !choicePanel.activeSelf)
        {
            yield return new WaitForSeconds(0.1f);
            waitingForChoiceDisplay = false;
            ShowChoices(currentDialogue.choices, currentDialogue.choiceSoundEffectName);
        }
        else
        {
            canInput = true;
            autoTimer = 0f;
        }

        if (inputQueuedBeforeChoice)
        {
            inputQueuedBeforeChoice = false;
            yield break;
        }
    }







    public void OnUserInput()
    {
        if (isAuto)
        {
            isAuto = false;
            Debug.Log("Auto OFF by user input");
        }
        autoTimer = 0f;
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

        if (isAuto)
        {
            // 타이핑 중이면 자동 진행 안 함
            if (!isTyping && !choicePanel.activeInHierarchy)
            {
                autoTimer += Time.deltaTime;
                if (autoTimer >= autoDelay)
                {
                    autoTimer = 0f;

                    // 다음 대사 진행
                    if (!isProcessingInput && canInput)
                    {
                        StartCoroutine(ProcessInputWithCooldown());
                    }
                }
            }
        }

        // Space 키 입력 처리 (수동 진행)
        if (Input.GetKeyDown(KeyCode.Space))
        {
            autoTimer = 0f; // 입력 있으면 자동 진행 타이머 초기화

            // 타이핑 중이면 무조건 스킵
            if (isTyping)
            {
                if (!isProcessingInput)
                    StartCoroutine(ProcessInputWithCooldown());
                return;
            }

            // 타이핑 완료 후, 선택지 나오기 전 대기 상태일 때
            if (waitingForChoiceDisplay)
            {
                // 대사 강제 출력
                if (typingCoroutine != null)
                    StopCoroutine(typingCoroutine);

                txt_Dialogue.text = dialogueDictByIDAndIndex[(currentID, currentIndex)].dialogue;
                isTyping = false;
                canInput = false; // 선택지 보여주는 쪽으로 입력 넘김

                waitingForChoiceDisplay = false;

                var dialogueData = dialogueDictByIDAndIndex[(currentID, currentIndex)];
                ShowChoices(dialogueData.choices, dialogueData.choiceSoundEffectName);

                return;
            }

            // 선택지 패널이 열려 있으면 입력 무시
            if (choicePanel.activeInHierarchy)
            {
                return;
            }

            // 타이핑 완료, 입력 가능 상태면 다음 대사 진행
            if (canInput)
            {
                if (!isProcessingInput)
                    StartCoroutine(ProcessInputWithCooldown());
            }

            // 유저가 직접 입력했으니 오토 모드 꺼도 괜찮음
            if (isAuto)
            {
                isAuto = false;
                Debug.Log("Auto OFF by user input");
            }
        }
    }



    public void ToggleAuto()
    {
        isAuto = !isAuto;
        if (isAuto)
        {
            Debug.Log("Auto ON");
            autoTimer = 0f; // 켤 때 타이머 초기화
        }
        else
        {
            Debug.Log("Auto OFF");
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

        // 선택지 패널이 열려 있어도, 선택지 뜨기 전 대기 상태면 대사 강제 출력 허용
        if (choicePanel.activeInHierarchy && !waitingForChoiceDisplay) return;

        if (waitingForChoiceDisplay)
        {
            Debug.Log("SkipDialogue 중이지만 waitingForChoiceDisplay가 true → 텍스트는 강제 출력");

            if (typingCoroutine != null)
                StopCoroutine(typingCoroutine);

            txt_Dialogue.text = dialogueDictByIDAndIndex[(currentID, currentIndex)].dialogue;
            isTyping = false;
            canInput = true;

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
                int nextIndex = currentIndex + 1; // 필요하면 nextIndex를 데이터에 맞게 조정하세요
                ShowDialogue(nextDialogueID, nextIndex);
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
                    string nextSheetName = GetNextSheetName(currentID);
                    OnDialogueEnded(nextSheetName);
                    return;

                }
            }
        }

    }


    private string GetNextSheetName(int currentID)
    {
        if (currentID >= 1000 && currentID < 2000) return "START";
        if (currentID >= 2000 && currentID < 3000) return "CHAPTER1";
        if (currentID >= 3000 && currentID < 4000) return "BEFORE_CH1_DEBATE1.2";
        return null; // 더 이상 시트 없음
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

        var img = cutsceneImageObject.GetComponent<Image>();
        if (img == null)
        {
            Debug.LogError("cutsceneImageObject에 Image 컴포넌트가 없습니다!");
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
        img.sprite = cutsceneSprite;
    }


    //
    //컷씬 숨기기 함수
    //
    private void HideCutscene()
    {
        cutsceneImageObject.SetActive(false);
    }


    public AudioSource audioSource;
    public AudioClip choiceSound;

    //
    // 선택지 선택 시 호출되는 함수
    //
    public void OnChoiceSelected(int nextID, int nextIndex)
    {
        // 효과음 재생
        audioSource.PlayOneShot(choiceSound);

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

        NextDialogue();

    }

    void Start()
    {
        sheetLoader = GoogleSheetLoader.Instance;
        if(SaveDatabase.Instance.GetNowSceneName().Contains("PYJ_Dialogue"))
            sheetLoader.OnSheetLoaded += OnSheetLoadedHandler;

        sheetLoader.usingBranching = true; // ✅ 분기 모드로 바로 설정
        sheetLoader.LoadNextSheet("INTRO");

        soundManager = DialogSoundManager.Instance;
    }


    private void OnSheetLoadedHandler()
    {
        // 첫 대사 시작
        currentID = sheetLoader.firstIDOfCurrentSheet; // 예: 1000
        currentIndex = 1;
        NextDialogue();
    }

    void OnDestroy()
    {
        if (sheetLoader != null && SaveDatabase.Instance.GetNowSceneName().Contains("PYJ_Dialogue"))
            sheetLoader.OnSheetLoaded -= OnSheetLoadedHandler;
    }


    private void ShowChoices(DialogueChoice[] choices, string choiceSoundEffectName)
    {
        Debug.Log("ShowChoices 호출됨, 선택지 개수: " + choices.Length);
        canInput = false;  // 입력 잠금
        choicePanel.SetActive(true);

        int countChoices = Mathf.Min(choices.Length, choiceButtons.Length, choiceButtonTexts.Length);

        for (int i = 0; i < countChoices; i++)
        {
            Debug.Log($"선택지[{i}] 텍스트='{choices[i].choiceText}', nextID={choices[i].nextID}, nextIndex={choices[i].nextIndex}");

            int localNextID = choices[i].nextID;
            int localNextIndex = choices[i].nextIndex;

            choiceButtons[i].gameObject.SetActive(true);
            choiceButtonTexts[i].text = choices[i].choiceText;

            if (choices[i].choiceText == "나는 인간이 아니다")
            {
                choiceButtons[i].interactable = false;
            }
            else
            {
                choiceButtons[i].interactable = true;
            }

            choiceButtons[i].onClick.RemoveAllListeners();

            int capturedNextID = localNextID;
            int capturedNextIndex = localNextIndex;

            choiceButtons[i].onClick.AddListener(() =>
            {
                Debug.Log($"선택지 클릭: nextID={capturedNextID}, nextIndex={capturedNextIndex}, soundEffect={choiceSoundEffectName}");

                // 먼저 DialogSE 생성 (clip은 null로)
                DialogSE se = new DialogSE(SEType.SE, null);

                // clip 로드 (로드 과정에서 stopSE 설정 가능)
                se.clip = DialogSoundManager.Instance.LoadAudioClipByName(choiceSoundEffectName, se);

                if (se.stopSE)
                {
                    DialogSoundManager.Instance.StopSE();
                    return;
                }

                if (se.clip != null)
                {
                    DialogSoundManager.Instance.PlaySE(se);
                }

                OnChoiceSelected(capturedNextID, capturedNextIndex);
            });


            Debug.Log($"리스너 등록 완료: 버튼 {i}");
        }

        for (int i = countChoices; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].gameObject.SetActive(false);
        }
    }


    public void OnDialogueEnded(string nextSheetName)
    {
        if (sheetLoader != null && !string.IsNullOrEmpty(nextSheetName))
        {
            sheetLoader.LoadNextSheet(nextSheetName);
        }
        else
        {
            OnOff(false); // 대화 종료 처리
        }
    }


    public void RefreshDialogueDict()
    {
        dialogueDictByIDAndIndex = new Dictionary<(int, int), DialogueData>();

        var dialogs = SaveDatabase.Instance.GetDialogs();

        if (dialogs == null)
        {
            Debug.LogWarning("SaveDatabase.Instance.GetDialogs()가 null입니다.");
            return;
        }

        foreach (var kvp in dialogs)
        {
            int id = kvp.Key;
            foreach (var dialogueData in kvp.Value)
            {
                dialogueDictByIDAndIndex[(id, dialogueData.index)] = dialogueData;
            }
        }

    }


    public void ProcessCommand(string command)
    {
        if (string.IsNullOrEmpty(command))
            return;

        if (command.StartsWith("BGM:"))
        {
            string bgmName = command.Substring("BGM:".Length).Trim();
            PlayBGMByName(bgmName);
        }
        // 다른 명령어 처리...
    }

    public void PlayBGMByName(string bgmName, float volume = 1f, int loopCount = 0)
    {
        // 먼저 DialogSE 생성 (clip은 일단 null)
        DialogSE bgm = new DialogSE(SEType.BGM, null, loopCount, volume);

        // clip 로드 (로드 과정에서 stopSE 설정 가능)
        bgm.clip = DialogSoundManager.Instance.LoadAudioClipByName(bgmName, bgm);

        if (bgm.stopSE)
        {
            // -1 처리 → BGM 중지
            DialogSoundManager.Instance.StopBGM();
            return;
        }

        if (bgm.clip == null)
        {
            Debug.LogWarning($"[DialogueManager] AudioClip '{bgmName}'를 찾을 수 없습니다.");
            return;
        }

        DialogSoundManager.Instance.PlayDialogSE(bgm);
    }
    // 현재 대화 위치부터 이후 대사들 중 선택지가 있는 첫 위치 반환

    private (int id, int index)? FindNextChoicePosition(int startID, int startIndex)
    {
        // 모든 대사를 ID → index 순으로 정렬
        var allKeys = dialogueDictByIDAndIndex.Keys
            .OrderBy(k => k.ID)
            .ThenBy(k => k.index)
            .ToList();

        bool startFound = false;

        foreach (var key in allKeys)
        {
            // 현재 위치 이후부터 탐색 시작
            if (!startFound)
            {
                if (key.ID > startID || (key.ID == startID && key.index >= startIndex))
                    startFound = true;
                else
                    continue;
            }

            var dialogue = dialogueDictByIDAndIndex[key];
            if (dialogue.choices != null && dialogue.choices.Length > 0)
            {
                return (key.ID, key.index); // 첫 번째 선택지 반환
            }
        }

        return null; // 끝까지 못 찾으면
    }


    public void SkipToNextChoice()
    {
        // 🚫 대화 시작 전이거나 데이터가 없는 경우 무시
        if (dialogueDictByIDAndIndex == null || dialogueDictByIDAndIndex.Count == 0)
            return;

        if (!dialogueDictByIDAndIndex.ContainsKey((currentID, currentIndex)))
            return;

        // 타이핑 중이면 먼저 끝내기
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;

            var dialogueData = dialogueDictByIDAndIndex[(currentID, currentIndex)];
            txt_Dialogue.text = dialogueData.dialogue;

            if (dialogueData.choices != null && dialogueData.choices.Length > 0)
            {
                ShowChoices(dialogueData.choices, dialogueData.choiceSoundEffectName);
                return;
            }
        }

        var currentDialogue = dialogueDictByIDAndIndex[(currentID, currentIndex)];
        if (currentDialogue.choices != null && currentDialogue.choices.Length > 0)
        {
            Debug.Log("이미 선택지 구간입니다. 스킵 불가.");
            return;
        }

        var nextChoicePos = FindNextChoicePosition(currentID, currentIndex + 1);
        if (nextChoicePos.HasValue)
        {
            var (id, index) = nextChoicePos.Value;
            JumpToDialogue(id, index);
        }
        else
        {
            Debug.Log("더 이상 선택지가 없습니다.");
        }
    }


    private void JumpToDialogue(int id, int index)
    {
        currentID = id;
        currentIndex = index;

        var dialogueData = dialogueDictByIDAndIndex[(currentID, currentIndex)];
        txt_Dialogue.text = dialogueData.dialogue;
        canInput = false;
        ShowChoices(dialogueData.choices, dialogueData.choiceSoundEffectName);
        isAuto = false;
    }





}
