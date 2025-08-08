using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class VoteManager : MonoBehaviour
{
    public GameObject startButton; // 👈 인스펙터에서 연결할 버튼

    public GameObject playerVoteUIPrefab;
    public List<PlayerVoteUI> playerVoteUIs = new List<PlayerVoteUI>(); // 초기화 추가
    public TextMeshProUGUI centerText;
    public GameObject confirmDialog;
    public TextMeshProUGUI confirmText;
    public GameObject resultDialog;
    public TextMeshProUGUI resultText;

    private string votedPlayerName = null;
    private string correctAnswer = "범인";

    public Transform playerVoteListParent;
    public List<string> playerNames = new List<string> { "시민1", "시민2", "범인" };

    public void OnVoteButtonClicked(string targetPlayerName)
    {
        if (votedPlayerName != null) return;
        confirmDialog.SetActive(true);
        confirmText.text = $"{targetPlayerName}에게 투표하시겠습니까?";
        votedPlayerName = targetPlayerName;
    }

    public void OnConfirmYes()
    {
        confirmDialog.SetActive(false);

        var target = playerVoteUIs.FirstOrDefault(p => p.playerName == votedPlayerName);
        if (target != null) target.IncreaseVote();

        centerText.text = "투표가 완료되었습니다.";

        if (votedPlayerName != correctAnswer)
        {
            StartCoroutine(ShowRetryDialog());
        }
    }

    public void OnConfirmNo()
    {
        votedPlayerName = null;
        confirmDialog.SetActive(false);
    }

    IEnumerator ShowRetryDialog()
    {
        yield return new WaitForSeconds(1f);
        resultDialog.SetActive(true);
        resultText.text = "이 사람은 진짜 범인이 아니야...\n다시 한번 선택해보자.";
        yield return new WaitForSeconds(2f);
        resultDialog.SetActive(false);
        votedPlayerName = null;
    }

    public void StartVoting()
    {
        playerVoteListParent.gameObject.SetActive(true);  // << 여기 추가

        foreach (string name in playerNames)
        {
            GameObject obj = Instantiate(playerVoteUIPrefab, playerVoteListParent);
            PlayerVoteUI ui = obj.GetComponent<PlayerVoteUI>();
            ui.Setup(name, this);
            playerVoteUIs.Add(ui);
        }

        centerText.text = "최종 투표를 진행해주세요.";
    }


    public void OnClickStartButton()
    {
        Debug.Log("시작 버튼이 눌렸습니다.");

        startButton.SetActive(false);
        StartVoting();
    }
}
