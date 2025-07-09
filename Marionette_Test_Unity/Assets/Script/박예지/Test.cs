using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;

[System.Serializable]
public class Dialogue
{
    public string characterName;

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

    [Header("캐릭터 스프라이트 등록")]
    [SerializeField] private List<Sprite> characterSprites; // 이름: 파일 이름과 동일

    private Dictionary<string, Sprite> characterSpriteDict;

    private void Awake()
    {
        characterSpriteDict = new Dictionary<string, Sprite>();
        foreach (var sprite in characterSprites)
        {
            characterSpriteDict[sprite.name] = sprite;
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

        typingCoroutine = StartCoroutine(TypeText(currentDialogue.dialogue));

        // 캐릭터 이름 출력
        txt_CharacterName.text = currentDialogue.characterName;

        // 이름에 맞는 스프라이트 설정
        if (characterSpriteDict.ContainsKey(currentDialogue.characterName))
        {
            sprite_Character.sprite = characterSpriteDict[currentDialogue.characterName];
        }
        else
        {
            sprite_Character.sprite = null;
        }

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
