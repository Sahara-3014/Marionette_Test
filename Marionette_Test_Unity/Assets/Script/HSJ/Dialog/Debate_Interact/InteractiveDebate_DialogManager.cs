using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

public class InteractiveDebate_DialogManager : MonoBehaviour
{
    public static InteractiveDebate_DialogManager instance;

    //Default Values
    SaveDatabase database;
    EffectManager effectManager;


    InteractiveDebate_DialogueData[] dialogs;
    public int currentIndex = 0;

    //대화 관련 변수들
    /// <summary> null이면 Play메서드 실행 </summary>
    UnityAction onNextProductionAcion = null;
    /// <summary> null이면 Play메서드 실행 </summary>
    Coroutine[] nextProductionCoroutine = null;
    
    public InteractiveDebate_DialogueData data { get; protected set; }

    [Space(10)]
    [Header("Effect Values")]
    [SerializeField] AudioSource bgmAudio;
    [SerializeField] AudioSource se1Audio;
    [SerializeField] AudioSource se2Audio;
    [SerializeField] DialogEffectManager dialogEffectManager;
    [SerializeField] DialogEffectManager_UI uiEffectManager;
    InteractiveDebate_UIManager uiManager;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        effectManager = EffectManager.Instance;
        database = SaveDatabase.Instance;
        uiManager = GetComponent<InteractiveDebate_UIManager>();
        currentIndex = 0;

        if(bgmAudio == null)
            bgmAudio = gameObject.AddComponent<AudioSource>();
        if(se1Audio == null)
            se1Audio = gameObject.AddComponent<AudioSource>();
        if (se2Audio == null)
            se2Audio = gameObject.AddComponent<AudioSource>();

