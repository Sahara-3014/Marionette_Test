using Newtonsoft.Json;
using SimpleJSON;
using System;
using UnityEngine;

public class Investigate_DialogueData
{
    enum SheetIndex
    {
        ID, INDEX, NEXT_ID,
        SPEAKER,

        CH1_NAME, CH1_POS, CH1_EFFECT,
        STATE_HEAD_1, STATE_BODY_1,

        CH2_NAME, CH2_POS, CH2_EFFECT,
        STATE_HEAD_2, STATE_BODY_2,

        DIALOGUE,

        BGM, SE1, SE2,

        CS
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
    #endregion

    #region Second Action
    /// <summary> 삽입 이미지 (CG 등) </summary>
    public Sprite CG { get; protected set; }

    /// <summary> 화자 대화 시작시 효과음 </summary>
    public SoundAsset SE1 { get; protected set; }
    #endregion

    #region Third Action
    // 캐릭터1
    public string CH1_NAME { get; protected set; } = null; // 캐릭터1 이름
    public int CH1_POS { get; protected set; } = -1; // 캐릭터1 위치
    public string STATE_HEAD_1 { get; protected set; } = null;
    public string STATE_BODY_1 { get; protected set; } = null;
    public Dialog_CharEffect CH1_EFFECT = Dialog_CharEffect.None;

    // 캐릭터2
    public string CH2_NAME { get; protected set; } = null; // 캐릭터2 이름
    public int CH2_POS { get; protected set; } = -1; // 캐릭터2 위치
    public string STATE_HEAD_2 { get; protected set; } = null;
    public string STATE_BODY_2 { get; protected set; } = null;
    public Dialog_CharEffect CH2_EFFECT = Dialog_CharEffect.None;


    // 효과음
    public SoundAsset SE2 { get; protected set; }
    #endregion

    public Investigate_DialogueData(JSONNode nod)
    {
        this.node = nod;
        SetProperty();
    }

    public Investigate_DialogueData(string[] row)
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


            this.SPEAKER = GetText(_index);
            _index += 1;


            this.CH1_NAME = GetText(_index);
            _index += 1;
            if (int.TryParse(GetText(_index), out var pos1) == true)
                this.CH1_POS = pos1;
            else
                this.CH1_POS = 1;
            this.STATE_HEAD_1 = GetText(_index);
            _index += 1;
            this.STATE_BODY_1 = GetText(_index);
            _index += 1;
            if(Enum.TryParse(GetText(_index), out this.CH1_EFFECT) == false)
                this.CH1_EFFECT = Dialog_CharEffect.None;
            _index += 1;


            this.CH2_NAME = GetText(_index);
            _index += 1;
            if (int.TryParse(GetText(_index), out var pos2) == true)
                this.CH2_POS = pos2;
            else
                this.CH2_POS = 1;
            _index += 1;
            this.STATE_HEAD_2 = GetText(_index);
            _index += 1;
            this.STATE_BODY_2 = GetText(_index);
            _index += 1;
            if (Enum.TryParse(GetText(_index), out this.CH2_EFFECT) == false)
                this.CH2_EFFECT = Dialog_CharEffect.None;


            this.DIALOGUE = GetText(_index);
            _index += 1;

            this.BGM = LoadAudioAssetByName(GetText(_index));
            _index += 1;

            this.SE1 = LoadAudioAssetByName(GetText(_index));
            _index += 1;

            this.SE2 = LoadAudioAssetByName(GetText(_index));
            _index += 1;

            this.CG = GetText(_index) != "" ? Resources.Load<Sprite>($"CG/{GetText(_index)}") : null;
            _index += 1;

        }
        catch (Exception e)
        {
            Debug.Log($"[SetProperty Error] Row Data: {this.ID}:{this.INDEX} → {_index} = data : {GetText(_index)}\n{e.Message}");
            throw; // 예외를 다시 던져서 호출자에게 알림
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
        return JsonConvert.SerializeObject(this);
    }
}
