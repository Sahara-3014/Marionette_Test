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

        centerText.text = "투표가 종료되었습니다.";   // 메시지 변경
        resultText.text = "투표가 종료되었습니다.";   // 결과 텍스트도 변경
        resultDialog.SetActive(true);                // 결과창 바로 활성화

        voteFinished = true;
        endMessageShown = true;   // 이걸 추가해서 Update()에서 또 띄우는 중복 방지
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
        yield return new WaitForSeconds(1f);
        resultDialog.SetActive(true);
        resultText.text = "이 사람은 진짜 범인이 아니야...\n다시 한번 선택해보자.";
        yield return new WaitForSeconds(2f);
        resultDialog.SetActive(false);
        votedPlayerName = null;
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

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (voteFinished && endMessageShown && !endPanelShown)
            {
                endPanelShown = true;
                resultDialog.SetActive(false);
                voteEndPanel.SetActive(true);
                RandomizeVotes();
                Debug.Log("Show voteEndPanel and update votes");
            }
        }
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
