using System.ComponentModel;
using UnityEngine;
public class EffectInfo : MonoBehaviour
{
    [Header("이펙트 정보")]
    [Tooltip("이 이펙트의 소멸 시간")]
    public float MaxDuration = 3.0f;
    [SerializeField] private bool isSelfDestroy = false;
    void Start()
    {

        if (MaxDuration > 0 && isSelfDestroy)
        {
            Destroy(gameObject, MaxDuration);
        }
    }
}