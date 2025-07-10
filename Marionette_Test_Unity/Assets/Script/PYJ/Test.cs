using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class Dialogue
{
    public string characterName;
    public string status;
    public string background;

    [TextArea]
    public string dialogue;
    public Sprite cg;
}

public class Test : MonoBehaviour
{

    [SerializeField] private SpriteRenderer sprite_Character;
    [SerializeField] private SpriteRenderer sprite_DialogueBox;
    [SerializeField] private TextMeshProUGUI txt_Dialogue;
    [SerializeField] private SpriteRenderer sprite_CharacterNameBox;
    [SerializeField] private TextMeshProUGUI txt_CharacterName;
    [SerializeField] private SpriteRenderer sprite_BG;

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

    public void ShowDialogue()
    {
        OnOff(true);
        count = 0;
        NextDialogue();
    }

    private void NextDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        var currentDialogue = dialogue[count];

        // 1. 한글 이름 표시 (예: 홍길동)
        string displayName = currentDialogue.characterName;
        txt_CharacterName.text = displayName;

        // 2. 영어 이름으로 매핑된 값 가져오기 (예: hong)
        string englishName = characterNameMap.ContainsKey(displayName) ? characterNameMap[displayName] : displayName;

        // 3. 스프라이트 키 생성: hong_웃음
        string spriteKey = $"{englishName}_{currentDialogue.status}";
        Debug.Log($"[캐릭터 스프라이트 키] {spriteKey}");

        // 4. 캐릭터 스프라이트 설정
        if (!string.IsNullOrEmpty(spriteKey) && characterSpriteDict.ContainsKey(spriteKey))
        {
            sprite_Character.sprite = characterSpriteDict[spriteKey];
        }
        else
        {
            sprite_Character.sprite = null;
            Debug.LogWarning($"[스프라이트 미적용] {spriteKey}는 등록되지 않았습니다.");
        }

        // 5. 배경 스프라이트 설정
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

        // 6. 타자 효과 시작
        typingCoroutine = StartCoroutine(TypeText(currentDialogue.dialogue));

        count++;
    }





    private IEnumerator TypeText(string sentence)
    {
        isTyping = true;
        txt_Dialogue.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
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
        if (isDialogue && Input.GetKeyDown(KeyCode.Space))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                txt_Dialogue.text = dialogue[count - 1].dialogue;
                isTyping = false;
                typingCoroutine = null;
            }
            else
            {
                if (count < dialogue.Length)
                {
                    NextDialogue();
                }
                else
                {
                    OnOff(false);
                }
            }
        }
    }
}
