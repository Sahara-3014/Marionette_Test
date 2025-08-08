using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVoteUI : MonoBehaviour
{
    public string playerName;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI voteCountText;
    public Image background;

    // 추가: 캐릭터 사진용 Image
    public Image characterImage;

    private int voteCount = 0;
    private VoteManager voteManager;

    // 캐릭터 스프라이트도 같이 받도록 수정
    public void Setup(string name, VoteManager manager, Sprite characterSprite = null, bool isSelf = false)
    {
        Debug.Log($"[PlayerVoteUI] 이름 할당됨: {name}");

        playerName = name;
        voteManager = manager;
        voteCount = 0;

        nameText.text = name;
        voteCountText.text = "0";

        if (isSelf)
            background.color = Color.yellow;

        if (characterSprite != null)
            characterImage.sprite = characterSprite;

        Button backgroundButton = background.GetComponent<Button>();
        if (backgroundButton == null)
        {
            backgroundButton = background.gameObject.AddComponent<Button>();
        }

        backgroundButton.onClick.RemoveAllListeners();
        backgroundButton.onClick.AddListener(() =>
        {
            Debug.Log($"[Button Clicked] playerName = {playerName}");
            voteManager.OnVoteButtonClicked(playerName);
        });
    }
    public void EnableButton()
    {
        var button = background.GetComponent<Button>();
        if (button != null)
            button.interactable = true;
    }


    public void IncreaseVote()
    {
        voteCount++;
        voteCountText.text = voteCount.ToString();

        if (voteCount >= 10)
            background.color = Color.red;

        var button = background.GetComponent<Button>();
        if (button != null)
            button.interactable = false;
    }
}
