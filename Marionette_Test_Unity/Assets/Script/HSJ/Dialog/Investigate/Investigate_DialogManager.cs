using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class Investigate_DialogManager : MonoBehaviour
{
    public static Investigate_DialogManager instance;

    //Default Values
    SaveDatabase database;
    EffectManager effectManager;


    Investigate_DialogueData[] dialogs;
    public int currentIndex = 0;

    //대화 관련 변수들
    [SerializeField] Transform[] charPos;
    [SerializeField] Image[] charsImg; //자식오브젝트에 얼굴있음
    [SerializeField] Image dialogBG;
    [SerializeField] TextMeshProUGUI nameLabel;
    [SerializeField] TextMeshProUGUI dialogLabel;
    [SerializeField] Image cutscene;



    /// <summary> null이면 Play메서드 실행 </summary>
    UnityAction onNextProductionAcion = null;
    /// <summary> null이면 Play메서드 실행 </summary>
    Coroutine[] nextProductionCoroutine = null;
    
    public Investigate_DialogueData data { get; protected set; }
    private Dictionary<string, string> characterNameMap = new Dictionary<string, string>()
    {
        { "김주한", "JUHAN" },
        { "설은비", "EUNBI" },
        { "한아영", "AHYOUNG" },
        { "하서하", "SEOHA" },
        { "유무구", "MUGU" },
        { "정해온", "HAEWON" },
        { "도민결", "MINGYEOL" },
        { "배수경", "SUKYUNG" },
        { "권하루", "HARU" },
        { "박세진", "SEJIN" },
        { "백이후", "IHU" },
        { "강세령", "SERYEONG" },
        { "최범식", "BEOMSIK" },
        { "나율", "YUL" },
        { "이시아", "SIA" }

                // 필요한 만큼 추가
    };

    [Space(10)]
    [Header("Effect Values")]
    [SerializeField] AudioSource bgmAudio;
    [SerializeField] AudioSource se1Audio;
    [SerializeField] AudioSource se2Audio;
    [SerializeField] DialogEffectManager_UI uiEffectManager;
    InteractiveDebate_UIManager uiManager;
    //[SerializeField] CharacterPositionManager characterPositionManager;

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

        if (bgmAudio == null)
            bgmAudio = DialogSoundManager.Instance.bgmSource;
        if (se1Audio == null)
            se1Audio = DialogSoundManager.Instance.seSource1;
        if (se2Audio == null)
            se2Audio = DialogSoundManager.Instance.seSource2;

        if(bgmAudio == null)
            bgmAudio = gameObject.AddComponent<AudioSource>();
        if(se1Audio == null)
            se1Audio = gameObject.AddComponent<AudioSource>();
        if (se2Audio == null)
            se2Audio = gameObject.AddComponent<AudioSource>();
    }
    
    public void SetDialogs_ID(int id)
    {
        SetDialogs(id, isIndexInit: true, isPlaying: true);
    }

    public void UI_Active(bool isActive = true, bool isSFXOFF = true)
    {
         foreach(Image img in charsImg)
            img.gameObject.SetActive(isActive);
        dialogBG.gameObject.SetActive(isActive);
        nameLabel.gameObject.SetActive(isActive);
        dialogLabel.gameObject.SetActive(isActive);
        cutscene.gameObject.SetActive(isActive);

        if (isActive == false)
            SFX_OFF();
    }

    public void SFX_OFF()
    {
        if(bgmAudio.isPlaying)
            bgmAudio.Stop();
        if(se1Audio.isPlaying)
            se1Audio.Stop();
        if(se2Audio.isPlaying)
            se2Audio.Stop();
    }



    public void SetDialogs(int id, bool isIndexInit = false, bool isPlaying = false)
    {
        //data = database.GetDialogs_NeedID(id);
        dialogs = database.Get_InvestigateDialogs_NeedID(id);
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
        data = dialogs[currentIndex];

        onNextProductionAcion = Step1;
        onNextProductionAcion.Invoke();
    }

    /// <summary> BGM 재생 </summary>
    void Step1()
    {
        Debug.Log("Step1");
        onNextProductionAcion = Step2;

        // 1. TODO bgm재생

        if (data.BGM != null)
        {
            bool isBGMEqual = (bgmAudio.clip == null ? "" : bgmAudio.clip.name) == (data.BGM == null ? "" : data.BGM.dialogSE.clip == null ? "" : data.BGM.dialogSE.clip.name);
            
            if(bgmAudio.clip != data.BGM.dialogSE.clip)
            {
                if(bgmAudio.isPlaying)
                bgmAudio.Stop();
                bgmAudio.clip = data.BGM.dialogSE.clip;
            }
            bgmAudio.Play();

            if (nextProductionCoroutine == null)
                nextProductionCoroutine = new Coroutine[4];
            nextProductionCoroutine[0] = StartCoroutine(NextAction(action: onNextProductionAcion, delay: 1f));
        }
        else
        {
            onNextProductionAcion.Invoke();
        }
    }

    /// <summary> SE1 재생 / 컷씬연출 재생 </summary>
    void Step2()
    {
        Debug.Log("Step2");
        onNextProductionAcion = Step3;

        if(nextProductionCoroutine != null && nextProductionCoroutine[0] != null)
            StopCoroutine(nextProductionCoroutine[0]);
        nextProductionCoroutine = null;

        // TODO se1재생
        if(data.SE1 != null)
            SEPlayEffect(se1Audio, data.SE1.dialogSE);
        if(data.CG != null)
        {
            if(cutscene.gameObject.activeSelf == false)
                cutscene.gameObject.SetActive(true);
            if(cutscene.sprite != data.CG)
                cutscene.sprite = data.CG;
            nextProductionCoroutine[0] = StartCoroutine(NextAction(action: onNextProductionAcion, delay: 1f));
        }
        else if(cutscene.gameObject.activeSelf == true)
        {
            cutscene.gameObject.SetActive(false);
            onNextProductionAcion?.Invoke();
        }

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

        bool[] posUsed = new bool[charsImg.Length];


        if(data.CH1_POS > -1 && data.CH1_POS < charsImg.Length)
        {
            ShowCharacter(data.CH1_NAME, data.STATE_HEAD_1, data.STATE_BODY_1, data.CH1_POS, data.CH1_EFFECT);
        }
        else
        {
            charsImg[0].sprite = null;
            var img = charsImg[0].transform.GetChild(0).GetComponent<Image>();
            img.sprite = null;
            img.gameObject.SetActive(false);
            charsImg[0].gameObject.SetActive(false);
        }

        if (data.CH2_POS > -1 && data.CH2_POS < charsImg.Length)
        {
            ShowCharacter(data.CH2_NAME, data.STATE_HEAD_2, data.STATE_BODY_2, data.CH2_POS, data.CH2_EFFECT);
        }
        else
        {
            charsImg[1].sprite = null;
            var img = charsImg[1].transform.GetChild(0).GetComponent<Image>();
            img.sprite = null;
            img.gameObject.SetActive(false);
            charsImg[1].gameObject.SetActive(false);
        }


        for (int i = 0; i < charsImg.Length; i++)
        {
            if (!posUsed[i])
            {
                charsImg[i].sprite = null;
                var img = charsImg[i].transform.GetChild(0).GetComponent<Image>();
                img.sprite = null;
                img.gameObject.SetActive(false);
                charsImg[i].gameObject.SetActive(false);
            }
        }

        nextProductionCoroutine[0] = StartCoroutine(uiEffectManager.RunCharacterEffect(data.CH1_EFFECT, uiManager.answer));
        nextProductionCoroutine[1] = StartCoroutine(uiEffectManager.RunCharacterEffect(data.CH2_EFFECT, uiManager.answer));

        // TODO se2재생
        SEPlayEffect(se2Audio, data.SE2.dialogSE);

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
            if (nextProductionCoroutine[0] != null)
                StopCoroutine(nextProductionCoroutine[0]);
            if (nextProductionCoroutine[1] != null)
                StopCoroutine(nextProductionCoroutine[1]);
            nextProductionCoroutine = null;
        }

        // TODO 대사 출력
        if (data.DIALOGUE != null)
        {
            uiManager.AddDialog(name: data.SPEAKER, text: data.DIALOGUE, () => onNextProductionAcion?.Invoke());
        }
        else
        {
            onNextProductionAcion?.Invoke();
        }
    }

    /// <summary> 감정 게이지 보여주기 </summary>
    void Step5()
    {
        onNextProductionAcion = null;

        // 다음 id가 설정되어있는경우 다음꺼 보여주기
        if (data.NEXT_ID != -1 && data.NEXT_ID != 0)
        {
            SetDialogs(data.NEXT_ID, isIndexInit: true, isPlaying: true);
        }
        // 다음대사 실행하기
        // TODO 다음대사 없고 할당이 안되어있으면 에러날듯
        else if(dialogs.Length <= currentIndex + 1)
        {
            currentIndex += 1;
        }
        else
        {
            //TODO OFF
            UI_Active(false);
        }
    }

    private void ShowCharacter(string name, string head, string body, int pos, Dialog_CharEffect effect)
    {
        int posIndex = (int)pos;
        if (posIndex < 0 || posIndex >= charsImg.Length) return;

        var headRenderer = charsImg[posIndex];
        var bodyRenderer = charsImg[posIndex].transform.GetChild(0).GetComponent<Image>();

        string englishName = characterNameMap.ContainsKey(name) ? characterNameMap[name] : name;

        string headSpriteName = $"{head}";
        Sprite headSprite = LoadSpriteForSpeaker(name, headSpriteName);
        if (headSprite != null)
        {
            headRenderer.sprite = headSprite;
            headRenderer.gameObject.SetActive(true);
        }
        else
        {
            headRenderer.sprite = null;
            headRenderer.gameObject.SetActive(false);
            Debug.LogWarning($"[머리 스프라이트 미적용] {headSpriteName}를 {name} 폴더에서 못 찾음");
        }

        string bodySpriteName = $"{body}";
        Sprite bodySprite = LoadSpriteForSpeaker(name, bodySpriteName);
        if (bodySprite != null)
        {
            bodyRenderer.sprite = bodySprite;
            bodyRenderer.gameObject.SetActive(true);
        }
        else
        {
            bodyRenderer.sprite = null;
            bodyRenderer.gameObject.SetActive(false);
            Debug.LogWarning($"[몸통 스프라이트 미적용] {bodySpriteName}를 {name} 폴더에서 못 찾음");
        }

        //if (characterPositionManager != null)
        //{
        //    Vector3 basePos = characterPositionManager.GetPositionByCharPos((Dialog_CharPos)pos);

        //    // 머리와 몸의 부모 컨테이너가 동일하다고 가정
        //    Transform container = headRenderer.transform.parent;
        //    if (container != null)
        //    {
        //        container.position = basePos;
        //        // 머리와 몸의 localPosition은 인스펙터에서 조절한 값 유지됨
        //    }
        //    else
        //    {
        //        Debug.LogWarning("머리 스프라이트에 부모 컨테이너가 없습니다. 위치가 이상할 수 있습니다.");
        //        // 부모 없으면 기존 방식 유지 (긴급 대비)
        //        //headRenderer.transform.position = basePos + headRenderer.transform.localPosition;
        //        //bodyRenderer.transform.position = basePos + bodyRenderer.transform.localPosition;
        //    }
        //}

        //if (effect != Dialog_CharEffect.None)
        //{
        //    StartCoroutine(uiEffectManager.RunCharacterEffect(effect, headRenderer));
        //    StartCoroutine(uiEffectManager.RunCharacterEffect(effect, bodyRenderer));
        //}
    }

    private Sprite LoadSpriteForSpeaker(string speakerName, string spriteName)
    {
        string folderName = speakerName;
        if (characterNameMap.ContainsKey(speakerName))
        {
            folderName = characterNameMap[speakerName];
        }

        string path = $"Sprites/Characters/{folderName}/{spriteName}";
        Sprite sprite = Resources.Load<Sprite>(path);
        if (sprite == null)
        {
            Debug.LogWarning($"[LoadSpriteForSpeaker] 스프라이트를 찾지 못함: {path}");
        }
        return sprite;
    }

    void SEPlayEffect(AudioSource audio, DialogSE se)
    {
        //bool isWait = false;

        if (se != null)
        {
            bool isNameEqual = (audio.clip == null ? "" : audio.clip.name) == (se == null ? "" : se.clip == null ? "" : se.clip.name);
            audio.loop = false;
            if (audio.isPlaying)
                audio.Stop();
            audio.clip = se.clip;
            //isWait = true;
            audio.Play();
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
