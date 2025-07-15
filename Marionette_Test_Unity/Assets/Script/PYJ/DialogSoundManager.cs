using UnityEngine;
using System.Collections;

public class DialogSoundManager : MonoBehaviour
{
    public AudioSource bgmSource;
    public AudioSource seSource1;
    public AudioSource seSource2;

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
            Debug.LogError("[PlayBGM] bgmSource�� �Ҵ�Ǿ� ���� �ʽ��ϴ�!");
            return;
        }

        if (bgm == null || bgm.clip == null)
        {
            Debug.LogWarning("[PlayBGM] ����� BGM�� ���ų� AudioClip�� null�Դϴ�.");
            return;
        }

        bgmSource.clip = bgm.clip;
        bgmSource.volume = bgm.volume;
        bgmSource.loop = (bgm.loopCount == 0);
        bgmSource.Play();

        Debug.Log($"[PlayBGM] BGM '{bgm.clip.name}' ��� ���� (����: {bgm.volume})");
    }


    public void PlaySE(DialogSE se)
    {


        AudioSource sourceToUse = null;

        if (!seSource1.isPlaying)
            sourceToUse = seSource1;
        else if (!seSource2.isPlaying)
            sourceToUse = seSource2;
        else
        {
            Debug.Log("[PlaySE] �� �� ��� ���̾ seSource1 ������ ���");
            seSource1.Stop();
            sourceToUse = seSource1;
        }

        sourceToUse.clip = se.clip;
        sourceToUse.volume = se.volume;
        sourceToUse.loop = (se.loopCount == 0);
        sourceToUse.Play();

        Debug.Log($"[PlaySE] ȿ���� '{se.clip.name}' ��� ���� (����: {se.volume})");

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
