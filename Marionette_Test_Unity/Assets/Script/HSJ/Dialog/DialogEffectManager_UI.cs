using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogEffectManager_UI : MonoBehaviour
{

    public Color otherColor = Color.red;        // OtherColorEnable용
    public Color darkenColor = new Color(0.5f, 0.5f, 0.5f, 1f); // AllColorEnable용
    public float fadeDuration = 0.5f;

    public SpriteRenderer[] characters;

    /// <summary> 배경효과 </summary>
    public IEnumerator RunScreenEffect(Dialog_ScreenEffect effect, SpriteRenderer bg, UnityAction callback = null)
    {
        Debug.Log("RunScreenEffect: " + effect);
        RectTransform rt = bg.GetComponent<RectTransform>();
        switch (effect)
        {
            case Dialog_ScreenEffect.ShakeVertical:
                yield return StartCoroutine(VerticalShakeScreen(rt, callback));
                break;
            case Dialog_ScreenEffect.ShakeHorizontal:
                yield return StartCoroutine(HorizontalShakeScreen(rt, callback));
                break;
            case Dialog_ScreenEffect.Shake:
                yield return StartCoroutine(RandomShakeScreen(rt, callback));
                break;
            case Dialog_ScreenEffect.ClearAll:
                yield return StartCoroutine(ClearAll(bg, callback: callback));
                break;
            case Dialog_ScreenEffect.FadeOutAll:
                yield return StartCoroutine(FadeOutScreen(bg, callback));
                break;
            case Dialog_ScreenEffect.OtherColorEnable:
                yield return StartCoroutine(ChangeBGColor(bg, otherColor, callback));
                break;
            case Dialog_ScreenEffect.OtherColorDisable:
                yield return StartCoroutine(ChangeBGColor(bg, Color.white, callback));
                break;
            case Dialog_ScreenEffect.AllColorEnable:
                yield return StartCoroutine(ChangeCharacterColor(darkenColor, callback));
                break;
            case Dialog_ScreenEffect.AllColorDisable:
                yield return StartCoroutine(ChangeCharacterColor(Color.white, callback));
                break;
            case Dialog_ScreenEffect.MoveUp:
                yield return StartCoroutine(MoveScreenUp(rt, callback));
                break;
            case Dialog_ScreenEffect.MoveDown:
                yield return StartCoroutine(MoveScreenDown(rt, callback));
                break;
            case Dialog_ScreenEffect.FadeIn:
                yield return StartCoroutine(FadeInScreen(bg, callback));
                break;
            case Dialog_ScreenEffect.FadeOut:
                yield return StartCoroutine(FadeOutScreen(bg, callback));
                break;
            case Dialog_ScreenEffect.ColorEnable:
                yield return StartCoroutine(EnableColorEffect(bg, callback));
                break;
            case Dialog_ScreenEffect.ColorDisable:
                yield return StartCoroutine(DisableColorEffect(bg, callback));
                break;
            case Dialog_ScreenEffect.None:
            default:
                callback?.Invoke();
                yield break;
        }
    }




    
    public float distance = 50f;
    public float duration = 0.5f;
    public float jumpHeight = 10f;

    /// <summary> 캐릭터효과 </summary>
    public IEnumerator RunCharacterEffect(Dialog_CharEffect effect, Image character, UnityAction callback = null)
    {
        Debug.Log("RunCharacterEffect: " + effect);
        RectTransform rt = character.GetComponent<RectTransform>();
        switch (effect)
        {
            case Dialog_CharEffect.ShakeVertical:
                yield return StartCoroutine(VerticalShake(rt, callback));
                break;
            case Dialog_CharEffect.ShakeHorizontal:
                yield return StartCoroutine(HorizontalShake(rt, callback));
                break;
            case Dialog_CharEffect.Shake:
                yield return StartCoroutine(RandomShake(rt, callback));
                break;
            case Dialog_CharEffect.Jump:
                yield return StartCoroutine(Jump(rt, callback));
                break;
            case Dialog_CharEffect.MoveOut2Left:
                yield return StartCoroutine(MoveOut(rt, Vector3.left, callback));
                break;
            case Dialog_CharEffect.MoveOut2Right:
                yield return StartCoroutine(MoveOut(rt, Vector3.right, callback));
                break;
            case Dialog_CharEffect.MoveLeft2Out:
                yield return StartCoroutine(MoveFrom(rt, Vector3.left, callback));
                break;
            case Dialog_CharEffect.MoveRight2Out:
                yield return StartCoroutine(MoveFrom(rt, Vector3.right, callback));
                break;
            case Dialog_CharEffect.MoveVertical:
                yield return StartCoroutine(MoveUpDown(rt, callback));
                break;
            case Dialog_CharEffect.MoveUp:
                yield return StartCoroutine(MoveDirection(rt, Vector3.up, callback));
                break;
            case Dialog_CharEffect.MoveDown:
                yield return StartCoroutine(MoveDirection(rt, Vector3.down, callback));
                break;
            case Dialog_CharEffect.FadeIn:
                yield return StartCoroutine(FadeInCharacter(character, callback));
                break;
            case Dialog_CharEffect.FadeOut:
                yield return StartCoroutine(FadeOutCharacter(character, callback));
                break;
            case Dialog_CharEffect.ColorEnable:
                yield return StartCoroutine(EnableColorEffect(character, callback));
                break;
            case Dialog_CharEffect.ColorDisable:
                yield return StartCoroutine(DisableColorEffect(character, callback));
                break;
            case Dialog_CharEffect.None:
            default:
                callback?.Invoke();
                yield break;
        }
    }

    #region 화면 효과
    private IEnumerator VerticalShakeScreen(RectTransform target, UnityAction callback = null)
    {
        //Vector3 originalPos = target.transform.localPosition;
        //for (int i = 0; i < 10; i++)
        //{
        //    target.localPosition = originalPos + new Vector3(0, Random.Range(-5f, 5f), 0);
        //    yield return new WaitForSeconds(0.02f);
        //}
        //target.transform.localPosition = originalPos;

        target.DOShakePosition(duration, new Vector3(0f, 5f)).OnComplete(()=>callback?.Invoke());
        yield break;
    }

    private IEnumerator HorizontalShakeScreen(RectTransform target, UnityAction callback = null)
    {
        //Vector3 originalPos = target.transform.localPosition;
        //for (int i = 0; i < 10; i++)
        //{
        //    target.transform.localPosition = originalPos + new Vector3(Random.Range(-5f, 5f), 0, 0);
        //    yield return new WaitForSeconds(0.02f);
        //}
        //target.transform.localPosition = originalPos;

        target.DOShakePosition(duration, new Vector3(5f, 0f)).OnComplete(()=>callback?.Invoke());
        yield break;
    }

    private IEnumerator RandomShakeScreen(RectTransform target, UnityAction callback = null)
    {
        //Vector3 originalPos = target.transform.localPosition;
        //for (int i = 0; i < 10; i++)
        //{
        //    target.transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * 5f;
        //    yield return new WaitForSeconds(0.02f);
        //}
        //target.transform.localPosition = originalPos;

        target.DOShakePosition(duration, (Vector3)Random.insideUnitCircle * 5f).OnComplete(()=>callback?.Invoke());
        yield break;
    }

    private IEnumerator FadeOutScreen(SpriteRenderer target, UnityAction callback = null)
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

        target.DOColor(new Color(target.color.r, target.color.g, target.color.b, 0f), fadeDuration).OnComplete(()=>callback?.Invoke());
        yield break;
    }

    private IEnumerator ClearAll(SpriteRenderer sr = null, Image img = null, RectTransform rt = null, UnityAction callback = null)
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
        callback?.Invoke();
        yield break;
    }

    private IEnumerator ChangeBGColor(SpriteRenderer bg, Color targetColor, UnityAction callback = null)
    {
        bg.DOColor(targetColor, fadeDuration).OnComplete(()=>callback?.Invoke());
        yield break;
    }

    private IEnumerator ChangeCharacterColor(Color color, UnityAction callback = null)
    {
        foreach (var sr in characters)
        {
            if (sr.CompareTag("Character"))
            {
                sr.color = color;
            }
        }
        callback?.Invoke();
        yield break;
    }

    private IEnumerator MoveScreenUp(Transform target, UnityAction callback = null)
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
        target.DOMoveY(originalPos.y, duration).OnComplete(() => callback?.Invoke());
        yield break;
    }

    private IEnumerator MoveScreenDown(Transform target, UnityAction callback = null)
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
        target.DOMoveY(originalPos.y, duration).OnComplete(()=>callback?.Invoke());
        yield break;
    }

    private IEnumerator FadeInScreen(SpriteRenderer target, UnityAction callback = null)
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
        //target.DOFade(1f, fadeDuration).OnComplete(() => callback?.Invoke());
        yield break;
    }

    private IEnumerator EnableColorEffect(SpriteRenderer target, UnityAction callback = null)
    {
        //// 예: tint 색을 빨간색으로 바꾸기 (임의 설정)
        //Color effectColor = new Color(1f, 0.5f, 0.5f, 1f);
        //target.color = effectColor;

        target.DOColor(Color.white, fadeDuration).OnComplete(()=>callback?.Invoke());
        yield break;
    }

    private IEnumerator DisableColorEffect(SpriteRenderer target, UnityAction callback = null)
    {
        //target.color = Color.white;
        target.DOColor(Color.gray, fadeDuration).OnComplete(() => callback?.Invoke());
        yield break;
    }
    #endregion


    #region 캐릭터 관련
    private IEnumerator Jump(RectTransform target, UnityAction callback = null)
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

        target.DOLocalJump(originalPos, jumpHeight, 1, duration, true).OnComplete(()=>callback?.Invoke());
        yield break;
    }

    private IEnumerator VerticalShake(RectTransform target, UnityAction callback = null)
    {
        Vector3 originalPos = target.localPosition;
        //for (int i = 0; i < 10; i++)
        //{
        //    target.transform.localPosition = originalPos + new Vector3(0, Random.Range(-5f, 5f), 0);
        //    yield return new WaitForSeconds(0.02f);
        //}
        //target.transform.localPosition = originalPos;

        target.DOShakeAnchorPos(duration, Vector3.up * 5f, snapping: true).OnComplete(()=>callback?.Invoke());
        yield break;
    }

    private IEnumerator HorizontalShake(RectTransform target, UnityAction callback = null)
    {
        Vector3 originalPos = target.localPosition;
        //for (int i = 0; i < 10; i++)
        //{
        //    target.transform.localPosition = originalPos + new Vector3(Random.Range(-5f, 5f), 0, 0);
        //    yield return new WaitForSeconds(0.02f);
        //}
        //target.transform.localPosition = originalPos;

        target.DOShakeAnchorPos(duration, Vector2.right * 5f, snapping: true).OnComplete(()=>callback?.Invoke());
        yield break;
    }

    private IEnumerator RandomShake(RectTransform target, UnityAction callback = null)
    {
        Vector3 originalPos = target.localPosition;
        //for (int i = 0; i < 10; i++)
        //{
        //    target.transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * 5f;
        //    yield return new WaitForSeconds(0.02f);
        //}
        //target.transform.localPosition = originalPos;

        target.DOShakePosition(duration, Vector2.one * 5f).OnComplete(() => callback?.Invoke());
        yield break;
    }

    private IEnumerator MoveOut(RectTransform target, Vector3 direction, UnityAction callback = null)
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
        target.DOMove(Vector3.zero, duration).OnComplete(() => target.gameObject.SetActive(false)).OnComplete(()=>callback?.Invoke());
        yield break;
    }

    private IEnumerator MoveFrom(RectTransform target, Vector3 fromDirection, UnityAction callback = null)
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
        target.DOMove(fromDirection, duration).OnComplete(()=>callback?.Invoke());
        yield break;
    }

    private IEnumerator MoveUpDown(RectTransform target, UnityAction callback = null)
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
        target.DOMoveY(distance / 2f, duration).OnComplete(()=> target.DOMoveY(_y, duration)).OnComplete(()=>callback?.Invoke());
        yield break;
    }

    private IEnumerator MoveDirection(RectTransform target, Vector3 direction, UnityAction callback = null)
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

        target.DOMove(direction, duration).OnComplete(()=>callback?.Invoke());
        yield break;
    }

    private IEnumerator FadeInCharacter(Image target, UnityAction callback = null)
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
        target.DOFade(1f, fadeDuration).OnComplete(()=>callback?.Invoke());
        yield break;
    }

    private IEnumerator FadeOutCharacter(Image target, UnityAction callback = null)
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

        target.DOFade(0f, fadeDuration).OnComplete(()=>callback?.Invoke());
        yield break;
    }

    private IEnumerator EnableColorEffect(Image target, UnityAction callback = null)
    {
        //// 예: tint 색을 빨간색으로 바꾸기 (임의 설정)
        //Color effectColor = new Color(1f, 0.5f, 0.5f, 1f);
        //target.color = effectColor;

        target.DOColor(Color.white, fadeDuration).OnComplete(()=>callback?.Invoke());
        yield break;
    }

    private IEnumerator DisableColorEffect(Image target, UnityAction callback = null)
    {
        //target.color = Color.white;
        target.DOColor(Color.gray, fadeDuration).OnComplete(()=>callback?.Invoke());
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
