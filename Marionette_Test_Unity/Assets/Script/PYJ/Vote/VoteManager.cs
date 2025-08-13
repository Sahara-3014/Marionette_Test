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

    public DebateVotingSystem debateVotingSystem; // TempVoteAction이 들어있는 스크립트
    public List<PlayerInfo> playerInfos = new List<PlayerInfo>();
    public GameObject startButton;
    public GameObject playerVoteUIPrefab;
    public List<PlayerVoteUI> playerVoteUIs = new List<PlayerVoteUI>();
    public TextMeshProUGUI centerText;
    public GameObject confirmDialog;
    public TextMeshProUGUI confirmText;
    public GameObject resultDialog;
    public TextMeshProUGUI resultText;
    public GameObject voteEndPanel;

    [Header("※ 진짜 범인을 여기서 고르세요!")]
    public string correctAnswer;

    public Transform playerVoteListParent;
    public List<string> playerNames = new List<string> { "시민1", "시민2", "범인" };

    private bool voteFinished = false;
    private bool endMessageShown = false;
    private bool endPanelShown = false;
    private string votedPlayerName = null;

    private enum VoteState
    {
        WaitingForVote,
        VoteFinishedMessage,
        VoteEndPanelShown,
        FinalResultShown
    }

    private VoteState currentVoteState = VoteState.WaitingForVote;

    #region 투표 UI
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

        // 🔹 투표 수 증가 기능 제거
        // var target = playerVoteUIs.FirstOrDefault(p => p.playerName == votedPlayerName);
        // if (target != null)
        // 
        if (votedPlayerName == correctAnswer)
        {
            voteFinished = true;
            currentVoteState = VoteState.VoteFinishedMessage;

            centerText.text = "투표가 종료되었습니다.";
            resultText.text = "투표가 종료되었습니다.";
            resultDialog.SetActive(true);
        }
        else
        {
            StartCoroutine(ShowRetryDialog());
        }
    }

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

        foreach (var ui in playerVoteUIs)
            ui.EnableButton();
    }
    #endregion

    #region Update
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && voteFinished)
        {
            switch (currentVoteState)
            {
                case VoteState.VoteFinishedMessage:
                    resultDialog.SetActive(false);
                    voteEndPanel.SetActive(true);
                    currentVoteState = VoteState.VoteEndPanelShown;
                    break;

                case VoteState.VoteEndPanelShown:
                    voteEndPanel.SetActive(false);

                    // 투표 종료 후 데드패널 표시
                    debateVotingSystem.TempVoteAction();
                    debateVotingSystem.gameObject.SetActive(true);
                    break;

                case VoteState.FinalResultShown:
                    break;
            }
        }
    }
    #endregion

    #region Start 버튼 클릭
    public void OnClickStartButton()
    {
        Debug.Log("시작 버튼이 눌렸습니다.");

        // 1️⃣ 모든 DeadPanel 끄기 (0번~끝까지)
        for (int i = 0; i < debateVotingSystem.charVotingGauge.Length; i++)
        {
            GameObject deadObj = debateVotingSystem.charVotingGauge[i].transform.parent.parent.GetChild(0).GetChild(0).gameObject;
            deadObj.SetActive(false);
            Debug.Log($"DeadPanel {i} 비활성화 완료");
        }

        // 2️⃣ Start 버튼 숨기기
        startButton.SetActive(false);

        // 3️⃣ 투표 UI 시작
        StartVoting();
    }
    #endregion
}
