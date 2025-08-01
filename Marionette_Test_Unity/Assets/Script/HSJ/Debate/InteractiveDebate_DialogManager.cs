using UnityEngine;
using UnityEngine.Events;

public class InteractiveDebate_DialogManager : MonoBehaviour
{
    #region enum
    #endregion




    //Default Values
    SaveDatabase database;
    EffectManager effectManager;


    InteractiveDebate_DialogueData[] dialogs;
    DialogueData[] data;
    int currentIndex = 0;

    //대화 관련 변수들
    UnityAction onNextProduction = null;

    [Space(10)]
    [Header("Effect Values")]
    [SerializeField] AudioSource bgmAudio;
    [SerializeField] AudioSource se1Audio;
    [SerializeField] AudioSource se2Audio;
    [SerializeField] DialogEffectManager dialogEffectManager;



    private void Start()
    {
        effectManager = EffectManager.Instance;
        database = SaveDatabase.Instance;
        currentIndex = 0;

        if(bgmAudio == null)
            bgmAudio = gameObject.AddComponent<AudioSource>();
        if(se1Audio == null)
            se1Audio = gameObject.AddComponent<AudioSource>();
        if (se2Audio == null)
            se2Audio = gameObject.AddComponent<AudioSource>();
    }



    public void SetDialogs(int id, bool isIndexInit = false, bool isPlaying = false)
    {
        data = database.GetDialogs_NeedID(id);
        if(isIndexInit)
            currentIndex = 0;

        if (isPlaying)
        {
            currentIndex = 0; 
            Play();
        }
    }

    public void Play()
    {
        // TODO bgm재생
        // TODO se1재생
        // TODO 배경연출 재생


        // TODO se2재생
        // TODO 캐릭터연출 재생
        // TODO 대사 출력

        // TOOD 다음 대사로 넘어가기
        currentIndex++;
    }
}
