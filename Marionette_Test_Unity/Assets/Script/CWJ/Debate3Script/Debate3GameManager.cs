using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Net.NetworkInformation;
public class PrintText : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Debate3ChoiceButton[] choiceButton;
    public GameObject choicebuttons, choicebutton0, choicebutton1, choicebutton2;
    public int CurrentRound;
    public int MaxRound;
    public bool NextRound;
    private int ChoicedAnswer;
    public int[] CorrectAnswer;
    public string[] OppText, ProText;
    public int[] roundStartDialogueStartNum, roundStartDialogueEndNum, choiceDialogueStartNum, choiceDialogueEndNum;
    public string[] ProText1, ProText2, ProText3;
    public TMP_Text OppTextBox, ProTextBox, ProText1Box, ProText2Box, ProText3Box, TimerTextBox;
    private bool roundStart, choiceAppear;

    public float timeLimit;
    private float currenttime;
    public Animator TimerUIanimator;
    public Animator BGanimator;

    public float textdelay;
    public float nexttextdelay;
    void Start()
    {
        TimerUIanimator.SetBool("Intro", true);
        BGanimator.SetBool("Intro", true);

        currenttime = timeLimit;
        NextRound = true;
        roundStart = true;
        choiceAppear = true; //임시
        /*choiceButton[Defaultchoice].thischoiceButtonSelected = true;
        for (int i = 0; i < choiceButton.Length; i++)
        {
            if (i != Defaultchoice)
            {
                choiceButton[i].thischoiceButtonSelected = false;
            }
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        if(!TimerUIanimator.GetCurrentAnimatorStateInfo(0).IsName("TimerUI_Intro"))
        {
            BGanimator.SetBool("Intro", false);
            if (choiceAppear)
                    choicebuttons.SetActive(true);
                else
                    choicebuttons.SetActive(false);

            if (NextRound == true)
            {
                if (roundStart)
                {
                    roundStart = false;
                    StartCoroutine(RoundStartDialogue());
                }
                


                Debug.Log("Round:" + CurrentRound);

                /*OppTextBox.text = OppText[CurrentRound];
                 ProTextBox.text = ProText[CurrentRound];*/

            
                string myString = Mathf.FloorToInt(currenttime).ToString();
                TimerTextBox.text = myString;

                for (int i = 0; i < choiceButton.Length; i++)
                {
                    if (choiceButton[i].thischoiceButtonSelected == true)
                    {
                        ChoicedAnswer = i;

                        if(NextRound == true)
                            StartCoroutine(ChoiceDialogue());
                    }
                }
            }

            if (currenttime > 1)
                currenttime -= Time.deltaTime;


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
            

            
            NextRound = true;
            roundStart = true;
            choicebutton0.SetActive(true);
            choicebutton1.SetActive(true);
            choicebutton2.SetActive(true);
        }
        else
        {
            Debug.Log("Round" + CurrentRound + " 오답!");

            switch (ChoicedAnswer)
            {
                case 0:
                    choicebutton0.SetActive(false);
                    break;
                case 1:
                    choicebutton1.SetActive(false);
                    break;
                case 2:
                    choicebutton2.SetActive(false);
                    break;
            }


            DeselectAllChoiceButtons();
            currenttime = timeLimit;
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

    public IEnumerator RoundStartDialogue()
    {
        // 대사 해야하는 게 라운드 시작 때 그리고 답 선택 때
        for(int i = roundStartDialogueStartNum[CurrentRound]; i <= roundStartDialogueEndNum[CurrentRound]; i++)
        {
            Debug.Log("Print RoundStartDialogue Text" + i);
            yield return StartCoroutine(PrintingText(OppText[i], OppTextBox));
            yield return StartCoroutine(PrintingText(ProText[i], ProTextBox));

            yield return new WaitForSeconds(nexttextdelay);
        }
        
        ProText1Box.text = ProText1[CurrentRound];
        ProText2Box.text = ProText2[CurrentRound];
        ProText3Box.text = ProText3[CurrentRound];
        
    }

    public IEnumerator ChoiceDialogue()
    {
        NextRound = false;
        int i = CurrentRound * 3 + ChoicedAnswer;
        for (int j = choiceDialogueStartNum[i]; j <= choiceDialogueEndNum[i]; j++)
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
        foreach (char letter in printText.ToCharArray())
        {
            TextBox.text += letter;
            yield return new WaitForSeconds(textdelay);
        }
    }
}
