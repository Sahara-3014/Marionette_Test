using Newtonsoft.Json;
using SimpleJSON;
using System;
using UnityEngine;

public class InteractiveDebate_DialogueData
{
    enum SheetIndex
    {
        ID = 0,
        INDEX = 1,
        NEXT_ID = 2,

        TARGET_NAME = 3,
        TARGET_EMOTION = 4,
        TARGET_EFFECT = 5,

        SPEAKER = 6,
        DIALOGUE = 7,

        BGM = 8,
        BGM_EFFECT = 9,

        SCREEN_EFFECT = 10,
        CG = 11,
        BG = 12,
        

        SE1 = 13,
        SE1_EFFECT = 14,
        SE1_DELAY = 15,
        
        CH1_NAME = 16,
        CH1_EMOTION = 17,
        CH1_EFFECT = 18,

        CH2_NAME = 19,
        CH2_EMOTION = 20,
        CH2_EFFECT = 21,

        CH3_NAME = 22,
        CH3_EMOTION = 23,
        CH3_EFFECT = 24,

        SE2 = 25,
        SE2_EFFECT = 26,
        SE2_DELAY = 27,

        CHOICE1_ID = 28,
        CHOICE1_TEXT = 29,
        CHOICE2_ID = 30,
        CHOICE2_TEXT = 31,
        CHOICE3_ID = 32,
        CHOICE3_TEXT = 33
    }
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
    #endregion

    

    #region Default Values
    /// <summary> 화자 이름 </summary>
    public string SPEAKER { get; protected set; }
    /// <summary> 대화 내용 </summary>
    public string DIALOGUE { get; protected set; }
    #endregion

    #region First Action
    /// <summary> BGM </summary>
    public SoundAsset BGM { get; protected set; }
    /// <summary> BGM 효과 </summary>
    public DialogSoundPlayType BGM_EFFECT { get; protected set; }
    #endregion

    #region Second Action
    /// <summary> 화면 효과 </summary>
    public Dialog_ScreenEffect screenEffect { get; protected set; } = Dialog_ScreenEffect.None;
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

    #region Target's Values
    public string TARGET_NAME { get; protected set; }
    public string TARGET_EMOTION { get; protected set; }
    public Dialog_CharEffect TARGET_EFFECT { get; protected set; }
    public string TARGET_INTERACT { get; protected set; }
    #endregion

    #region Third Action
    // 캐릭터1
    public string CH1_NAME { get; protected set; } = null; // 캐릭터1 이름
    public string CH1_EMOTION { get; protected set; } = null; // 캐릭터1 감정
    public Dialog_CharEffect CH1_EFFECT { get; protected set; } = Dialog_CharEffect.None; // 캐릭터1 효과

    // 캐릭터2
    public string CH2_NAME { get; protected set; } = null; // 캐릭터2 이름
    public string CH2_EMOTION { get; protected set; } = null; // 캐릭터2 감정
    public Dialog_CharEffect CH2_EFFECT { get; protected set; } = Dialog_CharEffect.None; // 캐릭터2 효과

    // 캐릭터3
    public string CH3_NAME { get; protected set; } = null; // 캐릭터3 이름
    public string CH3_EMOTION { get; protected set; } = null; // 캐릭터3 감정
    public Dialog_CharEffect CH3_EFFECT { get; protected set; } = Dialog_CharEffect.None; // 캐릭터3 효과

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
            this.ID = int.Parse(GetText(_index));
            _index += 1;
            this.INDEX = int.Parse(GetText(_index));
            _index += 1;

            if (!int.TryParse(GetText(_index), out int nextId))
                this.NEXT_ID = -100; // 예외 발생시 기본값 0으로 설정
            _index += 1;

            this.TARGET_NAME = GetText(_index);
            _index += 1;
            this.TARGET_EMOTION = GetText(_index);
            _index += 1;
            this.TARGET_INTERACT = GetText(_index);
            _index += 1;
            this.TARGET_EFFECT = (Dialog_CharEffect)int.Parse(GetText(_index) == "" ? "1" : GetText(_index));
            _index += 1;

