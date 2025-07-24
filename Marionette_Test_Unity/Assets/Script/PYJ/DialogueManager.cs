using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using System;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueData[] dialogueList;
    [SerializeField] private DialogueEffectManager effectManager;
    [SerializeField] private DialogSoundManager soundManager;

    // 예: 0 = Left, 1 = Center, 2 = Right
    [SerializeField] private SpriteRenderer[] sprite_Characters;
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
    { "이영희", "lee" }
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


    //다음 대화로 넘어가는 함수
    private void NextDialogue()
    {
        var currentDialogue = dialogue[count];

        foreach (var character in sprite_Characters)
        {
            character.sprite = null;
            character.gameObject.SetActive(false);
        }
        Debug.Log($"[NextDialogue] charPos: {currentDialogue.charPos}");

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        string displayName = currentDialogue.characterName;
        txt_CharacterName.text = displayName;

        if (soundManager != null)
        {
            if (currentDialogue.bgm != null && currentDialogue.bgm.clip != null)
                soundManager.PlayDialogSE(currentDialogue.bgm);
            if (currentDialogue.se1 != null && currentDialogue.se1.clip != null)
                soundManager.PlayDialogSE(currentDialogue.se1);
            if (currentDialogue.se2 != null && currentDialogue.se2.clip != null)
                soundManager.PlayDialogSE(currentDialogue.se2);
        }



        // 캐릭터 위치 인덱스 확인
        int posIndex = (int)currentDialogue.charPos;

        // 캐릭터 스프라이트 설정
        if (posIndex >= 0 && posIndex < sprite_Characters.Length)
        {
            SpriteRenderer targetRenderer = sprite_Characters[posIndex];

            string englishName = characterNameMap.ContainsKey(displayName) ? characterNameMap[displayName] : displayName;
            string spriteKey = $"{englishName}_{currentDialogue.status}";
            Debug.Log($"[캐릭터 스프라이트 키] {spriteKey}");

            if (!string.IsNullOrEmpty(spriteKey) && characterSpriteDict.ContainsKey(spriteKey))
            {
                targetRenderer.sprite = characterSpriteDict[spriteKey];
                targetRenderer.gameObject.SetActive(true);
            }
            else
            {
                targetRenderer.sprite = null;
                targetRenderer.gameObject.SetActive(false);
                Debug.LogWarning($"[스프라이트 미적용] {spriteKey}는 등록되지 않았습니다.");
            }

            // 위치 설정
            if (characterPositionManager != null)
            {
                characterPositionManager.SetCharacter(targetRenderer, currentDialogue.charPos);
                Debug.Log($"[DialogueManager] 캐릭터 위치 설정: {currentDialogue.charPos}");
            }

            // 캐릭터 이펙트 적용
            if (currentDialogue.charEffect != Dialog_CharEffect.None)
            {
                StartCoroutine(effectManager.RunCharacterEffect(currentDialogue.charEffect, targetRenderer));
            }
        }
        else if (currentDialogue.charPos == Dialog_CharPos.Clear)
        {
            foreach (var c in sprite_Characters)
            {
                c.sprite = null;
                c.gameObject.SetActive(false);
            }
            Debug.Log("[DialogueManager] 캐릭터 전체 Clear");
        }

        // 배경 설정
        string bgKey = currentDialogue.background;
        Debug.Log($"[배경 키 확인] '{bgKey}'");

        if (!string.IsNullOrEmpty(bgKey) && backgroundSpriteDict.ContainsKey(bgKey))
        {
            sprite_BG.sprite = backgroundSpriteDict[bgKey];
            Debug.Log($"[배경 적용됨] {bgKey}");
        }
        else
        {
            sprite_BG.sprite = null;
            Debug.LogWarning($"[배경 미적용] {bgKey}는 등록되지 않았습니다.");
        }

        // 화면 이펙트 적용 (배경 대상)
        if (currentDialogue.screenEffect != Dialog_ScreenEffect.None && sprite_BG != null)
        {
            StartCoroutine(effectManager.RunScreenEffect(currentDialogue.screenEffect, sprite_BG));
        }

        // 텍스트 타이핑 효과 시작
        typingCoroutine = StartCoroutine(TypeText(currentDialogue.dialogue));

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

    int index = 0; // 예: 첫 번째 캐릭터
    bool flag = true; // 활성화 여부
    private void OnOff(bool flag)
    {
        sprite_DialogueBox.gameObject.SetActive(flag);
        sprite_Characters[index].gameObject.SetActive(flag);
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
