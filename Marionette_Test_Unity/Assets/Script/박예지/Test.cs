using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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
    [SerializeField] private Text txt_Dialogue;

    private bool isDialogue = false;

    private int count = 0;
    [SerializeField] private Dialogue[] dialogue;

    

    public void ShowDialogue()
    {
        //sprite_DialogueBox.gameObject.SetActive(true);
        //sprite_Character.gameObject.SetActive(true);
        //txt_Dialogue.gameObject.SetActive(true);
        OnOff(true);
        count = 0;
        //isDialogue = true;
        NextDialogue();

    }

    //private void HideDialogue()
    //{
    //    sprite_DialogueBox.gameObject.SetActive(false);
    //    sprite_Character.gameObject.SetActive(false);
    //    txt_Dialogue.gameObject.SetActive(false);

    //    isDialogue = false;
    //}

    private void NextDialogue()
    {
        txt_Dialogue.text = dialogue[count].dialogue;
        sprite_Character.sprite = dialogue[count].cg;
        count++;
    }


    private void OnOff(bool _flag)
    {
        sprite_DialogueBox.gameObject.SetActive(_flag);
        sprite_Character.gameObject.SetActive(_flag);
        txt_Dialogue.gameObject.SetActive(_flag);
        isDialogue = _flag;
    }




    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isDialogue)
        {
            if (Input.GetKeyDown(KeyCode.Space))
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
