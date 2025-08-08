using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVoteUI : MonoBehaviour
{
    public string playerName;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI voteCountText;
    public Image background;

    private int voteCount = 0;
    private VoteManager voteManager;

    public void Setup(string name, VoteManager manager, bool isSelf = false)
    {
        playerName = name;
        voteManager = manager;
        voteCount = 0;

        nameText.text = name;
        voteCountText.text = "0";

        if (isSelf)
            background.color = Color.yellow;

        // 👉 배경 클릭 이벤트 연결 (Button 컴포넌트 이용)
        Button backgroundButton = background.GetComponent<Button>();
        if (backgroundButton == null)
        {
            backgroundButton = background.gameObject.AddComponent<Button>();
        }

        backgroundButton.onClick.RemoveAllListeners();
        backgroundButton.onClick.AddListener(() =>
        {
            voteManager.OnVoteButtonClicked(playerName);
        });
    }

    public void IncreaseVote()
    {
        voteCount++;
        voteCountText.text = voteCount.ToString();

        if (voteCount >= 10)
            background.color = Color.red;

        // 배경 클릭 비활성화 (중복투표 방지)
        var button = background.GetComponent<Button>();
        if (button != null)
            button.interactable = false;
    }
}
