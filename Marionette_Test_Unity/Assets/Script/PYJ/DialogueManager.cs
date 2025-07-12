using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;

public enum Dialog_ScreenEffect
{
    ShakeVertical, ShakeHorizontal, Shake, None, ClearAll, FadeOutAll,
    OtherColorEnable, OtherColorDisable, AllColorEnable, AllColorDisable
}

public enum Dialog_CharEffect
{
    Jump, ShakeVertical, ShakeHorizontal, Shake, MoveOut2Left, MoveOut2Right,
    MoveLeft2Out, MoveRight2Out, MoveVertical, MoveUp, FadeIn, FadeOut,
    ColorEnable, ColorDisable, None, MoveDown
}
public enum Dialog_CharPos
{ Left = 0, Center = 1, Right = 2, New = -1, None = -2, Clear = -3 }


[System.Serializable]
public class Dialogue
{
    public string characterName;     // 대사 화자
    public string status;            // 캐릭터 상태 (표정 등)
    public string background;        // 배경 키

    [TextArea]
    public string dialogue;          // 대사 본문
    public Sprite cg;                // 삽입 이미지 (옵션)

    public Dialog_ScreenEffect screenEffect = Dialog_ScreenEffect.None;
    public Dialog_CharEffect charEffect = Dialog_CharEffect.None;

    public float delay = 0.05f;      // 타이핑 속도
    public float duration = 0f;      // 대사 유지 시간

    public DialogSE se1;             // 효과음 1
    public DialogSE se2;             // 효과음 2
    public DialogSE bgm;             // 배경음악 전환

}


public class DialogueManager : MonoBehaviour
{
    [SerializeField] private Dialogue[] dialogueList;
    [SerializeField] private EffectManager effectManager;
    [SerializeField] private DialogSoundManager soundManager;

    [SerializeField] private SpriteRenderer sprite_Character;
    [SerializeField] private SpriteRenderer sprite_BG;
    [SerializeField] private SpriteRenderer sprite_DialogueBox;
    [SerializeField] private SpriteRenderer sprite_CharacterNameBox;

    [SerializeField] private TextMeshProUGUI txt_Dialogue;
    [SerializeField] private TextMeshProUGUI txt_CharacterName;


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

    [SerializeField] private Dialogue[] dialogue;

    private Coroutine typingCoroutine;

    public void SetDialogue(Dialogue[] newDialogue)
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

        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        var currentDialogue = dialogue[count];

        string displayName = currentDialogue.characterName;
        txt_CharacterName.text = displayName;

        if (soundManager != null)
        {
            if (currentDialogue.bgm != null)
                soundManager.PlayDialogSE(currentDialogue.bgm);
            if (currentDialogue.se1 != null)
                soundManager.PlayDialogSE(currentDialogue.se1);
            if (currentDialogue.se2 != null)
                soundManager.PlayDialogSE(currentDialogue.se2);
        }

        string englishName = characterNameMap.ContainsKey(displayName) ? characterNameMap[displayName] : displayName;
        string spriteKey = $"{englishName}_{currentDialogue.status}";
        Debug.Log($"[캐릭터 스프라이트 키] {spriteKey}");

        if (!string.IsNullOrEmpty(spriteKey) && characterSpriteDict.ContainsKey(spriteKey))
        {
            sprite_Character.sprite = characterSpriteDict[spriteKey];
        }
        else
        {
            sprite_Character.sprite = null;
            Debug.LogWarning($"[스프라이트 미적용] {spriteKey}는 등록되지 않았습니다.");
        }

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

        //  이펙트 실행
        StartCoroutine(effectManager.RunScreenEffect(currentDialogue.screenEffect, sprite_BG));
        StartCoroutine(effectManager.RunCharacterEffect(currentDialogue.charEffect, sprite_Character));


        typingCoroutine = StartCoroutine(TypeText(currentDialogue.dialogue));
        count++;
    }



    private IEnumerator FadeInCharacter()
    {
        float time = 0;
        Color c = sprite_Character.color;
        c.a = 0;
        sprite_Character.color = c;

        while (time < 1f)
        {
            time += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, time);
            sprite_Character.color = c;
            yield return null;
        }
    }
    private IEnumerator FadeOutCharacter()
    {
        float time = 0;
        Color c = sprite_Character.color;
        c.a = 1f;
        sprite_Character.color = c;

        while (time < 1f)
        {
            time += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, time);
            sprite_Character.color = c;
            yield return null;
        }
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
        sprite_Character.gameObject.SetActive(flag);
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


    //대사 정지/재개 함수
    private bool isPaused = false;

    public void TogglePauseDialogue()
    {
        isPaused = !isPaused;
        Debug.Log(isPaused ? " 일시정지됨" : " 다시 재생됨");
    }




}
