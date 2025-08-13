using UnityEngine;
using UnityEngine.Events;

public class CharacterController : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;                          // 점프 힘
    [Range(0, 1)][SerializeField] private float m_CrouchSpeed = .36f;           // 앉기 움직임 최대 속도 1 = 100%
    [Range(0, .3f)][SerializeField] private float m_MovementSmoothing = .05f;   // 움직임 부드러움 정도
    [SerializeField] private bool m_AirControl = false;                         // 공중에서 방향 전환 가능 여부
    [SerializeField] private LayerMask m_WhatIsGround;                          // 무엇이 땅인지 판단
    [SerializeField] private Transform m_GroundCheck;                           // 땅에 닿았는지 판단하는 마킹
    [SerializeField] private Transform m_CeilingCheck;                          // 천장 마킹
    [SerializeField] private Collider2D m_CrouchDisableCollider;                // 앉기 상태일 때 적용되지 않는 콜라이더

    const float k_GroundedRadius = .2f; // 땅에 닿았는지 판단하는 원의 반지름
    private bool m_Grounded;            // 땅에 닿았는지 여부
    const float k_CeilingRadius = .2f; // 일어설 수 있는지 판단하는 원의 반지름
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // 바라보는 방향 true = 오른쪽
    private Vector3 m_Velocity = Vector3.zero;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    public PlayerAnimatorController Animatorcontroller;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();
    }


    private void FixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // 원에 ground라고 지정된 게 있으면 땅에 닿은 상태
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (!wasGrounded)
                {
                    OnLandEvent.Invoke();
                    Debug.Log("!wasGrounded");
                }

            }
        }
    }


    public void Move(float move, bool crouch, bool jump)
    {
        // 앉은 상태일 때 일어설 수 있는지 판단
        if (!crouch)
        {
            // 범위 안에 천장이 있을 경우 일어서지 못함
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }

        // 땅에 닿았거나 공중 방향 전환이 가능할 때만 플레이어 조작
        if (m_Grounded || m_AirControl)
        {

            // 앉을 때
            if (crouch)
            {
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }

                // 앉을 때 움직이는 속도 줄이기
                move *= m_CrouchSpeed;

                // 앉을 때 콜라이더 하나 해제
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }
            else
            {
                // 앉지 않을 때 콜라이더 하나 적용
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = true;

                if (m_wasCrouching)
                {
                    m_wasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                }
            }

            // 타깃 속도로 플레이어를 움직이기
            Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.linearVelocity.y);
            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.linearVelocity = Vector3.SmoothDamp(m_Rigidbody2D.linearVelocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
           
            // 만약 오른쪽으로 움직이는데 왼쪽으로 바라보고 있다면
            if (move > 0 && !m_FacingRight)
            {
                // 플레이어 뒤집기
                Flip();
            }
            // 만약 왼쪽으로 움직이는데 오른쪽으로 바라보고 있다면
            else if (move < 0 && m_FacingRight)
            {
                // 플레이어 뒤집기
                Flip();
            }
        }
        // 플레이어 점프 판단
        if (m_Grounded && jump)
        {
            // 플레이어에게 세로 방향 힘 적용
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
        }

        //플레이어 애니메이터
        if (move != 0)
        {
            Animatorcontroller.playerState = "Walk";
        }
        else
        {
           Animatorcontroller.playerState = "Idle";
        }
    }


    private void Flip()
    {
        // 플레이어가 바라보는 방향 대로 변수 값 변환
        m_FacingRight = !m_FacingRight;

        // 플레이어 x좌표 * -1
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}