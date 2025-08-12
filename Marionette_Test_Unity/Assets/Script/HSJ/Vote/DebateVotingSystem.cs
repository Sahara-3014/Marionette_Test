using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DebateVotingSystem : MonoBehaviour
{
    public static DebateVotingSystem Instance;
    public RectTransform[] charVotingGauge;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public IEnumerator AddVoteToCharacters(List<int> voteIndexs, UnityAction callback = null)
    {
        WaitForSecondsRealtime wait = new WaitForSecondsRealtime(1f);
        int nowMaxVote = 0;
        Dictionary<int, int> charVotes = new();
        for(int i=0;i<charVotingGauge.Length;i++)
            charVotes.Add(i, 0);

        for (int i=0;i<voteIndexs.Count;i++)
        {
            charVotes[voteIndexs[i]]+=1;
            nowMaxVote += 1;
            for (int j=0;j<charVotingGauge.Length;j++)
               AddVoteToCharacter(j, charVotes[voteIndexs[i]], nowMaxVote);
            yield return wait;
        }

        callback?.Invoke();
    }

    void AddVoteToCharacter(int charIndex, int nowCharVote, int max, UnityAction callback = null)
    {
        for (int i=0;i<charVotingGauge.Length;i++)
        {
            float _maxGauge = charVotingGauge[charIndex].parent.GetComponent<RectTransform>().rect.width;
            _maxGauge -= charVotingGauge[charIndex].anchoredPosition.x * 2f;

            charVotingGauge[i].DOSizeDelta(new Vector2(((float)nowCharVote / max) / _maxGauge,
                charVotingGauge[i].sizeDelta.y), 0.5f);
        }

        callback?.Invoke();
    }

    public void CharSelect(int charIndex)
    {
        // 예외처리
        if (charIndex <= 0 || charIndex >= charVotingGauge.Length)
            return;
        // 죽은사람 투표X
        if (charVotingGauge[charIndex].transform.parent.parent.GetChild(0).GetChild(0).gameObject.activeSelf)
            return;

        GameObject selectObj = charVotingGauge[charIndex].transform.parent.parent.GetChild(0).GetChild(1).gameObject;
        selectObj.SetActive(!selectObj.activeSelf);

    }

    public int IsSelectedCharacter()
    {
        for(int i=0;i<charVotingGauge.Length;i++)
        {
            GameObject selectObj = charVotingGauge[i].transform.parent.parent.GetChild(0).GetChild(1).gameObject;

            if (selectObj.activeSelf)
                return i;
        }

        return -1; // 선택된 캐릭터가 없음
    }
}
