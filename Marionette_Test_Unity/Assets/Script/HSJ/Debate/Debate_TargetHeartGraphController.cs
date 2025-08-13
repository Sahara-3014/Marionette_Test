using System.Collections.Generic;
using UnityEngine;

public class Debate_TargetHeartGraphController : Debate_HeartRateGraphController
{
    public struct HeartData
    {
        public float heartRateBPM; // 분당 심박수
        public float beatSpikeHeight; // 심박 스파이크 높이
        public float beatSpikeWidth; // 스파이크의 날카로운 정도
        public float noiseAmount; // 노이즈 크기

        public HeartData(float heartRateBPM, float beatSpikeHeight, float beatSpikeWidth, float noiseAmount)
        {
            this.heartRateBPM = heartRateBPM;
            this.beatSpikeHeight = beatSpikeHeight;
            this.beatSpikeWidth = beatSpikeWidth;
            this.noiseAmount = noiseAmount;
        }
    }
    public enum TargetHeartType
    {
        /// <summary> 침착 상태 </summary>
        Normal,
        /// <summary> 일반 심박 </summary>
        Tension,
        /// <summary> 긴장 상태 </summary>
        Anxiety,
        /// <summary> 불안 상태 </summary>
        /// <summary> 거짓말 상태 </summary>
        Lie
    }

    [Header("Target Heart Graph Settings")]
    public TargetHeartType nowHeartType = TargetHeartType.Normal;

    [SerializeField]
    Dictionary<TargetHeartType, HeartData> targetHeartData = new Dictionary<TargetHeartType, HeartData>
    {
        { TargetHeartType.Normal, new HeartData(160f, 0, 0.001f, 0.08f) },
        { TargetHeartType.Tension, new HeartData(165f, 1.5f, 0.002f, 0.1f) },
        { TargetHeartType.Anxiety, new HeartData(180f, 2f, 0.003f, 0.14f) },
        { TargetHeartType.Lie, new HeartData(200f, 5f, 0.005f, 0.2f) }
    };

    public void Init()
    {
        base.Awake();
    }

    public void ChangeGraph()
    {
        // 현재 심박 타입에 따라 그래프를 변경
        ChangeHeartGraph(nowHeartType);
    }

    public void ChangeHeartGraph(TargetHeartType heartType)
    {
        nowHeartType = heartType;
        base.heartRateBPM = targetHeartData[heartType].heartRateBPM;
        base.beatSpikeHeight = targetHeartData[heartType].beatSpikeHeight;
        base.beatSpikeWidth = targetHeartData[heartType].beatSpikeWidth;
        base.noiseAmount = targetHeartData[heartType].noiseAmount;
    }

}
