using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using System;

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

    public void SetDialogue(DialogueData[] newDialogue)
    {
        dialogue = newDialogue;
    }


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
        // ✅ 배열 범위 초과 방지
        if (count >= dialogue.Length)
        {
            OnOff(false); // 대사 종료 처리
            return;
        }

        var currentDialogue = dialogue[count];

        // ✅ 현재 등장하는 위치만 추적
        bool[] posUsed = new bool[sprite_Heads.Length];

        // ✅ 캐릭터 출력 (머리 + 몸)
        if (currentDialogue.characters != null)
        {
            foreach (var ch in currentDialogue.characters)
            {
                int pos = (int)ch.position;
                if (pos >= 0 && pos < posUsed.Length)
                {
                    posUsed[pos] = true;
                    ShowCharacter(ch.name, ch.head, ch.body, ch.position, currentDialogue.charEffect);
                }
                else
                {
                    Debug.LogWarning($"[오류] 유효하지 않은 캐릭터 위치: {pos}");
                }
            }
        }

        // ✅ 등장하지 않은 위치는 끄기
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

        // ✅ 대사 관련 텍스트 및 배경 처리
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

        // ✅ 배경 효과
        if (currentDialogue.screenEffect != Dialog_ScreenEffect.None && sprite_BG != null)
        {
            StartCoroutine(effectManager.RunScreenEffect(currentDialogue.screenEffect, sprite_BG));
        }

        // ✅ 사운드 처리
        if (currentDialogue.bgm != null)
            soundManager.PlayDialogSE(currentDialogue.bgm);
        if (currentDialogue.se1 != null)
            soundManager.PlayDialogSE(currentDialogue.se1);
        if (currentDialogue.se2 != null)
            soundManager.PlayDialogSE(currentDialogue.se2);

        // ✅ 텍스트 출력
        typingCoroutine = StartCoroutine(TypeText(currentDialogue.dialogue));

        // ✅ 다음 대사 인덱스로 이동
        count++;
    }







    //대사를 한 줄씩 나오게 하는 함수
    private IEnumerator TypeText(string sentence)
    {
        isTyping = true;
        txt_Dialogue.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            //  일시정지일 때 대기
            while (isPaused)
            {
                yield return null;
            }

            txt_Dialogue.text += letter;
            yield return new WaitForSeconds(0.05f);
        }

        isTyping = false;
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




}
