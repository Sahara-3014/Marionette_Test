using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public void SetDialogue(DialogueData[] newDialogue)
    {
        dialogue = newDialogue;
    }

    [System.Serializable]
    public class CharacterStatus
    {
        public string name;
        public string head;
        public string body;
        public Dialog_CharPos position;
    }

    [SerializeField] private GameObject cutsceneImageObject; // UI Image or SpriteRenderer
    [SerializeField] private SpriteRenderer[] characterRenderers;

    [SerializeField] private DialogueData[] dialogueList;
    [SerializeField] private DialogEffectManager effectManager;
    [SerializeField] private DialogSoundManager soundManager;

    // 예: 0 = Left, 1 = Center, 2 = Right
    // 캐릭터 위치당 스프라이트 두 개 (머리, 몸)
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



    private bool isDialogue = false;
    private bool isTyping = false;
    private int count = 0;

    [SerializeField] private DialogueData[] dialogue;

    private Coroutine typingCoroutine;

    //대사 보여주는 함수
    public void ShowDialogue()
    {
        OnOff(true);
        count = 0;
        NextDialogue();
    }

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


    //다음 대화로 넘어가는 함수
    private void NextDialogue()
    {
        // ✅ 모든 대사 끝났을 때
        if (count >= dialogue.Length)
        {
            Debug.Log("대사 끝났음");
            OnOff(false);
            return;
        }

        var currentDialogue = dialogue[count];
        Debug.Log($"[1] nextSheet = {currentDialogue.nextSheet}");
        // ✅ 다음 대사 미리 준비
        count++;
        //if (currentDialogue.choices != null && currentDialogue.choices.Length > 0)
        //{
        //    ShowChoices(currentDialogue.choices);
        //    return; // 선택지 뜨면 대사 진행 멈춤
        //}
        // ✅ 마지막 대사였고, nextSheet가 존재하면
        if (count >= dialogue.Length && !string.IsNullOrEmpty(currentDialogue.nextSheet))
        {
            Debug.Log($"다음 시트로 이동: {currentDialogue.nextSheet}");
            OnOff(false);

            GoogleSheetLoader sheetLoader = UnityEngine.Object.FindFirstObjectByType<GoogleSheetLoader>();
            if (sheetLoader != null)
            {
                sheetLoader.LoadNextSheet(currentDialogue.nextSheet);
            }
            else
            {
                Debug.LogError("GoogleSheetLoader를 찾을 수 없습니다!");
            }

            return;
        }

        // ✅ 캐릭터 등장 위치 추적
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
        // ✅ 컷씬 처리: 여기가 추가되는 부분!
        if (!string.IsNullOrEmpty(currentDialogue.cutscene))
        {
            ShowCutscene(currentDialogue.cutscene);
        }
        else
        {
            HideCutscene();
        }

        // ✅ 이름/배경
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

        // ✅ 화면 효과
        if (currentDialogue.screenEffect != Dialog_ScreenEffect.None && sprite_BG != null)
        {
            StartCoroutine(effectManager.RunScreenEffect(currentDialogue.screenEffect, sprite_BG));
        }

        // ✅ 사운드
        if (currentDialogue.bgm != null)
            soundManager.PlayDialogSE(currentDialogue.bgm);
        if (currentDialogue.se1 != null)
            soundManager.PlayDialogSE(currentDialogue.se1);
        if (currentDialogue.se2 != null)
            soundManager.PlayDialogSE(currentDialogue.se2);

        // ✅ 텍스트 출력
        typingCoroutine = StartCoroutine(TypeText(currentDialogue.dialogue));
    }








    //대사를 한 줄씩 나오게 하는 함수
    private IEnumerator TypeText(string sentence)
    {
        isTyping = true;
        txt_Dialogue.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            while (isPaused)
                yield return null;

            txt_Dialogue.text += letter;
            yield return new WaitForSeconds(0.05f);
        }

        isTyping = false;

        // ✅ 텍스트 출력이 끝났을 때 선택지 있으면 보여주기
        var currentDialogue = dialogue[count - 1];
        if (currentDialogue.choices != null && currentDialogue.choices.Length > 0)
        {
            ShowChoices(currentDialogue.choices);
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



    void Update()
    {
        if (!isDialogue || isPaused) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SkipDialogue(); // 스페이스 입력으로 SkipDialogue 호출
        }
    }


    //대사 스킵 함수
    public void SkipDialogue()
    {
        if (!isDialogue) return;

        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            txt_Dialogue.text = dialogue[count - 1].dialogue;
            isTyping = false;
            typingCoroutine = null;
        }
       
        {
            if (count < dialogue.Length)
            {
                NextDialogue();
            }
            else
            {
                OnOff(false); // 대사 종료
            }
        }
    }


    //대사 정지,재개 함수
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



    private void HideCutscene()
    {
        cutsceneImageObject.SetActive(false);
    }

    private void OnChoiceSelected(string nextSheet)
    {
        choicePanel.SetActive(false);

        if (!string.IsNullOrEmpty(nextSheet))
        {
            GoogleSheetLoader sheetLoader = UnityEngine.Object.FindFirstObjectByType<GoogleSheetLoader>();
            if (sheetLoader != null)
            {
                sheetLoader.LoadNextSheet(nextSheet);
            }
            else
            {
                Debug.LogError("GoogleSheetLoader를 찾을 수 없습니다!");
            }
        }

        OnOff(true); // 대사 UI 다시 켜기 (필요하면)
    }


    private void ShowChoices(DialogueChoice[] choices)
    {
        choicePanel.SetActive(true);

        int count = Mathf.Min(choices.Length, choiceButtons.Length, choiceButtonTexts.Length);

        for (int i = 0; i < count; i++)
        {
            choiceButtons[i].gameObject.SetActive(true);
            choiceButtonTexts[i].text = choices[i].choiceText;

            string nextSheet = choices[i].nextSheet;  // 지역 변수 복사

            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(nextSheet));
        }

        for (int i = count; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].gameObject.SetActive(false);
        }
    }




}
