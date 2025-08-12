using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Debate3GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Debate3ChoiceButton[] choiceButton;
    public int Defaultchoice;
    public int CurrentRound;
    public int MaxRound;
    public bool NextRound;
    public int[] CorrectAnswer;
    public string[] OppText, ProText1, ProText2, ProText3;
    public TMP_Text OppTextBox, ProText1Box, ProText2Box, ProText3Box, TimerTextBox;
    public float timeLimit;
    private float currenttime;
    void Start()
    {
        currenttime = timeLimit;
        NextRound = true;
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
        Debug.Log("Round:" + CurrentRound);
        OppTextBox.text = OppText[CurrentRound];
        ProText1Box.text = ProText1[CurrentRound];
        ProText2Box.text = ProText2[CurrentRound];
        ProText3Box.text = ProText3[CurrentRound];
        string myString = Mathf.FloorToInt(currenttime).ToString();
        TimerTextBox.text = myString;
        if (NextRound == true)
        {
            for (int i = 0; i < choiceButton.Length; i++)
            {
                if (choiceButton[i].thischoiceButtonSelected == true)
                {
                    CheckAnswer();
                }
            }
        }
        
            if(currenttime>0)
            currenttime -= Time.deltaTime;
        

    }

    public void CheckAnswer()
    {
        NextRound = false;

        if (choiceButton[CorrectAnswer[CurrentRound]].thischoiceButtonSelected == true)
        {
            Debug.Log("Round" + CurrentRound + " 정답!");
            if (CurrentRound < MaxRound)
                CurrentRound++;

            DeselectAllChoiceButtons();
            currenttime = 30;
            if (currenttime >= 100)
                currenttime = 99;

            NextRound = true;
        }
        else
        {
            Debug.Log("Round" + CurrentRound + " 오답!");
            if (CurrentRound > 0)
                CurrentRound--;

            DeselectAllChoiceButtons();
            currenttime += 0;
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

}
