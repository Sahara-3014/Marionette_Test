using UnityEngine;
using System.Collections.Generic;

public class CharacterPositionManager : MonoBehaviour
{
    public Transform leftPosition;
    public Transform centerPosition;
    public Transform rightPosition;

    private Dictionary<Dialog_CharPos, SpriteRenderer> charSlots = new Dictionary<Dialog_CharPos, SpriteRenderer>();

    public void SetCharacter(SpriteRenderer character, Dialog_CharPos pos, Vector3 offset)
    {
        if (pos == Dialog_CharPos.None)
            return;
        if (pos == Dialog_CharPos.Clear)
        {
            ClearAllSlots();
            return;
        }

        Transform targetPos = GetTransformByCharPos(pos);
        if (targetPos == null)
        {
            Debug.LogWarning("[CharPosManager] 지정된 위치의 Transform이 없습니다.");
            return;
        }

        character.transform.position = targetPos.position + offset; // 여기 수정됨
        character.gameObject.SetActive(true);

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
    public Vector3 GetPositionByCharPos(Dialog_CharPos pos)
    {
        var t = GetTransformByCharPos(pos);
        return t != null ? t.position : Vector3.zero;
    }


}
