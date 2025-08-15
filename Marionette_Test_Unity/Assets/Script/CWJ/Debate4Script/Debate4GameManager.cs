using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections;
using System.Net.NetworkInformation;
public class Debate4GameManager : MonoBehaviour
{
    public Debate4TileButton[] tileButton;
    public Debate4ConfirmButton confirmButton;
    public int CurrentRound;
    public int MaxRound;
    public bool NextRound;
    public bool[] ChoicedAnswer;
    public bool[] CorrectAnswer;
    
    [TextArea]
    public string[] QuestionText;
    public TMP_Text QuestionTextBox;

    void Start()
    {
        NextRound = true;
        CurrentRound = 0;
    }


    void Update()
    {
        if (NextRound == true)
        {
            ShowQuestion();
        }
        else
        {
            for (int i = 0; i < tileButton.Length; i++)
            {
                if (tileButton[i].thistileButtonSelected == true)
                    ChoicedAnswer[i + (CurrentRound*25)] = true;
                else
                    ChoicedAnswer[i + (CurrentRound * 25)] = false;
            }

            if(confirmButton.confirmButtonSelected == true)
                CheckAnswer();
        }
            
    }

    void CheckAnswer()
    {
        bool Wrong = false;
        for (int i = 0; i < tileButton.Length; i++)
        {
            if (CorrectAnswer[i + (CurrentRound * 25)] != tileButton[i].thistileButtonSelected)
            {
                Wrong = true;
            }
        }

        if(Wrong == false)
        {
            if (CurrentRound < MaxRound)
            {
                CurrentRound++;
                NextRound = true;
            }
            else
            {
                QuestionTextBox.text = "검증 완료";
                Debug.Log("현 논쟁 파트 완료"); //다음 논쟁 파트로  
            }
                
            
        }
        else
        {
            Debug.Log("신뢰도 소모"); //신뢰도 소모 추가
        }

        confirmButton.confirmButtonSelected = false;
    }
    void ShowQuestion()
    {
        NextRound = false;
        QuestionTextBox.text = QuestionText[CurrentRound];
        
    }

}
