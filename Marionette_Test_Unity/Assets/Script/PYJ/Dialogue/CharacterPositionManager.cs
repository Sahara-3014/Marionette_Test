using UnityEngine;
using System.Collections.Generic;

public class CharacterPositionManager : MonoBehaviour
{
    public Transform leftPosition;
    public Transform centerPosition;
    public Transform rightPosition;

    private Dictionary<Dialog_CharPos, SpriteRenderer> charSlots = new Dictionary<Dialog_CharPos, SpriteRenderer>();

    public void SetCharacter(SpriteRenderer character, Dialog_CharPos pos)
    {
        if (pos == Dialog_CharPos.None)
        {
            // 위치 변경 없음
            return;
        }

        if (pos == Dialog_CharPos.Clear)
        {
            // Clear 처리 (예: 모든 슬롯 제거)
            ClearAllSlots();
            return;
        }

        // 위치 대상 Transform 가져오기
        Transform targetPos = GetTransformByCharPos(pos);
        if (targetPos == null)
        {
            Debug.LogWarning("[CharPosManager] 지정된 위치의 Transform이 없습니다.");
            return;
        }

        character.transform.position = targetPos.position;
        character.gameObject.SetActive(true);

        // 해당 슬롯에 등록
        if (pos == Dialog_CharPos.Left || pos == Dialog_CharPos.Center || pos == Dialog_CharPos.Right)
        {
            charSlots[pos] = character;
        }
    }

    public void ClearSlot(Dialog_CharPos pos)
    {
        if (charSlots.TryGetValue(pos, out SpriteRenderer character))
        {
            if (character != null)
                character.gameObject.SetActive(false);
            charSlots.Remove(pos);
        }
    }

    public void ClearAllSlots()
    {
        foreach (var kvp in charSlots)
        {
            if (kvp.Value != null)
                kvp.Value.gameObject.SetActive(false);
        }
        charSlots.Clear();
    }

    public Transform GetTransformByCharPos(Dialog_CharPos pos)
    {
        switch (pos)
        {
            case Dialog_CharPos.Left:
                return leftPosition;
            case Dialog_CharPos.Center:
                return centerPosition;
            case Dialog_CharPos.Right:
                return rightPosition;
            default:
                return null;
        }
    }
    

}
