using UnityEngine;
using System.Collections;

public class DialogSoundManager : MonoBehaviour
{
    public AudioSource bgmSource;
    public AudioSource seSource1;
    public AudioSource seSource2;

    public void PlayDialogSE(DialogSE dialogSE)
    {
        if (dialogSE == null || dialogSE.clip == null) return;

        switch (dialogSE.type)
        {
            case SEType.BGM:
                PlayBGM(dialogSE);
                break;

            case SEType.SE:
                PlaySE(dialogSE);
                break;
        }
    }

    public void PlayBGM(DialogSE bgm)
    {
        bgmSource.clip = bgm.clip;
        bgmSource.volume = bgm.volume;
        bgmSource.loop = (bgm.loopCount == 0);
        bgmSource.Play();
    }

    public void PlaySE(DialogSE se)
    {
        AudioSource source = seSource1;

        if (!seSource1.isPlaying)
            source = seSource1;
        else if (!seSource2.isPlaying)
            source = seSource2;
        else
        {
            seSource1.Stop();
            source = seSource1;
        }

        source.clip = se.clip;
        source.volume = se.volume;
        source.loop = (se.loopCount == 0);
        source.Play();

        if (se.loopCount > 0)
            StartCoroutine(PlaySELoop(source, se.loopCount));
    }

    private IEnumerator PlaySELoop(AudioSource source, int loopCount)
    {
        int count = 1;
        while (count < loopCount)
        {
            yield return new WaitForSeconds(source.clip.length);
            source.Play();
            count++;
        }
    }

}
