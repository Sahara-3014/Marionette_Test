using UnityEngine;
using System.Collections.Generic;
public enum CharacterType
{
    None,
    Player_SD,
    Character_SD
}

[System.Serializable]
public struct CharacterState
{
    public string stateName;
    public Sprite sprite;
}

[System.Serializable]
public struct CharacterData
{
    public CharacterType characterType;
    public List<CharacterState> states;
}


public class EffectCharacterManager : MonoBehaviour
{
    public static EffectCharacterManager Instance { get; private set; }

    [Header("캐릭터 데이터베이스")]
    [Tooltip("모든 캐릭터의 타입과 상태를 등록")]
    public List<CharacterData> characterDatabase;

    [Header("캐릭터 부모 오브젝트")]
    [Tooltip("생성된 캐릭터 오브젝트들이 위치할 부모 Transform")]
    public Transform characterParent;

    // 현재 씬에 활성화된 캐릭터들을 관리
    private Dictionary<CharacterType, SpriteRenderer> activeCharacters = new Dictionary<CharacterType, SpriteRenderer>();
    private Dictionary<CharacterType, List<CharacterState>> characterDataDict = new Dictionary<CharacterType, List<CharacterState>>();

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        characterDataDict.Clear(); // Awake가 여러 번 호출될 경우를 대비해 초기화
        foreach (var data in characterDatabase)
        {
            if (!characterDataDict.ContainsKey(data.characterType))
                characterDataDict[data.characterType] = data.states;
            else
                Debug.LogWarning($"중복된 캐릭터 타입 감지: {data.characterType}");
        }
    }

    #region 공개 함수

    public void RegisterCharacter(CharacterType type, GameObject characterObject, SpriteRenderer renderer)
    {
        if (type == CharacterType.None) return;

        if (activeCharacters.ContainsKey(type))
        {
            HideCharacter(type);
        }

        activeCharacters[type] = renderer;
    }

    public void HideCharacter(CharacterType type)
    {
        if (activeCharacters.TryGetValue(type, out SpriteRenderer renderer))
        {
            if (renderer != null) Destroy(renderer.gameObject);
            activeCharacters.Remove(type);
        }
    }

    public void ChangeCharacterState(CharacterType type, string newStateName)
    {
        if (activeCharacters.TryGetValue(type, out SpriteRenderer renderer) &&
            characterDataDict.TryGetValue(type, out List<CharacterState> states))
        {
            var state = states.Find(s => s.stateName == newStateName);
            if (state.sprite != null)
            {
                renderer.sprite = state.sprite;
            }
            else
            {
                Debug.LogWarning($"캐릭터 '{type}'의 '{newStateName}' 상태를 찾을 수 없습니다.");
            }
        }
    }

    public void SetSilhouette(CharacterType type, bool isSilhouette)
    {
        if (activeCharacters.TryGetValue(type, out SpriteRenderer renderer))
        {
            if (isSilhouette)
            {
                renderer.color = Color.black;
            }
            else
            {
                renderer.color = Color.white;
            }
        }
    }

    public void FlipCharacter(CharacterType type, bool isFlipped)
    {
        if (activeCharacters.TryGetValue(type, out SpriteRenderer renderer))
        {
            renderer.flipX = isFlipped;
        }
    }
    #endregion
}