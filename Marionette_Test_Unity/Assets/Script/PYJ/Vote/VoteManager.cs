using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class VoteManager : MonoBehaviour
{
    [System.Serializable]
    public class PlayerInfo
    {
        public string playerName;
        public Sprite characterSprite;
    }

    public List<PlayerInfo> playerInfos = new List<PlayerInfo>();
    public GameObject startButton;
    public GameObject playerVoteUIPrefab;
    public List<PlayerVoteUI> playerVoteUIs = new List<PlayerVoteUI>();
    public TextMeshProUGUI centerText;
    public GameObject confirmDialog;
    public TextMeshProUGUI confirmText;
    public GameObject resultDialog;
    public TextMeshProUGUI resultText;

    [Header("※ 진짜 범인을 여기서 고르세요!")]
    public string correctAnswer;

    public Transform playerVoteListParent;
    public List<string> playerNames = new List<string> { "시민1", "시민2", "범인" };
    public GameObject voteEndPanel;

    private bool voteFinished = false;     // 투표가 완료됐는지
    private bool endMessageShown = false;  // "투표 종료되었습니다." 메시지 표시 여부
    private bool endPanelShown = false;    // 투표 종료 패널 표시 여부

    private string votedPlayerName = null;

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
        if (target != null)
            target.IncreaseVote();

        if (votedPlayerName == correctAnswer)
        {
            // 투표 완료 상태로 변경
            voteFinished = true;
            currentVoteState = VoteState.VoteFinishedMessage;

            // 메시지 띄우기
            centerText.text = "투표가 종료되었습니다.";
            resultText.text = "투표가 종료되었습니다.";
            resultDialog.SetActive(true);
        }
        else
        {
            StartCoroutine(ShowRetryDialog());
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && voteFinished)
        {
            switch (currentVoteState)
            {
                case VoteState.VoteFinishedMessage:
                    // 1번째 스페이스바 → 메시지 닫고 투표 종료 패널 열기
                    resultDialog.SetActive(false);
                    voteEndPanel.SetActive(true);
                    // 숫자 랜덤화는 하지 않는다! 여기선 안 함
                    currentVoteState = VoteState.VoteEndPanelShown;
                    break;

                case VoteState.VoteEndPanelShown:
                    // 2번째 스페이스바 → 종료 패널 닫고 최종 결과 메시지 띄우면서 숫자 랜덤화!
                    voteEndPanel.SetActive(false);

                    centerText.text = "최종 결과입니다.";
                    resultText.text = "투표가 종료되었습니다!";

                    resultDialog.SetActive(true);

                    RandomizeVotes();  // 여기서 숫자 늘리는 함수 호출!

                    currentVoteState = VoteState.FinalResultShown;
                    break;

                case VoteState.FinalResultShown:
                    // 이후 추가 동작
                    break;
            }
        }
    }




    private enum VoteState
    {
        WaitingForVote,        // 투표 진행중
        VoteFinishedMessage,   // "투표가 종료되었습니다." 메시지 띄운 상태
        VoteEndPanelShown,     // 투표 종료 패널 보여준 상태
        FinalResultShown       // 최종 결과 보여준 상태
    }

    private VoteState currentVoteState = VoteState.WaitingForVote;


    public void OnConfirmNo()
    {
        votedPlayerName = null;
        confirmDialog.SetActive(false);
        centerText.text = "다시 선택해주세요.";

        foreach (var ui in playerVoteUIs)
            ui.EnableButton();
    }

    IEnumerator ShowRetryDialog()
    {
        resultDialog.SetActive(true);
        resultText.text = "이 사람은 진짜 범인이 아니야...\n다시 한번 선택해보자.";
        yield return new WaitForSeconds(2f);
        resultDialog.SetActive(false);

        votedPlayerName = null;
        centerText.text = "최종 투표를 진행해주세요.";

        // 버튼 활성화 (투표 다시 받도록)
        foreach (var ui in playerVoteUIs)
            ui.EnableButton();
    }

    public void StartVoting()
    {
        playerVoteListParent.gameObject.SetActive(true);

        foreach (string name in playerNames)
        {
            Sprite foundSprite = null;
            var info = playerInfos.FirstOrDefault(p => p.playerName == name);
            if (info != null)
                foundSprite = info.characterSprite;

            GameObject obj = Instantiate(playerVoteUIPrefab, playerVoteListParent);
            PlayerVoteUI ui = obj.GetComponent<PlayerVoteUI>();
            ui.Setup(name, this, foundSprite);
            playerVoteUIs.Add(ui);
        }

        centerText.text = "최종 투표를 진행해주세요.";
        Debug.Log($"[투표 시작] 정답은: {correctAnswer}");
    }

    private bool canProcessSpace = true;

    IEnumerator ShowResultPanelThenFinalResult()
    {
        voteEndPanel.SetActive(true);
        RandomizeVotes();

        yield return new WaitForSeconds(2f);

        voteEndPanel.SetActive(false);

        centerText.text = "투표가 종료되었습니다.";
        resultText.text = "최종 결과입니다.";
        resultDialog.SetActive(true);

        voteFinished = true;
        endMessageShown = true;
    }


    IEnumerator DelayNextSpaceInput()
    {
        canProcessSpace = false;
        yield return new WaitForSeconds(0.2f);  // 0.2초 대기 (원하는 시간으로 조정)
        canProcessSpace = true;
    }
    void RandomizeVotes()
    {
        foreach (var ui in playerVoteUIs)
        {
            int newVotes = Random.Range(0, 11); // 0~10 랜덤 투표 수
            ui.SetVoteCount(newVotes);
        }
    }

    public void OnClickStartButton()
    {
        Debug.Log("시작 버튼이 눌렸습니다.");
        startButton.SetActive(false);
        StartVoting();
    }
}
