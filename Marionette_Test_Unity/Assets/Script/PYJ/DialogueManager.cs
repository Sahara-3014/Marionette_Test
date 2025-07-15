using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using System;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueData[] dialogueList;
    [SerializeField] private EffectManager effectManager;
    [SerializeField] private DialogSoundManager soundManager;

    // ��: 0 = Left, 1 = Center, 2 = Right
    [SerializeField] private SpriteRenderer[] sprite_Characters;
    [SerializeField] private SpriteRenderer sprite_BG;
    [SerializeField] private SpriteRenderer sprite_DialogueBox;
    [SerializeField] private SpriteRenderer sprite_CharacterNameBox;

    [SerializeField] private TextMeshProUGUI txt_Dialogue;
    [SerializeField] private TextMeshProUGUI txt_CharacterName;

    [SerializeField] private CharacterPositionManager characterPositionManager;

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

    [SerializeField] private DialogueData[] dialogue;

    private Coroutine typingCoroutine;

    public void SetDialogue(DialogueData[] newDialogue)
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



        // ĳ���� ��ġ �ε��� Ȯ��
        int posIndex = (int)currentDialogue.charPos;

        // ĳ���� ��������Ʈ ����
        if (posIndex >= 0 && posIndex < sprite_Characters.Length)
        {
            SpriteRenderer targetRenderer = sprite_Characters[posIndex];

            string englishName = characterNameMap.ContainsKey(displayName) ? characterNameMap[displayName] : displayName;
            string spriteKey = $"{englishName}_{currentDialogue.status}";
            Debug.Log($"[ĳ���� ��������Ʈ Ű] {spriteKey}");

            if (!string.IsNullOrEmpty(spriteKey) && characterSpriteDict.ContainsKey(spriteKey))
            {
                targetRenderer.sprite = characterSpriteDict[spriteKey];
                targetRenderer.gameObject.SetActive(true);
            }
            else
            {
                targetRenderer.sprite = null;
                targetRenderer.gameObject.SetActive(false);
                Debug.LogWarning($"[��������Ʈ ������] {spriteKey}�� ��ϵ��� �ʾҽ��ϴ�.");
            }

            // ��ġ ����
            if (characterPositionManager != null)
            {
                characterPositionManager.SetCharacter(targetRenderer, currentDialogue.charPos);
                Debug.Log($"[DialogueManager] ĳ���� ��ġ ����: {currentDialogue.charPos}");
            }

            // ĳ���� ����Ʈ ����
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
            Debug.Log("[DialogueManager] ĳ���� ��ü Clear");
        }

        // ��� ����
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

        // ȭ�� ����Ʈ ���� (��� ���)
        if (currentDialogue.screenEffect != Dialog_ScreenEffect.None && sprite_BG != null)
        {
            StartCoroutine(effectManager.RunScreenEffect(currentDialogue.screenEffect, sprite_BG));
        }

        // �ؽ�Ʈ Ÿ���� ȿ�� ����
        typingCoroutine = StartCoroutine(TypeText(currentDialogue.dialogue));

        count++;
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

    int index = 0; // ��: ù ��° ĳ����
    bool flag = true; // Ȱ��ȭ ����
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


    //��� ����,�簳 �Լ�
    private bool isPaused = false;

    public void TogglePauseDialogue()
    {
        isPaused = !isPaused;
        Debug.Log(isPaused ? " �Ͻ�������" : " �ٽ� �����");
    }




}