        if (dialogEffectManager == null)
            dialogEffectManager = GetComponent<DialogEffectManager>();
    }
    
    public void SetDialogs_ID(int id)
    {
        SetDialogs(id, isIndexInit: true, isPlaying: true);
    }


    public void SetDialogs(int id, bool isIndexInit = false, bool isPlaying = false)
    {
        //data = database.GetDialogs_NeedID(id);
        dialogs = database.Get_DebateDialogs_NeedID(id);
        for (int i = 0; i < dialogs.Length; i++)
            Debug.Log(dialogs[i].ToString());
        if (isIndexInit)
            currentIndex = 0;

        

        if (isPlaying)
        {
            currentIndex = 0; 
            Play();
        }
    }

    public void Play()
    {
        Debug.Log("Play");
        if (onNextProductionAcion != null)
        {
            onNextProductionAcion.Invoke();
            return;
        }
        if (uiManager.IsChoicePanelOpened())
            return;
        if (dialogs[currentIndex].EVIDENCE_ID != 0)
            return;
        Debug.Log("Play_Next");
        data = dialogs[currentIndex];

        if(currentIndex == 0)
        {
            uiManager.ActiveAbilityGauge(data.DEBATE_TYPE == 1);
            uiManager.SnapScrollArrowActive(data.DEBATE_TYPE == 1);
            InventoryManager.Instance.InventoryIcon.SetActive(data.DEBATE_TYPE == 1);
            if(data.DEBATE_TYPE == 1)
                uiManager.ClearDialog();
        }

        Debug.Log($"data [{currentIndex}] : {data.ToString()}");

        onNextProductionAcion = Step1;
        onNextProductionAcion.Invoke();
    }

    /// <summary> BGM 재생 </summary>
    void Step1()
    {
        Debug.Log("Step1");
        onNextProductionAcion = Step2;

        // 1. TODO bgm재생
        bool isWait = false;
        switch (data.BGM_EFFECT)
        {
            // -3
            case (int)InteractiveDebate_DialogueData.DialogSoundPlayType.Continue:
                bgmAudio.UnPause();
                break;
            // -2
            case (int)InteractiveDebate_DialogueData.DialogSoundPlayType.Pause:
                if (bgmAudio.isPlaying)
                    bgmAudio.Pause();
                break;
            // -1
            case (int)InteractiveDebate_DialogueData.DialogSoundPlayType.Stop:
                if (bgmAudio.isPlaying)
                    bgmAudio.Stop();
                break;
            // 12
            case (int)InteractiveDebate_DialogueData.DialogSoundPlayType.FadeOut:
                isWait = true;
                if (bgmAudio.loop)
                    bgmAudio.loop = false;
                bgmAudio.DOFade(0f, .5f).OnComplete(()=> isWait = false);
                break;
        }

        if (data.BGM != null)
        {
            bool isBGMEqual = (bgmAudio.clip == null ? "" : bgmAudio.clip.name) == (data.BGM == null ? "" : data.BGM.dialogSE.clip == null ? "" : data.BGM.dialogSE.clip.name);
            switch (data.BGM_EFFECT)
            {
                // 1
                case (int)InteractiveDebate_DialogueData.DialogSoundPlayType.PlayOnShot:
                    if (bgmAudio.loop)
                        bgmAudio.loop = false;
                    if (bgmAudio.isPlaying)
                        bgmAudio.Stop();
                    bgmAudio.clip = data.BGM.dialogSE.clip;
                    bgmAudio.volume = data.BGM.dialogSE.volume;
                    bgmAudio.Play();
                    break;

                // 11
                case (int)InteractiveDebate_DialogueData.DialogSoundPlayType.FadeIn:
                    if (bgmAudio.loop)
                        bgmAudio.loop = false;
                    if (bgmAudio.isPlaying)
                        bgmAudio.Stop();
                    bgmAudio.clip = data.BGM.dialogSE.clip;
                    bgmAudio.volume = 0f;
                    bgmAudio.Play();
                    bgmAudio.DOFade(bgmAudio.volume, .5f);

                    break;
                // 13
                case (int)InteractiveDebate_DialogueData.DialogSoundPlayType.FadeOutToIn:
                    if (bgmAudio.loop)
                        bgmAudio.loop = false;
                    if (bgmAudio.isPlaying)
                    {
                        bgmAudio.DOFade(0f, .5f).OnComplete(() =>
                        {
                            bgmAudio.Stop();
                            bgmAudio.clip = data.BGM.dialogSE.clip;
                            bgmAudio.Play();
                            bgmAudio.DOFade(bgmAudio.volume, .5f);
                        });
                    }
                    else
                    {
                        bgmAudio.volume = 0f;
                        bgmAudio.clip = data.BGM.dialogSE.clip;
                        bgmAudio.Play();
                        bgmAudio.DOFade(bgmAudio.volume, .5f);
                    }
                    break;
                // 0
                case (int)InteractiveDebate_DialogueData.DialogSoundPlayType.PlayLoop:
                    if (isBGMEqual == false)
                    {
                        if(bgmAudio.isPlaying)
                            bgmAudio.Stop();
                        bgmAudio.loop = true;
                        bgmAudio.volume = data.BGM.dialogSE.volume;
                        bgmAudio.clip = data.BGM.dialogSE.clip;
                        bgmAudio.Play();
                    }
                    break;
            }

            if (nextProductionCoroutine == null)
                nextProductionCoroutine = new Coroutine[4];
            nextProductionCoroutine[0] = StartCoroutine(NextAction(action: onNextProductionAcion, delay: 1f));
        }
        else
        {
            if (isWait)
                nextProductionCoroutine[0] = StartCoroutine(NextAction(action: onNextProductionAcion, delay: 0.5f));
            else
                onNextProductionAcion.Invoke();
        }
    }

    /// <summary> SE1 재생 / 배경연출 재생 </summary>
    async void Step2()
    {
        Debug.Log("Step2");
        onNextProductionAcion = Step3;
        if(nextProductionCoroutine != null && nextProductionCoroutine[0] != null)
            StopCoroutine(nextProductionCoroutine[0]);
        nextProductionCoroutine = null;

        // TODO 배경연출 재생
        if (nextProductionCoroutine == null)
            nextProductionCoroutine = new Coroutine[4];

        Debug.Log("debateData.screenEffect: " + data.BGEffect+$"{uiManager.BG == null}");

        bool isComplete = true;
        if((Dialog_ScreenEffect)data.BGEffect != Dialog_ScreenEffect.None)
        {
            isComplete = false;
            nextProductionCoroutine[0] = StartCoroutine(uiEffectManager.RunScreenEffect((Dialog_ScreenEffect)data.BGEffect, uiManager.BG, () => isComplete = true));
        }

        Debug.Log($"debateData: {nextProductionCoroutine[0] == null}");

        // TODO se1재생
        if(data.SE1 != null)
            SEPlayEffect(se1Audio, data.SE1.dialogSE, data.SE1_EFFECT);
        // TODO 기다리고 바로 실행하기
        while(!isComplete)
        {
            await Task.Yield();
        }

        onNextProductionAcion?.Invoke();
    }

    /// <summary> SE2 재생 / 캐릭터 연출 재생 </summary>
    async void Step3()
    {
        Debug.Log("Step3");
        onNextProductionAcion = Step4;
        if(nextProductionCoroutine != null && nextProductionCoroutine[0] != null)
            StopCoroutine(nextProductionCoroutine[0]);
        nextProductionCoroutine = null;

        if (nextProductionCoroutine == null)
            nextProductionCoroutine = new Coroutine[4];

        if (data.TARGET_NAME != "" && data.TARGET_BODY != "" || data.TARGET_HEAD != "")
            uiManager.ChangeCharacter(true, data.TARGET_NAME, data.TARGET_HEAD, data.TARGET_BODY);
        else
            uiManager.ChangeCharacter(true, data.TARGET_NAME);
        if (data.CH1_NAME != "" && data.CH1_BODY != "" || data.CH1_HEAD != "")
            uiManager.ChangeCharacter(false, data.CH1_NAME, data.CH1_HEAD, data.CH1_BODY);
        else
            uiManager.ChangeCharacter(false, data.CH1_NAME);
            

        // TODO 캐릭터연출 재생 <- 타입에 따라서 프레임에 먹일 이펙트/캐릭터에 먹일 이펙트 분리해야함
        if((Dialog_CharEffect)data.TARGET_EFFECT != Dialog_CharEffect.None)
        nextProductionCoroutine[0] = StartCoroutine(dialogEffectManager.RunCharacterEffect((Dialog_CharEffect)data.TARGET_EFFECT, uiManager.target));
        if((Dialog_CharEffect)data.CH1_EFFECT != Dialog_CharEffect.None)
        nextProductionCoroutine[1] = StartCoroutine(dialogEffectManager.RunCharacterEffect((Dialog_CharEffect)data.CH1_EFFECT, uiManager.answer));



        // TODO se2재생
        if (data.SE2 != null)
            SEPlayEffect(se2Audio, data.SE2.dialogSE, data.SE2_EFFECT);


        // TODO 기다리고 바로 실행하기
        Debug.Log("Step3 Waiting");
        await Task.Delay(TimeSpan.FromSeconds(uiEffectManager.duration));
        onNextProductionAcion?.Invoke();
    }

    /// <summary> 대사 출력 / 다음 대사 넘어가기 </summary>
    void Step4()
    {
        Debug.Log("Step4");
        // TOOD 다음 대사로 넘어가기
        onNextProductionAcion = ()=>
        {
            if(se2Audio.isPlaying)
                se2Audio.Stop();
            if(uiManager.skipAction != null)
            {
                uiManager.skipAction.Invoke();
                uiManager.skipAction = null;
            }
            else
            {
                Step5();
            }
        };

        //이전 스텝 변수 초기화
        if(nextProductionCoroutine != null)
        {
            if (nextProductionCoroutine[1] != null)
                StopCoroutine(nextProductionCoroutine[1]);
            if (nextProductionCoroutine[2] != null)
                StopCoroutine(nextProductionCoroutine[2]);
            if (nextProductionCoroutine[3] != null)
                StopCoroutine(nextProductionCoroutine[3]);
            nextProductionCoroutine = null;
        }

        // TODO 대사 출력
        if (data.DIALOGUE != null)
        {
            uiManager.AddDialog(name: data.SPEAKER, text: data.DIALOGUE, ()=>onNextProductionAcion?.Invoke());
        }
    }

    /// <summary> 감정 게이지 보여주기 </summary>
    void Step5()
    {
        onNextProductionAcion = null;

        if (data.TARGET_INTERACT != null && data.TARGET_INTERACT != "")
        {
            Debug.Log("Last");

            string[] interact = data.TARGET_INTERACT.Split('/');

            CharAttributeData.CharAttributeType interactType = (CharAttributeData.CharAttributeType)Enum.Parse(typeof(CharAttributeData.CharAttributeType), interact[0]);
            int value = int.Parse(interact[1]);
            int nowValue = database.SaveData_GetCharData_GetGauge(data.TARGET_NAME, interactType).value;

            database.SaveData_SetCharData_SetGauge(data.TARGET_NAME, interactType, nowValue + value);
            uiManager.ChangeAbilityGauge(interactType);
        }


        // 선택지 보여주기
        Debug.Log($"select id : {data.CHOICE1_ID} / next : {data.NEXT_ID} / {dialogs.Length <= currentIndex + 1}");
        if (data.CHOICE1_ID != 0)
        {
            List<(int, string)> choices = new();
            choices.Add((data.CHOICE1_ID, data.CHOICE1_TEXT));
            if (data.CHOICE2_ID != 0)
                choices.Add((data.CHOICE2_ID, data.CHOICE2_TEXT));
            if (data.CHOICE3_ID != 0)
                choices.Add((data.CHOICE3_ID, data.CHOICE3_TEXT));

            uiManager.OpenChoicePanel(choices);
        }
        // 다음 id가 설정되어있는경우 다음꺼 보여주기
        else if (data.NEXT_ID > 0)
        {
            SetDialogs(data.NEXT_ID, isIndexInit: true);
        }
        // 다음대사 실행하기
        // TODO 다음대사 없고 할당이 안되어있으면 에러날듯
        else if(dialogs.Length > currentIndex + 1)
        {
            currentIndex += 1;
        }
    }

    async void SEPlayEffect(AudioSource audio, DialogSE se, int effect)
    {
        //bool isWait = false;
        switch (effect)
        {
            // -3
            case (int)InteractiveDebate_DialogueData.DialogSoundPlayType.Continue:
                if (!audio.isPlaying)
                    audio.UnPause();
                break;
            // -2
            case (int)InteractiveDebate_DialogueData.DialogSoundPlayType.Pause:
                if (audio.isPlaying)
                    audio.Pause();
                break;
            // -1
            case (int)InteractiveDebate_DialogueData.DialogSoundPlayType.Stop:
                if (audio.isPlaying)
                    audio.Stop();
                break;
            // 12
            case (int)InteractiveDebate_DialogueData.DialogSoundPlayType.FadeOut:
                if (audio.isPlaying)
                {
                    //isWait = true;
                    audio.DOFade(0f, .5f);//.OnComplete(() => isWait = false);
                }
                break;
        }

        if (se != null)
        {
            bool isNameEqual = (audio.clip == null ? "" : audio.clip.name) == (se == null ? "" : se.clip == null ? "" : se.clip.name);
            switch (effect)
            {
                // 0
                case (int)InteractiveDebate_DialogueData.DialogSoundPlayType.PlayLoop:
                    audio.loop = true;
                    audio.clip = se.clip;
                    audio.volume = se.volume;
                    audio.Play();
                    break;
                // 11
                case (int)InteractiveDebate_DialogueData.DialogSoundPlayType.FadeIn:
                    if (audio.isPlaying)
                        audio.Stop();
                    audio.loop = false;
                    audio.clip = se.clip;
                    audio.volume = 0f;
                    audio.Play();
                    audio.DOFade(se.volume, .5f);
                    break;
                // 13
                case (int)InteractiveDebate_DialogueData.DialogSoundPlayType.FadeOutToIn:
                    audio.loop = false;
                    if (audio.isPlaying)
                    {
                        audio.DOFade(0f, .5f).OnComplete(() =>
                        {
                            audio.Stop();
                            audio.clip = se.clip;
                            audio.Play();
                            audio.DOFade(se.volume, .5f);
                        });
                    }
                    else
                    {
                        audio.volume = 0f;
                        audio.clip = se.clip;
                        audio.Play();
                        audio.DOFade(se.volume, .5f);
                    }
                    break;
                // 1~10
                default:
                    audio.loop = false;
                    if (audio.isPlaying)
                        audio.Stop();
                    audio.clip = se.clip;
                    //isWait = true;
                    var time = TimeSpan.FromSeconds(se.clip.length + 0.1f);
                    for (int i = 1; i < effect; i++)
                    {
                        audio.Play();
                        await Task.Delay(time);
                    }
                    //isWait = false;
                    break;
            }
        }
    }

    IEnumerator NextAction(float delay, UnityAction action = null, Coroutine coroutine = null)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);
        if(action != null)
            action.Invoke();
        if (coroutine != null)
            StopCoroutine(coroutine);
    }
}
