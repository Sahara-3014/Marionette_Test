using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class DebateVotingSystem : MonoBehaviour
{
    public static DebateVotingSystem Instance;
    public RectTransform[] charVotingGauge;
    public List<int> tempVote = new()
    {
        14, 0, 14, 11, 14, 0, 0, 11, 14, 14, 14, 12, 14
    };

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

    async public void AddVoteToCharacters(List<int> voteIndexs, UnityAction callback = null)
    {

        List<int> charVotes = new();
        for (int i = 0; i < charVotingGauge.Length; i++)
        {
            charVotingGauge[i].sizeDelta = new Vector2(0f, charVotingGauge[i].sizeDelta.y);
            charVotes.Add(0);
        }

        for (int i = 0; i < voteIndexs.Count; i++)
        {
            charVotes[voteIndexs[i]] += 1;
            for (int j = 0; j < charVotingGauge.Length; j++)
            {
                //Debug.Log($"{j}번째 {charVotes[i]}번 캐릭터 투표: {charVotes[j]} / {voteIndexs.Count}");
                if ((j == voteIndexs[i] && IsSelectedSlot(j) == false) ||
                    (j != voteIndexs[i] && IsSelectedSlot(j) == true))
                    ToggleSelectPanel(j);
                ChangeVoteGauge(j, charVotes[j], charVotingGauge.Length);
            }
            await Task.Delay(TimeSpan.FromSeconds(1f));
        }

        callback?.Invoke();
    }

    public void ChangeVoteGauge(int charIndex, int nowCharVote, int max, UnityAction callback = null)
    {
        float _maxGauge = charVotingGauge[charIndex].parent.GetComponent<RectTransform>().rect.width;
        _maxGauge -= (charVotingGauge[charIndex].anchoredPosition.x * 2f);

        charVotingGauge[charIndex].DOSizeDelta(new Vector2(((float)nowCharVote / max) * _maxGauge,
            charVotingGauge[charIndex].sizeDelta.y), 0.5f);

        callback?.Invoke();
    }

    public void ToggleSelectPanel(int index)
    {
        // 예외처리
        if (index < 0 || index >= charVotingGauge.Length)
            return;
        // 죽은사람 투표X
        if (IsDeadedSlot(index))
            return;

        GameObject selectObj = charVotingGauge[index].transform.parent.parent.GetChild(0).GetChild(1).gameObject;
        selectObj.SetActive(!selectObj.activeSelf);

    }

    public void ToggleDeadPanel(int index)
    {
        GameObject deadObj = charVotingGauge[index].transform.parent.parent.GetChild(0).GetChild(0).gameObject;
        deadObj.SetActive(!deadObj.activeSelf);
    }

    public bool IsDeadedSlot(int index)
    {
        if (index < 0 || index >= charVotingGauge.Length)
            return false;
        GameObject selectObj = charVotingGauge[index].transform.parent.parent.GetChild(0).GetChild(0).gameObject;
        return selectObj.activeSelf;
    }

    public int IsSelectedCharacter()
    {
        for (int i = 0; i < charVotingGauge.Length; i++)
        {
            GameObject selectObj = charVotingGauge[i].transform.parent.parent.GetChild(0).GetChild(1).gameObject;

            if (selectObj.activeSelf)
                return i;
        }

        return -1; // 선택된 캐릭터가 없음
    }

    public bool IsSelectedSlot(int index)
    {
        if (index < 0 || index >= charVotingGauge.Length)
            return false;
        GameObject selectObj = charVotingGauge[index].transform.parent.parent.GetChild(0).GetChild(1).gameObject;
        return selectObj.activeSelf;
    }



    public void TempVoteAction()
    {
        AddVoteToCharacters(tempVote, () =>
        {
            int index = tempVote.Max();
            ToggleDeadPanel(index);
        });
    }
}
