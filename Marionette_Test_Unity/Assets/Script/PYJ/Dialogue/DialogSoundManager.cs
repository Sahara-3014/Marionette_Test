using UnityEngine;
using System.Collections;

public class DialogSoundManager : MonoBehaviour
{
    public static DialogSoundManager Instance;

    public AudioSource bgmSource;
    public AudioSource seSource1;
    public AudioSource seSource2;
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

    public void PlayBGM(DialogSE bgm)
    {
        if (bgmSource == null)
        {
            Debug.LogError("[PlayBGM] bgmSource가 할당되어 있지 않습니다!");
            return;
        }

        if (bgm == null || bgm.clip == null)
        {
            Debug.LogWarning("[PlayBGM] 재생할 BGM이 없거나 AudioClip이 null입니다.");
            return;
        }

        bgmSource.clip = bgm.clip;
        bgmSource.volume = bgm.volume;
        bgmSource.loop = (bgm.loopCount == 0);
        bgmSource.Play();

        Debug.Log($"[PlayBGM] BGM '{bgm.clip.name}' 재생 시작 (볼륨: {bgm.volume})");
    }


    public void PlaySE(DialogSE se)
    {
        if (se == null)
        {
            Debug.LogError("[PlaySE] DialogSE가 null입니다.");
            return;
        }
        if (se.clip == null)
        {
            Debug.LogError("[PlaySE] DialogSE.clip이 null입니다.");
            return;
        }
        if (seSource1 == null || seSource2 == null)
        {
            Debug.LogError($"[PlaySE] AudioSource 미할당! seSource1: {seSource1}, seSource2: {seSource2}");
            return;
        }

        AudioSource sourceToUse = null;

        if (!seSource1.isPlaying)
            sourceToUse = seSource1;
        else if (!seSource2.isPlaying)
            sourceToUse = seSource2;
        else
        {
            Debug.Log("[PlaySE] 둘 다 재생 중, seSource1 강제 사용");
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
}