            this.SPEAKER = GetText(_index);
            _index += 1;
            this.DIALOGUE = GetText(_index);
            _index += 1;

            this.BGM = LoadAudioAssetByName(GetText(_index));
            _index += 1;
            this.BGM_EFFECT = (DialogSoundPlayType)int.Parse(GetText(_index) == "" ? "0" : GetText(_index));
            _index += 1;

            this.BGM_EFFECT = (DialogSoundPlayType)int.Parse(GetText(_index) == "" ? "0" : GetText(_index));
            _index += 1;
            this.CG = GetText(_index) != "" ? Resources.Load<Sprite>($"CG/{GetText(_index)}") : null;
            _index += 1;
            this.BG = GetText(_index);
            _index += 1;
            this.SE1 = LoadAudioAssetByName(GetText(_index));
            _index += 1;
            this.SE1_EFFECT = int.Parse(GetText(_index) == "" ? "0" : GetText(_index));
            _index += 1;
            //this.SE1_Delay = float.Parse(GetText(_index) == "" ? "0" : GetText(_index));
            //_index += 1;

            this.CH1_NAME = GetText(_index);
            _index += 1;
            this.CH1_EMOTION = GetText(_index);
            _index += 1;
            this.CH1_EFFECT = (Dialog_CharEffect)int.Parse(GetText(_index) == "" ? "1" : GetText(_index));
            _index += 1;
            //this.CH2_NAME = GetText(_index);
            //_index += 1;
            //this.CH2_EMOTION = GetText(_index);
            //_index += 1;
            //this.CH2_EFFECT = (Dialog_CharEffect)int.Parse(GetText(_index) == "" ? "1" : GetText(_index));
            //_index += 1;
            //this.CH3_NAME = GetText(_index);
            //_index += 1;
            //this.CH3_EMOTION = GetText(_index);
            //_index += 1;
            //this.CH3_EFFECT = (Dialog_CharEffect)int.Parse(GetText(25) == "" ? "1" : GetText(_index));
            //_index += 1;

            this.SE2 = LoadAudioAssetByName(GetText(_index));
            _index += 1;
            this.SE2_EFFECT = int.Parse(GetText(_index) == "" ? "0" : GetText(_index));
            _index += 1;
            //this.SE2_Delay = float.Parse(GetText(_index) == "" ? "0" : GetText(_index));
            //_index += 1;

            this.CHOICE1_ID = int.Parse(GetText(_index) == "" ? "0" : GetText(_index));
            _index += 1;
            this.CHOICE1_TEXT = GetText(_index);
            _index += 1;
            this.CHOICE2_ID = int.Parse(GetText(_index) == "" ? "0" : GetText(_index));
            _index += 1;
            this.CHOICE2_TEXT = GetText(_index);
            _index += 1;
            this.CHOICE3_ID = int.Parse(GetText(_index) == "" ? "0" : GetText(_index));
            _index += 1;
            this.CHOICE3_TEXT = GetText(_index);
            _index += 1;

            this.EVIDENCE_ID = int.Parse(GetText(_index) == "" ? "0" : GetText(_index));
            _index += 1;
            this.EVIDENCE_NEXT_ID = int.Parse(GetText(_index) == "" ? "0" : GetText(_index));
            _index += 1;
        }
        catch (Exception e)
        {
            Debug.Log($"[SetProperty Error] Row Data: {this.ID}:{this.INDEX} → {_index} = data : {GetText(_index)}\n{e.Message}");
        }

    }

    protected string GetText(int index) => 
        node == null ? (row.Length > index && row[index] != null) ? row[index].Trim() : ""
        : (node.Count > index && node[index] != null) ? node[index].Value.Trim() : "";

    protected AudioClip LoadAudioClipByName(string clipName) =>
         Resources.Load<AudioClip>($"Audio/{clipName}");

    protected SoundAsset LoadAudioAssetByName(string clipName) =>
         Resources.Load<SoundAsset>($"Audio/SoundAsset/{clipName}");

    public override string ToString()
    {
        //return base.ToString();
        return JsonConvert.SerializeObject(this);
    }
}
