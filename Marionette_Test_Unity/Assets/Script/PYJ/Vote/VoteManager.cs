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

    public GameObject startButton; // 👈 인스펙터에서 연결할 버튼

    public GameObject playerVoteUIPrefab;
    public List<PlayerVoteUI> playerVoteUIs = new List<PlayerVoteUI>(); // 초기화 추가
    public TextMeshProUGUI centerText;
    public GameObject confirmDialog;
    public TextMeshProUGUI confirmText;
    public GameObject resultDialog;
    public TextMeshProUGUI resultText;

    private string votedPlayerName = null;
    [Header("※ 진짜 범인을 여기서 고르세요!")]
    public string correctAnswer;


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

        if (votedPlayerName == correctAnswer)
        {
            // ✅ 정답일 경우에만 투표 수 증가
            var target = playerVoteUIs.FirstOrDefault(p => p.playerName == votedPlayerName);
            if (target != null) target.IncreaseVote();

            centerText.text = "투표가 완료되었습니다.";
            resultDialog.SetActive(true);
            resultText.text = "정답입니다! 범인을 잡았습니다!";
        }
        else
        {
            // ❌ 틀린 경우엔 IncreaseVote 안 함
            StartCoroutine(ShowRetryDialog());
        }
    }


    public void OnConfirmNo()
    {
        votedPlayerName = null;
        confirmDialog.SetActive(false);
        centerText.text = "다시 선택해주세요.";

        // 모든 버튼 다시 활성화
        foreach (var ui in playerVoteUIs)
        {
            ui.EnableButton();
        }
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
            // ✅ 이름에 맞는 스프라이트 찾기
            Sprite foundSprite = null;
            var info = playerInfos.FirstOrDefault(p => p.playerName == name);
            if (info != null)
                foundSprite = info.characterSprite;

            GameObject obj = Instantiate(playerVoteUIPrefab, playerVoteListParent);
            PlayerVoteUI ui = obj.GetComponent<PlayerVoteUI>();

            // ✅ 스프라이트도 함께 넘기기
            ui.Setup(name, this, foundSprite);
            playerVoteUIs.Add(ui);
        }

        centerText.text = "최종 투표를 진행해주세요.";

        Debug.Log($"[투표 시작] 정답은: {correctAnswer}");
    }




    public void OnClickStartButton()
    {
        Debug.Log("시작 버튼이 눌렸습니다.");

        startButton.SetActive(false);
        StartVoting();
    }
}
