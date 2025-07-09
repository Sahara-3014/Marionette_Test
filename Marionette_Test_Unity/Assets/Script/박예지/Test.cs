using UnityEngine;
using System.Collections;
using TMPro;

[System.Serializable]
public class Dialogue
{
    [TextArea]
    public string dialogue;
    public Sprite cg;
}

public class Test : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sprite_Character;
    [SerializeField] private SpriteRenderer sprite_DialogueBox;
    [SerializeField] private TextMeshProUGUI txt_Dialogue;

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
        typingCoroutine = StartCoroutine(TypeText(dialogue[count].dialogue));
        sprite_Character.sprite = dialogue[count].cg;
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
