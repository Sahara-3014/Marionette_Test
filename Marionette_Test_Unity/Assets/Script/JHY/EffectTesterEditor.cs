using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CustomEditor(typeof(EffectTester))]
public class EffectTesterEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EffectTester tester = (EffectTester)target;

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("연출 세트 제어판", EditorStyles.boldLabel);

        string[] setNames;
        if (tester.testableDirectionSets != null && tester.testableDirectionSets.Count > 0)
        {
            setNames = tester.testableDirectionSets.Select(so => so != null ? so.name : " (비어있음)").ToArray();
        }
        else
        {
            setNames = new string[] { " (테스트할 목록이 비어있음)" };
        }

        tester.selectedSetIndex = EditorGUILayout.Popup("테스트할 연출 세트", tester.selectedSetIndex, setNames);

        EditorGUILayout.Space(10);

        GUI.backgroundColor = new Color(0.8f, 1f, 0.8f);
        if (GUILayout.Button("▶ Play Selected Set", GUILayout.Height(40)))
        {
            DirectionSetSO setToTest = GetSelectedSet(tester);
            if (IsReadyToTest(setToTest))
            {
                EffectManager.Instance.PlayDirectionSet(setToTest);
                Debug.Log($"[EffectTester] '{setToTest.name}' 연출 세트를 재생합니다.");
            }
        }

        EditorGUILayout.Space(5);

        GUI.backgroundColor = new Color(1f, 0.8f, 0.8f);
        if (GUILayout.Button("■ Stop Selected Set", GUILayout.Height(40)))
        {
            DirectionSetSO setToTest = GetSelectedSet(tester);
            if (IsReadyToTest(setToTest))
            {
                EffectManager.Instance.StopDirectionSet(setToTest, tester.stopParticlesImmediately);
                Debug.Log($"[EffectTester] '{setToTest.name}' 연출 세트를 중지합니다.");
            }
        }
        GUI.backgroundColor = Color.white;
    }

    // 선택된 인덱스에 해당하는 DirectionSetSO를 가져오는 함수
    private DirectionSetSO GetSelectedSet(EffectTester tester)
    {
        if (tester.testableDirectionSets != null &&
            tester.selectedSetIndex >= 0 &&
            tester.selectedSetIndex < tester.testableDirectionSets.Count)
        {
            return tester.testableDirectionSets[tester.selectedSetIndex];
        }
        return null;
    }

    // 테스트 준비 상태를 확인하는함수
    private bool IsReadyToTest(DirectionSetSO set)
    {
        if (!Application.isPlaying || EffectManager.Instance == null)
        {
            EditorGUILayout.HelpBox("테스트를 하려면 게임을 실행(Play)해야 합니다.", MessageType.Warning);
            return false;
        }

        if (set == null)
        {
            EditorGUILayout.HelpBox("드롭다운 메뉴에서 테스트할 유효한 연출 세트를 선택해주세요.", MessageType.Info);
            return false;
        }
        return true; 
    }
}