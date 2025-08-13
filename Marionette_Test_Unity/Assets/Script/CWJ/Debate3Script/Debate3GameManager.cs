using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Net.NetworkInformation;
public class Debate3GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Debate3ChoiceButton[] choiceButton;
    
    public GameObject choicebuttons;
    public Image choicebutton0, choicebutton1, choicebutton2;
    public Image TimerCircle;
    public int CurrentRound;
    public int MaxRound;
    public bool NextRound;
    private int ChoicedAnswer;
    public int[] CorrectAnswer;
    [TextArea]
    public string[] OppText, ProText;
    public int[] roundStartDialogueStartNum, roundStartDialogueEndNum, choiceDialogueStartNum, choiceDialogueEndNum;
    public string[] ProText1, ProText2, ProText3;
    public TMP_Text OppTextBox, ProTextBox, ProText1Box, ProText2Box, ProText3Box, TimerTextBox;
    public static bool roundStart;
    private bool c1, c2, c3;
    public float timeLimit;
    private float currenttime;
    public Animator TimerUIanimator;
    public Animator BGanimator;
    public Animator ChoiceButtonsAnimator;

    public float textdelay;
    public float linedelay;
    public float nexttextdelay;

    void Start()
    {
        TimerUIanimator.enabled = true;
        TimerUIanimator.SetBool("Intro", true);
        BGanimator.SetBool("Intro", true);

        currenttime = timeLimit;
        NextRound = true;
        roundStart = true;
        
        c1 = true; c2 = true; c3 = true;
        
    }

    void Update()
    {
        TimerCircle.fillAmount = currenttime / timeLimit;
        Debug.Log("TimerCircle.fillAmount: " + currenttime / timeLimit);
        if (!TimerUIanimator.GetCurrentAnimatorStateInfo(0).IsName("TimerUI_Intro"))
        {
            TimerUIanimator.enabled = false;
            BGanimator.SetBool("Intro", false);

            if (NextRound == true)
            {
                
                if (roundStart)
                {
                    roundStart = false;
                    StartCoroutine(RoundStartDialogue());
                }

                Debug.Log("Round:" + CurrentRound);

                string myString = Mathf.FloorToInt(currenttime).ToString();
                if (currenttime < 0)
                    TimerTextBox.text = "0";
                else
                    TimerTextBox.text = myString;

                for (int i = 0; i < choiceButton.Length; i++)
                {
                    if (choiceButton[i].thischoiceButtonSelected == true)
                    {
                        ChoicedAnswer = i;
                        
                        if (NextRound == true)
                            StartCoroutine(ChoiceDialogue());

                        
                    }
                }

                if (currenttime > 0)
                    currenttime -= Time.deltaTime;
                else if (currenttime < 0)
                    currenttime = 0;
            }
            
        }

    }


    public void CheckAnswer()
    {
        if (choiceButton[CorrectAnswer[CurrentRound]].thischoiceButtonSelected == true)
        {
            Debug.Log("Round" + CurrentRound + " 정답!");
            if (CurrentRound < MaxRound)
                CurrentRound++;

            DeselectAllChoiceButtons();
            currenttime = timeLimit;

            c1 = true; c2 = true; c3 = true;

            
            NextRound = true;
            roundStart = true;
        }
        else
        {
            Debug.Log("Round" + CurrentRound + " 오답!");

            switch (ChoicedAnswer)
            {
                case 0:
                    c1 = false;
                    break;
                case 1:
                    c2 = false;
                    break;
                case 2:
                    c3 = false;
                    break;
            }

            DeselectAllChoiceButtons();
            currenttime = timeLimit;
            ShowAllChoiceButtons(); //choiceAppear = true;
            NextRound = true;
        }
    }

    public void DeselectAllChoiceButtons()
    {
        for (int i = 0; i < choiceButton.Length; i++)
        {
            choiceButton[i].thischoiceButtonSelected = false;
            Debug.Log("i: " + i);
        }
    }
    public void ShowAllChoiceButtons()
    {
        ChoiceButtonsAnimator.Play("ChoiceButton_Appear");

        if (c1 == true)
            choicebutton0.gameObject.SetActive(true);

        if (c2 == true)
            choicebutton1.gameObject.SetActive(true);

        if (c3 == true)
            choicebutton2.gameObject.SetActive(true);
        
    }
    public void HideAllChoiceButtons()
    {
        choicebutton0.gameObject.SetActive(false);
        choicebutton1.gameObject.SetActive(false);
        choicebutton2.gameObject.SetActive(false);
    }

    public IEnumerator RoundStartDialogue()
    {
        HideAllChoiceButtons(); //choiceAppear = false;
        // 대사 해야하는 게 라운드 시작 때 그리고 답 선택 때
        for (int i = roundStartDialogueStartNum[CurrentRound]; i <= roundStartDialogueEndNum[CurrentRound]; i++)
        {
            Debug.Log("Print RoundStartDialogue Text" + i);
            yield return StartCoroutine(PrintingText(OppText[i], OppTextBox));
            yield return StartCoroutine(PrintingText(ProText[i], ProTextBox));

            yield return new WaitForSeconds(nexttextdelay);
        }
        
        ProText1Box.text = ProText1[CurrentRound];
        ProText2Box.text = ProText2[CurrentRound];
        ProText3Box.text = ProText3[CurrentRound];

        ShowAllChoiceButtons(); //choiceAppear = true;
    }

    public IEnumerator ChoiceDialogue()
    {
        HideAllChoiceButtons(); //choiceAppear = false;
        NextRound = false;
        int i = CurrentRound * 3 + ChoicedAnswer;
        for(int j = choiceDialogueStartNum[i]; j <= choiceDialogueEndNum[i]; j++)
        {
            Debug.Log("Print ChoiceDialogue Text" + j);
            yield return StartCoroutine(PrintingText(OppText[j], OppTextBox));
            yield return StartCoroutine(PrintingText(ProText[j], ProTextBox));

            yield return new WaitForSeconds(nexttextdelay);
        }
        
        CheckAnswer();
    }
    IEnumerator PrintingText(string printText, TMP_Text TextBox)
    {
        TextBox.text = "";
        foreach(char letter in printText.ToCharArray())
        {
            if (letter == '\\')
            {
                TextBox.text += '\n';
                yield return new WaitForSeconds(linedelay);
            }
            else
                TextBox.text += letter;
            
            yield return new WaitForSeconds(textdelay);
        }
    }


}
