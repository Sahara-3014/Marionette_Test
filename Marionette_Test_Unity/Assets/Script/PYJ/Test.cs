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

    [Header("��� ��������Ʈ ���")]
    [SerializeField] private List<Sprite> backgroundSprites;
    private Dictionary<string, Sprite> backgroundSpriteDict;


    [Header("ĳ���� ��������Ʈ ���")]
    [SerializeField] private List<Sprite> characterSprites; // �̸�: ���� �̸��� ����
    private Dictionary<string, Sprite> characterSpriteDict;


    private Dictionary<string, string> characterNameMap = new Dictionary<string, string>()
{
    { "����", "JUHAN" },
    { "�̷�", "MIRAE" },
    { "�̿���", "lee" }
    // �ʿ��� ��ŭ �߰�
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

        // 1. �ѱ� �̸� ǥ�� (��: ȫ�浿)
        string displayName = currentDialogue.characterName;
        txt_CharacterName.text = displayName;

        // 2. ���� �̸����� ���ε� �� �������� (��: hong)
        string englishName = characterNameMap.ContainsKey(displayName) ? characterNameMap[displayName] : displayName;

        // 3. ��������Ʈ Ű ����: hong_����
        string spriteKey = $"{englishName}_{currentDialogue.status}";
        Debug.Log($"[ĳ���� ��������Ʈ Ű] {spriteKey}");

        // 4. ĳ���� ��������Ʈ ����
        if (!string.IsNullOrEmpty(spriteKey) && characterSpriteDict.ContainsKey(spriteKey))
        {
            sprite_Character.sprite = characterSpriteDict[spriteKey];
        }
        else
        {
            sprite_Character.sprite = null;
            Debug.LogWarning($"[��������Ʈ ������] {spriteKey}�� ��ϵ��� �ʾҽ��ϴ�.");
        }

        // 5. ��� ��������Ʈ ����
        string bgKey = currentDialogue.background;
        Debug.Log($"[��� Ű Ȯ��] '{bgKey}'");

        if (!string.IsNullOrEmpty(bgKey) && backgroundSpriteDict.ContainsKey(bgKey))
        {
            sprite_BG.sprite = backgroundSpriteDict[bgKey];
            Debug.Log($"[��� �����] {bgKey}");
        }
        else
        {
            sprite_BG.sprite = null;
            Debug.LogWarning($"[��� ������] {bgKey}�� ��ϵ��� �ʾҽ��ϴ�.");
        }

        // 6. Ÿ�� ȿ�� ����
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
