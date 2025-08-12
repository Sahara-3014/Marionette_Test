using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CharacterActionPlayer : MonoBehaviour
{
    [Header("캐릭터 정보")]
    [Tooltip("이 캐릭터의 타입")]
    public CharacterType characterType;

    [Header("등장 시 설정")]
    [Tooltip("캐릭터가 등장할 위치")]
    public Transform positionTarget;

    [Tooltip("캐릭터의 초기 상태(스프라이트)이름")]
    public string initialState = "기본";

    [Tooltip("캐릭터의 등장 시 크기 배율")]
    [Range(0.1f, 5f)]
    public float initialScale = 1.0f;

    [Tooltip("체크하면 실루엣으로 등장")]
    public bool appearAsSilhouette = false;

    [Tooltip("체크하면 좌우 반전")]
    public bool flipOnStart = false;

    private SpriteRenderer selfRenderer;

    void Awake()
    {
        selfRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (EffectCharacterManager.Instance == null)
        {
            Debug.LogError("EffectCharacterManager가 씬에 없음!!!!!");
            return;
        }

        EffectCharacterManager.Instance.RegisterCharacter(characterType, this.gameObject, selfRenderer);

        if (positionTarget != null)
        {
            transform.position = positionTarget.position;
        }
        transform.localScale = new Vector3(initialScale, initialScale, 1f);

        EffectCharacterManager.Instance.ChangeCharacterState(characterType, initialState);

        if (appearAsSilhouette)
        {
            EffectCharacterManager.Instance.SetSilhouette(characterType, true);
        }
        if (flipOnStart)
        {
            EffectCharacterManager.Instance.FlipCharacter(characterType, true);
        }
    }
}