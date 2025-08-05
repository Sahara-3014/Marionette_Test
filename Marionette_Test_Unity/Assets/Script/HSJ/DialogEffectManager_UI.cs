using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class DialogEffectManager_UI : MonoBehaviour
{

    public Color otherColor = Color.red;        // OtherColorEnable용
    public Color darkenColor = new Color(0.5f, 0.5f, 0.5f, 1f); // AllColorEnable용
    public float fadeDuration = 0.5f;

    public SpriteRenderer[] characters;

    /// <summary> 배경효과 </summary>
    public IEnumerator RunScreenEffect(Dialog_ScreenEffect effect, SpriteRenderer bg)
    {
        RectTransform rt = bg.GetComponent<RectTransform>();
        switch (effect)
        {
            case Dialog_ScreenEffect.ShakeVertical:
                yield return StartCoroutine(VerticalShakeScreen(rt));
                break;
            case Dialog_ScreenEffect.ShakeHorizontal:
                yield return StartCoroutine(HorizontalShakeScreen(rt));
                break;
            case Dialog_ScreenEffect.Shake:
                yield return StartCoroutine(RandomShakeScreen(rt));
                break;
            case Dialog_ScreenEffect.None:
                yield break;
            case Dialog_ScreenEffect.ClearAll:
                yield return StartCoroutine(ClearAll(bg));
                break;
            case Dialog_ScreenEffect.FadeOutAll:
                yield return StartCoroutine(FadeOutScreen(bg));
                break;
            case Dialog_ScreenEffect.OtherColorEnable:
                yield return StartCoroutine(ChangeBGColor(bg, otherColor));
                break;
            case Dialog_ScreenEffect.OtherColorDisable:
                yield return StartCoroutine(ChangeBGColor(bg, Color.white));
                break;
            case Dialog_ScreenEffect.AllColorEnable:
                yield return StartCoroutine(ChangeCharacterColor(darkenColor));
                break;
            case Dialog_ScreenEffect.AllColorDisable:
                yield return StartCoroutine(ChangeCharacterColor(Color.white));
                break;
            case Dialog_ScreenEffect.MoveUp:
                yield return StartCoroutine(MoveScreenUp(rt));
                break;
            case Dialog_ScreenEffect.MoveDown:
                yield return StartCoroutine(MoveScreenDown(rt));
                break;
            case Dialog_ScreenEffect.FadeIn:
                yield return StartCoroutine(FadeInScreen(bg));
                break;
            case Dialog_ScreenEffect.FadeOut:
                yield return StartCoroutine(FadeOutScreen(bg));
                break;
            case Dialog_ScreenEffect.ColorEnable:
                yield return StartCoroutine(EnableColorEffect(bg));
                break;
            case Dialog_ScreenEffect.ColorDisable:
                yield return StartCoroutine(DisableColorEffect(bg));
                break;

            default:
                yield break;
        }
    }




    
    public float distance = 50f;
    public float duration = 0.5f;
    public float jumpHeight = 10f;

    /// <summary> 캐릭터효과 </summary>
    public IEnumerator RunCharacterEffect(Dialog_CharEffect effect, Image character)
    {
        RectTransform rt = character.GetComponent<RectTransform>();
        switch (effect)
        {
            case Dialog_CharEffect.ShakeVertical:
                yield return StartCoroutine(VerticalShake(rt));
                break;
            case Dialog_CharEffect.ShakeHorizontal:
                yield return StartCoroutine(HorizontalShake(rt));
                break;
            case Dialog_CharEffect.Shake:
                yield return StartCoroutine(RandomShake(rt));
                break;
            case Dialog_CharEffect.Jump:
                yield return StartCoroutine(Jump(rt));
                break;
            case Dialog_CharEffect.MoveOut2Left:
                yield return StartCoroutine(MoveOut(rt, Vector3.left));
                break;
            case Dialog_CharEffect.MoveOut2Right:
                yield return StartCoroutine(MoveOut(rt, Vector3.right));
                break;
            case Dialog_CharEffect.MoveLeft2Out:
                yield return StartCoroutine(MoveFrom(rt, Vector3.left));
                break;
            case Dialog_CharEffect.MoveRight2Out:
                yield return StartCoroutine(MoveFrom(rt, Vector3.right));
                break;
            case Dialog_CharEffect.MoveVertical:
                yield return StartCoroutine(MoveUpDown(rt));
                break;
            case Dialog_CharEffect.MoveUp:
                yield return StartCoroutine(MoveDirection(rt, Vector3.up));
                break;
            case Dialog_CharEffect.MoveDown:
                yield return StartCoroutine(MoveDirection(rt, Vector3.down));
                break;
            case Dialog_CharEffect.FadeIn:
                yield return StartCoroutine(FadeInCharacter(character));
                break;
            case Dialog_CharEffect.FadeOut:
                yield return StartCoroutine(FadeOutCharacter(character));
                break;
            case Dialog_CharEffect.ColorEnable:
                character.color = Color.white;
                break;
            case Dialog_CharEffect.ColorDisable:
                character.color = Color.gray;
                break;
            case Dialog_CharEffect.None:
            default:
                yield break;
        }
    }

    #region 화면 효과
    private IEnumerator VerticalShakeScreen(RectTransform target)
    {
        //Vector3 originalPos = target.transform.localPosition;
        //for (int i = 0; i < 10; i++)
        //{
        //    target.localPosition = originalPos + new Vector3(0, Random.Range(-5f, 5f), 0);
        //    yield return new WaitForSeconds(0.02f);
        //}
        //target.transform.localPosition = originalPos;

        target.DOShakePosition(duration, new Vector3(0f, 5f));
        yield break;
    }

    private IEnumerator HorizontalShakeScreen(RectTransform target)
    {
        //Vector3 originalPos = target.transform.localPosition;
        //for (int i = 0; i < 10; i++)
        //{
        //    target.transform.localPosition = originalPos + new Vector3(Random.Range(-5f, 5f), 0, 0);
        //    yield return new WaitForSeconds(0.02f);
        //}
        //target.transform.localPosition = originalPos;

        target.DOShakePosition(duration, new Vector3(5f, 0f));
        yield break;
    }

    private IEnumerator RandomShakeScreen(RectTransform target)
    {
        //Vector3 originalPos = target.transform.localPosition;
        //for (int i = 0; i < 10; i++)
        //{
        //    target.transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * 5f;
        //    yield return new WaitForSeconds(0.02f);
        //}
        //target.transform.localPosition = originalPos;

        target.DOShakePosition(duration, (Vector3)Random.insideUnitCircle * 5f);
        yield break;
    }

    private IEnumerator FadeOutScreen(SpriteRenderer target)
    {
        //Color originalColor = target.color;
        //float timer = 0f;
        //while (timer < fadeDuration)
        //{
        //    float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
        //    target.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
        //    timer += Time.deltaTime;
        //    yield return null;
        //}
        //target.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        target.DOColor(new Color(target.color.r, target.color.g, target.color.b, 0f), fadeDuration);
        yield break;
    }

    private IEnumerator ClearAll(SpriteRenderer sr = null, Image img = null, RectTransform rt = null)
    {
        if(sr != null)
        {
            sr.DOKill();
            sr.sprite = null;
        }
        if (img != null)
        {
            img.DOKill();
            img.color = Color.white;
        }
        if(rt != null)
        {
            rt.DOComplete();
            rt.DOKill();
        }
        yield break;
    }

    private IEnumerator ChangeBGColor(SpriteRenderer bg, Color targetColor)
    {
        bg.DOColor(targetColor, fadeDuration);
        yield break;
    }

    private IEnumerator ChangeCharacterColor(Color color)
    {
        foreach (var sr in characters)
        {
            if (sr.CompareTag("Character"))
            {
                sr.color = color;
            }
        }
        yield break;
    }

    private IEnumerator MoveScreenUp(Transform target)
    {
        Vector3 originalPos = target.localPosition;
        Vector3 targetPos = originalPos + (Vector3.up * distance);

        //float timer = 0f;
        //while (timer < duration)
        //{
        //    target.transform.localPosition = Vector3.Lerp(originalPos, targetPos, timer / duration);
        //    timer += Time.deltaTime;
        //    yield return null;
        //}
        //target.transform.localPosition = targetPos;

        target.position = targetPos;
        target.DOMoveY(originalPos.y, duration);
        yield break;
    }

    private IEnumerator MoveScreenDown(Transform target)
    {
        Vector3 originalPos = target.localPosition;
        Vector3 targetPos = originalPos + Vector3.down * distance;

        //float timer = 0f;
        //while (timer < duration)
        //{
        //    target.transform.localPosition = Vector3.Lerp(originalPos, targetPos, timer / duration);
        //    timer += Time.deltaTime;
        //    yield return null;
        //}
        //target.transform.localPosition = targetPos;

        target.position = targetPos;
        target.DOMoveY(originalPos.y, duration);
        yield break;
    }

    private IEnumerator FadeInScreen(SpriteRenderer target)
    {
        //Color color = target.color;
        //float timer = 0f;
        //while (timer < fadeDuration)
        //{
        //    float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
        //    target.color = new Color(color.r, color.g, color.b, alpha);
        //    timer += Time.deltaTime;
        //    yield return null;
        //}
        //target.color = new Color(color.r, color.g, color.b, 1f);

        target.color = new Color(target.color.r, target.color.g, target.color.b, 0f);
        target.DOFade(1f, fadeDuration);
        yield break;
    }

    private IEnumerator EnableColorEffect(SpriteRenderer target)
    {
        //// 예: tint 색을 빨간색으로 바꾸기 (임의 설정)
        //Color effectColor = new Color(1f, 0.5f, 0.5f, 1f);
        //target.color = effectColor;

        target.DOColor(Color.white, fadeDuration);
        yield break;
    }

    private IEnumerator DisableColorEffect(SpriteRenderer target)
    {
        //target.color = Color.white;
        target.DOColor(Color.gray, fadeDuration);
        yield break;
    }
    #endregion


    #region 캐릭터 관련
    private IEnumerator Jump(RectTransform target)
    {
        Vector3 originalPos = target.localPosition;
        //float timer = 0f;
        //while (timer < duration)
        //{
        //    float y = Mathf.Sin((timer / duration) * Mathf.PI) * jumpHeight;
        //    target.transform.localPosition = originalPos + new Vector3(0, y, 0);
        //    timer += Time.deltaTime;
        //    yield return null;
        //}
        //target.transform.localPosition = originalPos;

        target.DOLocalJump(originalPos, jumpHeight, 1, duration, true);
        yield break;
    }

    private IEnumerator VerticalShake(RectTransform target)
    {
        Vector3 originalPos = target.localPosition;
        //for (int i = 0; i < 10; i++)
        //{
        //    target.transform.localPosition = originalPos + new Vector3(0, Random.Range(-5f, 5f), 0);
        //    yield return new WaitForSeconds(0.02f);
        //}
        //target.transform.localPosition = originalPos;

        target.DOShakeAnchorPos(duration, Vector3.up * 5f, snapping: true);
        yield break;
    }

    private IEnumerator HorizontalShake(RectTransform target)
    {
        Vector3 originalPos = target.localPosition;
        //for (int i = 0; i < 10; i++)
        //{
        //    target.transform.localPosition = originalPos + new Vector3(Random.Range(-5f, 5f), 0, 0);
        //    yield return new WaitForSeconds(0.02f);
        //}
        //target.transform.localPosition = originalPos;

        target.DOShakeAnchorPos(duration, Vector2.right * 5f, snapping: true);
        yield break;
    }

    private IEnumerator RandomShake(RectTransform target)
    {
        Vector3 originalPos = target.localPosition;
        //for (int i = 0; i < 10; i++)
        //{
        //    target.transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * 5f;
        //    yield return new WaitForSeconds(0.02f);
        //}
        //target.transform.localPosition = originalPos;

        target.DOShakePosition(duration, Vector2.one * 5f);
        yield break;
    }

    private IEnumerator MoveOut(RectTransform target, Vector3 direction)
    {
        //Vector3 startPos = target.transform.localPosition;
        //Vector3 endPos = startPos + direction * distance;
        //float timer = 0f;
        //while (timer < duration)
        //{
        //    target.transform.localPosition = Vector3.Lerp(startPos, endPos, timer / duration);
        //    timer += Time.deltaTime;
        //    yield return null;
        //}
        //target.transform.localPosition = endPos;

        Vector3 ori = target.position;
        target.DOMove(Vector3.zero, duration).OnComplete(() => target.gameObject.SetActive(false));
        yield break;
    }

    private IEnumerator MoveFrom(RectTransform target, Vector3 fromDirection)
    {
        //Vector3 endPos = target.transform.localPosition;
        //Vector3 startPos = endPos + fromDirection * distance;
        //target.transform.localPosition = startPos;

        //float timer = 0f;
        //while (timer < duration)
        //{
        //    target.transform.localPosition = Vector3.Lerp(startPos, endPos, timer / duration);
        //    timer += Time.deltaTime;
        //    yield return null;
        //}
        //target.transform.localPosition = endPos;

        target.gameObject.SetActive(true);
        target.DOMove(fromDirection, duration);
        yield break;
    }

    private IEnumerator MoveUpDown(RectTransform target)
    {
        //Vector3 originalPos = target.transform.localPosition;
        //Vector3 targetPos = originalPos + Vector3.up * 50f;

        //float timer = 0f;
        //while (timer < duration)
        //{
        //    target.transform.localPosition = Vector3.Lerp(originalPos, targetPos, timer / duration);
        //    timer += Time.deltaTime;
        //    yield return null;
        //}
        //timer = 0f;
        //while (timer < duration)
        //{
        //    target.transform.localPosition = Vector3.Lerp(targetPos, originalPos, timer / duration);
        //    timer += Time.deltaTime;
        //    yield return null;
        //}
        //target.transform.localPosition = originalPos;

        float _y = target.localPosition.y;
        target.DOMoveY(distance / 2f, duration).OnComplete(()=> target.DOMoveY(_y, duration));
        yield break;
    }

    private IEnumerator MoveDirection(RectTransform target, Vector3 direction)
    {
        //Vector3 startPos = target.transform.localPosition;
        //Vector3 endPos = startPos + direction * distance;

        //float timer = 0f;
        //while (timer < duration)
        //{
        //    target.transform.localPosition = Vector3.Lerp(startPos, endPos, timer / duration);
        //    timer += Time.deltaTime;
        //    yield return null;
        //}
        //target.transform.localPosition = endPos;

        target.DOMove(direction, duration);
        yield break;
    }

    private IEnumerator FadeInCharacter(Image target)
    {
        //Color color = target.color;
        //float timer = 0f;
        //while (timer < fadeDuration)
        //{
        //    float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
        //    target.color = new Color(color.r, color.g, color.b, alpha);
        //    timer += Time.deltaTime;
        //    yield return null;
        //}
        //target.color = new Color(color.r, color.g, color.b, 1f);

        target.color = new Color(target.color.r, target.color.g, target.color.b, 0f);
        target.DOFade(1f, fadeDuration);
        yield break;
    }

    private IEnumerator FadeOutCharacter(Image target)
    {
        //Color color = target.color;
        //float timer = 0f;
        //while (timer < fadeDuration)
        //{
        //    float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
        //    target.color = new Color(color.r, color.g, color.b, alpha);
        //    timer += Time.deltaTime;
        //    yield return null;
        //}
        //target.color = new Color(color.r, color.g, color.b, 0f);

        target.DOFade(0f, fadeDuration);
        yield break;
    }
    #endregion

    
    private List<GameObject> activeEffects = new List<GameObject>();

    /// <summary> 모든 이펙트 정리 </summary>
    public void ClearEffects()
    {
        foreach (var effect in activeEffects)
        {
            if (effect != null)
                Destroy(effect);
        }
        activeEffects.Clear();
    }

}
