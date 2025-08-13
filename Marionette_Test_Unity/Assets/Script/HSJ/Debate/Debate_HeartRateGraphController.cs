using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Debate_HeartRateGraphController : MonoBehaviour
{
    private LineRenderer lineRenderer;

    [Header("Graph Settings")]
    [Range(30, 500)]
    public int pointCount = 70; // 그래프를 구성할 점의 개수 (해상도) // 심박 다음점 위치

    [Header("Heartbeat Simulation")]
    public float heartRateBPM = 160f; // 분당 심박수 (BPM) = 흘러가는 속도
    public float beatSpikeHeight = 1.5f; // 심박 스파이크 높이 //  1.5 긴장상태 2 불안 5 거짓
    public float beatSpikeWidth = 0.001f;  // 스파이크의 날카로운 정도 
    public float noiseAmount = 0.05f; // 기본 노이즈 크기 // 기본심박 노이즈
    [SerializeField] Rect rect; // 그래프의 rect

    private List<float> dataPoints;
    private float timeSinceLastBeat = 0f;
    private float beatInterval;

    public void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        dataPoints = new List<float>();

        lineRenderer.positionCount = 0;

        // 초기 데이터 (평평한 선) 생성
        for (int i = 0; i < pointCount; i++)
        {
            dataPoints.Add(0f);
        }

        // BPM을 초당 간격으로 변환
        beatInterval = 60f / heartRateBPM;
    }

    void Update()
    {
        // Line Renderer 초기화
        lineRenderer.positionCount = dataPoints.Count;

        // 심박 간격 타이머 업데이트
        timeSinceLastBeat += Time.deltaTime;

        // 새로운 데이터 포인트 생성
        float newDataPoint = GenerateDataPoint();
        dataPoints.Add(newDataPoint);

        // 가장 오래된 데이터 제거 및 새 데이터 추가
        if (dataPoints.Count > pointCount)
            dataPoints.RemoveAt(0);

        // BPM이 변경될 수 있으므로 매 프레임 간격 업데이트
        beatInterval = 60f / heartRateBPM;

        // Line Renderer 업데이트
        DrawGraph();
    }

    ///// <summary> OnValidate는 Unity 에디터에서 스크립트가 변경될 때마다 호출됩니다. </summary>
    //private void OnValidate()
    //{
    //    Vector3 top_start = new Vector3(transform.position.x + rect.x - (rect.width / 2f), transform.position.y + rect.y - (rect.height / 2f), transform.position.z);
    //    Vector3 top_end = new Vector3(transform.position.x + (rect.width / 2f), transform.position.y - (rect.height / 2f), transform.position.z);
    //    Debug.DrawLine(top_start, top_end, Color.red);

    //    //Vector3 bottom_start = new Vector3(transform.position.x + rect.x - (rect.width / 2f), transform.position.y +rect.y + (rect.height / 2f), transform.position.z);
    //    //Vector3 bottom_end = new Vector3(transform.position.x +rect.x + (rect.width / 2f), transform.position.y + rect.y + (rect.height / 2f), transform.position.z);
    //    //Debug.DrawLine(bottom_start, bottom_start, Color.red);

    //    //Vector3 left_start = new Vector3(transform.position.x + rect.x - (rect.width / 2f), transform.position.y + rect.y - (rect.height / 2f), transform.position.z);
    //    //Vector3 left_end = new Vector3(transform.position.x - (rect.width / 2f), transform.position.y + (rect.height / 2f), transform.position.z);
    //    //Debug.DrawLine(left_start, left_end, Color.red);

    //    //Vector3 right_start = new Vector3(transform.position.x + rect.x + (rect.width / 2f), transform.position.y + rect.y - (rect.height / 2f), transform.position.z);
    //    //Vector3 right_end = new Vector3(transform.position.x + rect.x + (rect.width / 2f), transform.position.y + rect.y + (rect.height / 2f), transform.position.z);
    //    //Debug.DrawLine(right_start, right_end, Color.red);
    //}

    private float GenerateDataPoint()
    {
        // 기본 노이즈 생성
        float noise = (Random.value - 0.5f) * 2f * noiseAmount;

        // 심박 간격이 되면 스파이크 생성
        if (timeSinceLastBeat >= beatInterval)
        {
            timeSinceLastBeat -= beatInterval;
        }

        // 스파이크 모양 생성 (PQRST파형 단순화)
        float spike = 0;
        float timeFromBeat = timeSinceLastBeat / beatInterval; // 0~1 사이 값

        if (timeFromBeat < beatSpikeWidth) // R파 (가장 높은 스파이크)
        {
            spike = Mathf.Sin((timeFromBeat / beatSpikeWidth) * Mathf.PI) * beatSpikeHeight;
        }
        else if (timeFromBeat > 0.4f && timeFromBeat < 0.5f) // T파 (작은 봉우리)
        {
            spike = Mathf.Sin(((timeFromBeat - 0.4f) / 0.1f) * Mathf.PI) * beatSpikeHeight * 0.2f;
        }


        return spike + noise;
    }

    private void DrawGraph()
    {
        RectTransform rt = GetComponent<RectTransform>();
        float graphWidth = rt != null ? rt.rect.width : rect.width;
        float graphHeight = rt != null? rt.rect.height : rect.height;
        float yOffset = -graphHeight / 2f; // 그래프를 중앙에 맞추기 위한 오프셋

        Vector3[] positions = new Vector3[dataPoints.Count];
        for (int i = 0; i < dataPoints.Count; i++)
        {
            float x = ((float)i / (dataPoints.Count - 1)) * graphWidth;
            float y = (dataPoints[i] * (graphHeight / 2f)) + yOffset; // y값을 그래프 높이에 맞게 스케일링

            positions[i] = new Vector3(x - graphWidth / 2f, y + graphHeight/2f, 0);
            //Debug.Log(positions[i].ToString());
        }

        
        lineRenderer.SetPositions(positions);
    }
}