using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerVoteUI : MonoBehaviour
{
    public string playerName;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI voteCountText;
    public Image background;
    public Image characterImage; // 캐릭터 사진

    private int voteCount = 0;
    private VoteManager voteManager;
    private Button backgroundButton;






    /// <summary>
    /// 플레이어 투표 UI 세팅
    /// </summary>
    public void Setup(string name, VoteManager manager, Sprite characterSprite = null, bool isSelf = false)
    {
        Debug.Log($"[PlayerVoteUI] 이름 할당됨: {name}");

        playerName = name;
        voteManager = manager;
        voteCount = 0;

        // UI 초기화
        nameText.text = name;
        voteCountText.text = voteCount.ToString();

        // 자기 자신 표시
        if (isSelf)
            background.color = Color.yellow;

        // 캐릭터 스프라이트 설정
        if (characterSprite != null)
            characterImage.sprite = characterSprite;

        // 버튼 컴포넌트 확보
        backgroundButton = background.GetComponent<Button>();
        if (backgroundButton == null)
            backgroundButton = background.gameObject.AddComponent<Button>();

        // 클릭 이벤트 등록
        backgroundButton.onClick.RemoveAllListeners();
        backgroundButton.onClick.AddListener(() =>
        {
            Debug.Log($"[Vote Button Clicked] playerName = {playerName}");
            voteManager.OnVoteButtonClicked(playerName);
        });

    }




    /// <summary>
    /// 투표 가능 상태로 변경
    /// </summary>
    public void EnableButton()
    {
        if (backgroundButton != null)
            backgroundButton.interactable = true;
    }





    /// <summary>
    /// 투표 수 증가 및 UI 반영
    /// </summary>
    public void IncreaseVote()
    {
        voteCount++;
        voteCountText.text = voteCount.ToString();

        // 특정 조건에서 색상 변경 (예: 10표 이상이면 빨강)
        if (voteCount >= 10)
            background.color = Color.red;

        // 한 번만 투표 가능하게 만들고 싶으면 여기서 비활성화
        if (backgroundButton != null)
            backgroundButton.interactable = false;
    }



    public void SetVoteCount(int count)
    {
        voteCount = count;
        voteCountText.text = voteCount.ToString();

        // 색상 변경 조건 그대로 유지
        if (voteCount >= 10)
            background.color = Color.red;
        else
            background.color = Color.white;
    }



}
