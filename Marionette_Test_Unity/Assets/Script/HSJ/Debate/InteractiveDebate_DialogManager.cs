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
    DialogueData[] data; // 임시
    public int currentIndex = 0;

    //대화 관련 변수들
    /// <summary> null이면 Play메서드 실행 </summary>
    UnityAction onNextProductionAcion = null;
    /// <summary> null이면 Play메서드 실행 </summary>
    Coroutine[] nextProductionCoroutine = null;
    
    public InteractiveDebate_DialogueData debateData { get; protected set; }

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
        //if (uiManager.IsChoicePanelOpened())
        //    return;
        Debug.Log("Play_Next");
        debateData = dialogs[currentIndex];

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
        switch (debateData.BGM_EFFECT)
        {
            // -3
            case InteractiveDebate_DialogueData.DialogSoundPlayType.Continue:
                bgmAudio.UnPause();
                break;
            // -2
            case InteractiveDebate_DialogueData.DialogSoundPlayType.Pause:
                if (bgmAudio.isPlaying)
                    bgmAudio.Pause();
                break;
            // -1
            case InteractiveDebate_DialogueData.DialogSoundPlayType.Stop:
                if (bgmAudio.isPlaying)
                    bgmAudio.Stop();
                break;
            // 12
            case InteractiveDebate_DialogueData.DialogSoundPlayType.FadeOut:
                isWait = true;
                if (bgmAudio.loop)
                    bgmAudio.loop = false;
                bgmAudio.DOFade(0f, .5f).OnComplete(()=> isWait = false);
                break;
        }

        if (debateData.BGM != null)
        {
            bool isBGMEqual = (bgmAudio.clip == null ? "" : bgmAudio.clip.name) == (debateData.BGM == null ? "" : debateData.BGM.clip == null ? "" : debateData.BGM.clip.name);
            switch (debateData.BGM_EFFECT)
            {
                // 1
                case InteractiveDebate_DialogueData.DialogSoundPlayType.PlayOnShot:
                    if (bgmAudio.loop)
                        bgmAudio.loop = false;
                    if (bgmAudio.isPlaying)
                        bgmAudio.Stop();
                    bgmAudio.clip = debateData.BGM.clip;
                    bgmAudio.volume = debateData.BGM.volume;
                    bgmAudio.Play();
                    break;

                // 11
                case InteractiveDebate_DialogueData.DialogSoundPlayType.FadeIn:
                    if (bgmAudio.loop)
                        bgmAudio.loop = false;
                    if (bgmAudio.isPlaying)
                        bgmAudio.Stop();
                    bgmAudio.clip = debateData.BGM.clip;
                    bgmAudio.volume = 0f;
                    bgmAudio.Play();
                    bgmAudio.DOFade(bgmAudio.volume, .5f);

                    break;
                // 13
                case InteractiveDebate_DialogueData.DialogSoundPlayType.FadeOutToIn:
                    if (bgmAudio.loop)
                        bgmAudio.loop = false;
                    if (bgmAudio.isPlaying)
                    {
                        bgmAudio.DOFade(0f, .5f).OnComplete(() =>
                        {
                            bgmAudio.Stop();
                            bgmAudio.clip = debateData.BGM.clip;
                            bgmAudio.Play();
                            bgmAudio.DOFade(bgmAudio.volume, .5f);
                        });
                    }
                    else
                    {
                        bgmAudio.volume = 0f;
                        bgmAudio.clip = debateData.BGM.clip;
                        bgmAudio.Play();
                        bgmAudio.DOFade(bgmAudio.volume, .5f);
                    }
                    break;
                // 0
                case InteractiveDebate_DialogueData.DialogSoundPlayType.PlayLoop:
                    if (bgmAudio.isPlaying)
                        bgmAudio.Stop();
                    bgmAudio.loop = true;
                    bgmAudio.volume = debateData.BGM.volume;
                    bgmAudio.clip = debateData.BGM.clip;
                    bgmAudio.Play();
                    break;
            }
            bgmAudio.clip = debateData.BGM.clip;

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

        Debug.Log("debateData.screenEffect: " + debateData.screenEffect+$"{uiManager.BG == null}");

        bool isComplete = false;
        nextProductionCoroutine[0] = StartCoroutine(uiEffectManager.RunScreenEffect(debateData.screenEffect, uiManager.BG, ()=> isComplete = true));

        Debug.Log($"debateData: {nextProductionCoroutine[0] == null}");

        // TODO se1재생
        SEPlayEffect(se1Audio, debateData.SE1, debateData.SE1_EFFECT);
        // TODO 기다리고 바로 실행하기
        while(!isComplete)
        {
            await Task.Yield();
        }

        onNextProductionAcion.Invoke();
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

        // TODO 캐릭터연출 재생 <- 타입에 따라서 프레임에 먹일 이펙트/캐릭터에 먹일 이펙트 분리해야함
        nextProductionCoroutine[0] = StartCoroutine(dialogEffectManager.RunCharacterEffect(debateData.TARGET_EFFECT, uiManager.target));
        nextProductionCoroutine[1] = StartCoroutine(uiEffectManager.RunCharacterEffect(debateData.CH1_EFFECT, uiManager.answer));

        // TODO se2재생
        SEPlayEffect(se2Audio, debateData.SE2, debateData.SE2_EFFECT);

        // TODO 기다리고 바로 실행하기
        Debug.Log("Step3 Waiting");
        await Task.Delay(TimeSpan.FromSeconds(uiEffectManager.duration));
        onNextProductionAcion.Invoke();
    }

    /// <summary> 대사 출력 / 다음 대사 넘어가기 </summary>
    void Step4()
    {
        Debug.Log("Step4");
        // TOOD 다음 대사로 넘어가기
        onNextProductionAcion = ()=>
        {
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
        if (debateData.DIALOGUE != null)
        {
            uiManager.AddDialog(name: debateData.SPEAKER, text: debateData.DIALOGUE);
        }
    }

    /// <summary> 감정 게이지 보여주기 </summary>
    void Step5()
    {
        onNextProductionAcion = null;

        if (debateData.TARGET_INTERACT != null && debateData.TARGET_INTERACT != "")
        {
            Debug.Log("Last");

            string[] interact = debateData.TARGET_INTERACT.Split('/');

            CharAttributeData.CharAttributeType interactType = (CharAttributeData.CharAttributeType)Enum.Parse(typeof(CharAttributeData.CharAttributeType), interact[0], true);
            int value = int.Parse(interact[1]);
            int nowValue = database.SaveData_GetCharData_GetGauge(debateData.TARGET_NAME, interactType).value;

            database.SaveData_SetCharData_SetGauge(debateData.TARGET_NAME, interactType, nowValue + value);
            uiManager.ChangeAbilityGauge(interactType);
        }


        // 선택지 보여주기
        if (debateData.CHOICE1_ID != 0)
        {
            List<(int, string)> choices = new();
            choices.Add((debateData.CHOICE1_ID, debateData.CHOICE1_TEXT));
            if (debateData.CHOICE2_ID != 0)
                choices.Add((debateData.CHOICE2_ID, debateData.CHOICE2_TEXT));
            if (debateData.CHOICE3_ID != 0)
                choices.Add((debateData.CHOICE3_ID, debateData.CHOICE3_TEXT));

            uiManager.OpenChoicePanel(choices);
        }
        // 다음 id가 설정되어있는경우 다음꺼 보여주기
        else if (debateData.NEXT_ID != -1 && debateData.NEXT_ID != 0)
        {
            SetDialogs(debateData.NEXT_ID, isIndexInit: true, isPlaying: true);
        }
        // 다음대사 실행하기
        // TODO 다음대사 없고 할당이 안되어있으면 에러날듯
        else if(dialogs.Length <= currentIndex + 1)
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
                    for (int i = 1; i < effect; i++)
                    {
                        audio.Play();
                        await Task.Delay(TimeSpan.FromSeconds(debateData.SE2_Delay));
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
