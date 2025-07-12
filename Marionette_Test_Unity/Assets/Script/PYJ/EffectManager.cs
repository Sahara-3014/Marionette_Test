using System.Collections;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public IEnumerator RunScreenEffect(Dialog_ScreenEffect effect, SpriteRenderer bg)
    {
        switch (effect)
        {
            case Dialog_ScreenEffect.Shake:
                yield return StartCoroutine(ScreenShake(bg));
                break;
            case Dialog_ScreenEffect.FadeOutAll:
                yield return StartCoroutine(FadeOutScreen(bg));
                break;
            default:
                yield break;
        }
    }

    public IEnumerator RunCharacterEffect(Dialog_CharEffect effect, SpriteRenderer character)
    {
        switch (effect)
        {
            case Dialog_CharEffect.Shake:
                yield return StartCoroutine(CharacterShake(character));
                break;
            case Dialog_CharEffect.FadeIn:
                yield return StartCoroutine(FadeInCharacter(character));
                break;
            case Dialog_CharEffect.FadeOut:
                yield return StartCoroutine(FadeOutCharacter(character));
                break;
            default:
                yield break;
        }
    }

    private IEnumerator ScreenShake(SpriteRenderer target)
    {
        Vector3 originalPos = target.transform.localPosition;
        for (int i = 0; i < 10; i++)
        {
            target.transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * 5f;
            yield return new WaitForSeconds(0.02f);
        }
        target.transform.localPosition = originalPos;
    }

    private IEnumerator FadeOutScreen(SpriteRenderer target)
    {
        float time = 0;
        Color c = target.color;
        while (time < 1f)
        {
            time += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, time);
            target.color = c;
            yield return null;
        }
    }

    private IEnumerator CharacterShake(SpriteRenderer target)
    {
        Vector3 originalPos = target.transform.localPosition;
        for (int i = 0; i < 10; i++)
        {
            target.transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * 3f;
            yield return new WaitForSeconds(0.02f);
        }
        target.transform.localPosition = originalPos;
    }

    private IEnumerator FadeInCharacter(SpriteRenderer target)
    {
        float time = 0;
        Color c = target.color;
        c.a = 0;
        target.color = c;

        while (time < 1f)
        {
            time += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, time);
            target.color = c;
            yield return null;
        }
    }

    private IEnumerator FadeOutCharacter(SpriteRenderer target)
    {
        float time = 0;
        Color c = target.color;
        c.a = 1f;
        target.color = c;

        while (time < 1f)
        {
            time += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, time);
            target.color = c;
            yield return null;
        }
    }
}
