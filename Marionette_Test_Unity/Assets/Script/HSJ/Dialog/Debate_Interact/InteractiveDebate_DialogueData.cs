using Newtonsoft.Json;
using SimpleJSON;
using System;
using UnityEngine;

public class InteractiveDebate_DialogueData
{
    public enum DialogSoundPlayType { PlayOnShot = 1, PlayLoop = 0, Stop = -1, Pause = -2, Continue = -3, FadeIn = 11, FadeOut = 12, FadeOutToIn = 13 }
    public DialogSoundPlayType soundPlayType = DialogSoundPlayType.PlayOnShot;

    private string[] row;
    private JSONNode node;

    #region General
    /// <summary> 그룹 </summary>
    public int ID { get; protected set; }
    /// <summary> 대화 순서 </summary>
    public int INDEX { get; protected set; }
    /// <summary> 다음대화 ID </summary>
    public int NEXT_ID { get; protected set; }
    public string NEXT_SCENE { get; protected set; }

    /// <summary> 화자 이름 </summary>
    public string SPEAKER { get; protected set; }
    /// <summary> 대화 내용 </summary>
    public string DIALOGUE { get; protected set; }
    #endregion

    public int DEBATE_TYPE { get; protected set; } = 0;
    public int DIALOG_SELECT_ID { get; protected set; } = 0;

    #region First Action
    /// <summary> BGM </summary>
    public SoundAsset BGM { get; protected set; }
    /// <summary> BGM 효과 </summary>
    public int BGM_EFFECT { get; protected set; } = 0;
    #endregion

    #region Second Action
    /// <summary> 화면 효과 </summary>
    public int BGEffect { get; protected set; } = 0;
    /// <summary> 배경 키 값 </summary>
    public string BG { get; protected set; }
    /// <summary> 삽입 이미지 (CG 등) </summary>
    public Sprite CG { get; protected set; }

    /// <summary> 화자 대화 시작시 효과음 </summary>
    public SoundAsset SE1 { get; protected set; }
    /// <summary>  </summary>
    public int SE1_EFFECT { get; protected set; } = 1;
    //public float SE1_Delay { get; protected set; } = default; // SE1 재생 딜레이
    #endregion

    #region Character's Values
    public string TARGET_NAME { get; protected set; }
    public string TARGET_BODY { get; protected set; }
    public string TARGET_HEAD { get; protected set; }
    public int TARGET_EFFECT { get; protected set; } = 1;
    public string TARGET_INTERACT { get; protected set; }

    public string CH1_NAME { get; protected set; } = null; // 캐릭터1 이름
    public string CH1_BODY { get; protected set; } = null; // 캐릭터1 감정
    public string CH1_HEAD { get; protected set; } = null; // 캐릭터1 감정
    public int CH1_EFFECT { get; protected set; } = 0; // 캐릭터1 효과

    #endregion

    #region Third Action
    // 캐릭터1

    // 캐릭터2
    public string CH2_NAME { get; protected set; } = null; // 캐릭터2 이름
    public string CH2_EMOTION { get; protected set; } = null; // 캐릭터2 감정
    public int CH2_EFFECT { get; protected set; } = 0; // 캐릭터2 효과

    // 캐릭터3
    //public string CH3_NAME { get; protected set; } = null; // 캐릭터3 이름
    //public string CH3_EMOTION { get; protected set; } = null; // 캐릭터3 감정
    //public Dialog_CharEffect CH3_EFFECT { get; protected set; } = Dialog_CharEffect.None; // 캐릭터3 효과

    // 효과음
    public SoundAsset SE2 { get; protected set; }
    public int SE2_EFFECT { get; protected set; } = 1;
    //public float SE2_Delay { get; protected set; } = default; // SE1 재생 딜레이
    #endregion

    #region Choice Values
    public int CHOICE1_ID { get; protected set; } = 0; // 선택지 1 ID
    public string CHOICE1_TEXT { get; protected set; } = null; // 선택지 1 텍스트

