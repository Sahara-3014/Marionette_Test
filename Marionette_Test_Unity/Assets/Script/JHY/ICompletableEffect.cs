using System; 

public interface ICompletableEffect
{
    // 이 이벤트에 이펙트 종료를 알리기
    event Action OnComplete;

    // 실제 이펙트 지속 시간
    float Duration { get; }
}