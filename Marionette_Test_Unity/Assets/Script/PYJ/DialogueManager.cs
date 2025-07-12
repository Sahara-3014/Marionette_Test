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
    public string characterName;     // ��� ȭ��
    public string status;            // ĳ���� ���� (ǥ�� ��)
    public string background;        // ��� Ű

    [TextArea]
    public string dialogue;          // ��� ����
    public Sprite cg;                // ���� �̹��� (�ɼ�)

    public Dialog_ScreenEffect screenEffect = Dialog_ScreenEffect.None;
    public Dialog_CharEffect charEffect = Dialog_CharEffect.None;

    public float delay = 0.05f;      // Ÿ���� �ӵ�
    public float duration = 0f;      // ��� ���� �ð�

    public DialogSE se1;             // ȿ���� 1
    public DialogSE se2;             // ȿ���� 2
    public DialogSE bgm;             // ������� ��ȯ

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


    //��� �����ִ� �Լ�
    public void ShowDialogue()
    {
        OnOff(true);
        count = 0;
        NextDialogue();
    }


    //���� ��ȭ�� �Ѿ�� �Լ�
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
        Debug.Log($"[ĳ���� ��������Ʈ Ű] {spriteKey}");

        if (!string.IsNullOrEmpty(spriteKey) && characterSpriteDict.ContainsKey(spriteKey))
        {
            sprite_Character.sprite = characterSpriteDict[spriteKey];
        }
        else
        {
            sprite_Character.sprite = null;
            Debug.LogWarning($"[��������Ʈ ������] {spriteKey}�� ��ϵ��� �ʾҽ��ϴ�.");
        }

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

        //  ����Ʈ ����
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


    //��縦 �� �پ� ������ �ϴ� �Լ�
    private IEnumerator TypeText(string sentence)
    {
        isTyping = true;
        txt_Dialogue.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            //  �Ͻ������� �� ���
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
            SkipDialogue(); // �����̽� �Է����� SkipDialogue ȣ��
        }
    }


    //��� ��ŵ �Լ�
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
                OnOff(false); // ��� ����
            }
        }
    }


    //��� ����/�簳 �Լ�
    private bool isPaused = false;

    public void TogglePauseDialogue()
    {
        isPaused = !isPaused;
        Debug.Log(isPaused ? " �Ͻ�������" : " �ٽ� �����");
    }




}
