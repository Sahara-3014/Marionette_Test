using UnityEngine;
using System.Collections;

public class DialogSoundManager : MonoBehaviour
{
    public static DialogSoundManager Instance;

    public AudioSource bgmSource;
    public AudioSource seSource1;
    public AudioSource seSource2;
    public AudioSource choiceSeSource3; // 추가
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void PlayDialogSE(DialogSE dialogSE)
    {

        switch (dialogSE.type)
        {
            case SEType.BGM:
                PlayBGM(dialogSE);
                break;

            case SEType.SE:
                PlaySE(dialogSE);
                break;

            default:
                break;
        }
    }
    private string currentBGMName = ""; // 현재 재생 중인 BGM 이름 저장

    public void PlayBGM(DialogSE bgm)
    {
        if (bgmSource == null)
        {
            Debug.LogError("[PlayBGM] bgmSource가 할당되어 있지 않습니다!");
            return;
        }

        if (bgm == null)
        {
            Debug.LogWarning("[PlayBGM] DialogSE가 null입니다.");
            return;
        }

        if (bgm.clip == null)
        {
            Debug.LogWarning("[PlayBGM] 재생할 BGM이 없거나 AudioClip이 null입니다.");
            return;
        }

        // 이미 같은 BGM이 재생 중이면 건너뜀
        if (bgm.clip.name == currentBGMName && bgmSource.isPlaying)
        {
            Debug.Log($"[PlayBGM] '{bgm.clip.name}' 이미 재생 중, 다시 재생하지 않음");
            return;
        }

        currentBGMName = bgm.clip.name;
        bgmSource.clip = bgm.clip;
        bgmSource.volume = bgm.volume;
        bgmSource.loop = (bgm.loopCount == 0);
        bgmSource.Play();

        Debug.Log($"[PlayBGM] BGM '{bgm.clip.name}' 재생 시작 (볼륨: {bgm.volume})");
    }
    public void PlaySE(DialogSE se)
    {
        if (se == null)
            return;

        // -1 명령이면 재생 중이던 효과음 모두 끔
        if (se.stopSE)
        {
            seSource1.Stop();
            seSource2.Stop();
            choiceSeSource3.Stop();
            Debug.Log("[PlaySE] -1 명령 → 모든 효과음 중지");
            return;
        }

        if (se.clip == null) return;

        if (seSource1 == null || seSource2 == null || choiceSeSource3 == null)
        {
            Debug.LogError("[PlaySE] AudioSource 미할당");
            return;
        }

        AudioSource sourceToUse = null;
        if (!seSource1.isPlaying)
            sourceToUse = seSource1;
        else if (!seSource2.isPlaying)
            sourceToUse = seSource2;
        else if (!choiceSeSource3.isPlaying)
            sourceToUse = choiceSeSource3;
        else
        {
            seSource1.Stop();
            sourceToUse = seSource1;
        }

        sourceToUse.clip = se.clip;
        sourceToUse.volume = se.volume;
        sourceToUse.loop = (se.loopCount == 0);
        sourceToUse.Play();

        Debug.Log($"[PlaySE] 효과음 '{se.clip.name}' 재생 시작 (볼륨: {se.volume})");

        if (se.loopCount > 0)
            StartCoroutine(PlaySELoop(sourceToUse, se.loopCount));
    }





    private IEnumerator PlaySELoop(AudioSource source, int loopCount)
    {
        int playedCount = 1;
        while (playedCount < loopCount)
        {
            yield return new WaitForSeconds(source.clip.length);
            if (!source.isPlaying)
                source.Play();

            playedCount++;
        }
    }

    public void SetBGMSpeed(float speed)
    {
        if (bgmSource.clip == null) return;
        bgmSource.pitch = speed;
        Debug.Log($"[SetBGMSpeed] BGM 속도 변경: {speed}");
    }


    public void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
            Debug.Log("[StopBGM] BGM 재생 중지");
        }
    }


    public void PlayBGM()
    {
        if (bgmSource.clip == null) return;
        if (!bgmSource.isPlaying)
            bgmSource.Play();
        Debug.Log("[StopBGM] BGM 다시 시작");
    }

    public AudioClip LoadAudioClipByName(string clipName, DialogSE targetSE)
    {
        if (string.IsNullOrEmpty(clipName) || clipName == "-1")
        {
            Debug.Log($"[LoadAudioClipByName] '{clipName}' → 효과음 끔 명령");
            if (targetSE != null)
                targetSE.stopSE = true; // -1 명령임 표시
            return null;
        }

        AudioClip clip = Resources.Load<AudioClip>($"Audio/{clipName}");
        if (clip == null)
            Debug.LogWarning($"AudioClip '{clipName}'를 Resources/Audio 폴더에서 찾을 수 없습니다.");
        return clip;
    }


    void Start()
    {
        bgmSource.pitch = 1f;  // 초기화 코드가 없으면 다른 값이 남아있을 수 있음
    }
    public void StopSE()
    {
        if (seSource1 != null) seSource1.Stop();
        if (seSource2 != null) seSource2.Stop();
    }

}