    public int CHOICE2_ID { get; protected set; } = 0; // 선택지 1 ID
    public string CHOICE2_TEXT { get; protected set; } = null; // 선택지 2 텍스트

    public int CHOICE3_ID { get; protected set; } = 0; // 선택지 3 ID
    public string CHOICE3_TEXT { get; protected set; } = null; // 선택지 3 텍스트

    #endregion

    public int EVIDENCE_ID { get; protected set; } = 0;
    public int EVIDENCE_NEXT_ID { get; protected set; } = 0;

    public InteractiveDebate_DialogueData(JSONNode nod)
    {
        this.node = nod;
        SetProperty();
    }

    public InteractiveDebate_DialogueData(string[] row)
    {
        //for(int i=0;i<row.Length; i++) 
        //    Debug.Log($"{i} : {row[i]}");

        this.row = row;
        SetProperty();
    }

    void SetProperty()
    {
        int _index = 0;
        try
        {
            this.ID = (GetNum(_index)); _index += 1;
            this.INDEX = (GetNum(_index)); _index += 1;
            this.NEXT_SCENE = GetText(_index);
            this.NEXT_ID = (GetNum(_index)); _index += 1;

            this.DEBATE_TYPE = (GetNum(_index)); _index += 1;


            this.TARGET_NAME = GetText(_index); _index += 1;
            this.TARGET_BODY = GetText(_index); _index += 1;
            this.TARGET_HEAD = GetText(_index); _index += 1;
            this.TARGET_INTERACT = GetText(_index); _index += 1;

            this.TARGET_EFFECT = (GetNum(_index, 1)); _index += 1;


            this.SPEAKER = GetText(_index); _index += 1;
            this.DIALOGUE = GetText(_index); _index += 1;


            this.DIALOG_SELECT_ID = GetNum(_index);


            this.BGM = LoadAudioAssetByName(GetText(_index)); _index += 1;
            this.BGM_EFFECT = (GetNum(_index)); _index += 1;

            this.BGEffect = (GetNum(_index)); _index += 1;


            this.CG = Resources.Load<Sprite>($"Cutscenes/{GetText(_index)}"); _index += 1;
            this.BG = GetText(_index); _index += 1;
            this.SE1 = LoadAudioAssetByName(GetText(_index)); _index += 1;
            this.SE1_EFFECT = (GetNum(_index)); _index += 1;


            this.CH1_NAME = GetText(_index); _index += 1;
            this.CH1_BODY = GetText(_index); _index += 1;
            this.CH1_HEAD = GetText(_index); _index += 1;
            this.CH1_EFFECT = (GetNum(_index)); _index += 1;


            this.SE2 = LoadAudioAssetByName(GetText(_index)); _index += 1;
            this.SE2_EFFECT = (GetNum(_index)); _index += 1;


            this.EVIDENCE_ID = (GetNum(_index)); _index += 1;
            this.EVIDENCE_NEXT_ID = (GetNum(_index)); _index += 1;
        }
        catch (Exception e)
        {
            Debug.Log($"[SetProperty Error] Row Data: {this.ID}:{this.INDEX} → {_index} = data : {GetText(_index)}\n{e.Message}");
        }

    }

    protected string GetText(int index)
    {
        string result = node == null ? (row.Length > index && row[index] != null) ? row[index].Trim() : ""
        : (node.Count > index && node[index] != null) ? node[index].Value.Trim() : "";
        //index += 1;
        return result;
    }

    protected int GetNum(int index, int _default = 0)
    {
        if(int.TryParse(GetText(index), out int value))
        {
            //index += 1;
            return value;
        }
        else
        {
            //index += 1;
            return _default;
        }
    }

    protected SoundAsset LoadAudioAssetByName(string clipName) =>
         Resources.Load<SoundAsset>($"Audio/SoundAsset/{clipName}");

    public override string ToString()
    {
        //return base.ToString();
        return JsonConvert.SerializeObject(this);
    }
}
