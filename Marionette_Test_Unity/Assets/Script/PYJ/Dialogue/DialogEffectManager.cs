using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogEffectManager : MonoBehaviour
{

    public Color otherColor = Color.red;        // OtherColorEnable용
    public Color darkenColor = new Color(0.5f, 0.5f, 0.5f, 1f); // AllColorEnable용
    public float fadeDuration = 0.5f;


    //
    //배경효과
    //
    public IEnumerator RunScreenEffect(Dialog_ScreenEffect effect, SpriteRenderer bg)
    {
        switch (effect)
        {
            case Dialog_ScreenEffect.ShakeVertical:
                yield return StartCoroutine(VerticalShakeScreen(bg));
                break;
            case Dialog_ScreenEffect.ShakeHorizontal:
                yield return StartCoroutine(HorizontalShakeScreen(bg));
                break;
            case Dialog_ScreenEffect.Shake:
                yield return StartCoroutine(RandomShakeScreen(bg));
                break;
            case Dialog_ScreenEffect.None:
                // 배경 이미지 투명도 기본값으로 복구
                bg.color = new Color(1f, 1f, 1f, 1f);
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
                yield return StartCoroutine(MoveScreenUp(bg));
                break;
            case Dialog_ScreenEffect.MoveDown:
                yield return StartCoroutine(MoveScreenDown(bg));
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




    //
    //캐릭터효과
    //
    public float moveDistance = 100f;
    public float moveDuration = 0.5f;
    public float jumpHeight = 30f;
    public IEnumerator RunCharacterEffect(Dialog_CharEffect effect, SpriteRenderer character)
    {

        switch (effect)
        {
            
            case Dialog_CharEffect.ShakeVertical:
                yield return StartCoroutine(VerticalShake(character));
                break;
            case Dialog_CharEffect.ShakeHorizontal:
                yield return StartCoroutine(HorizontalShake(character));
                break;
            case Dialog_CharEffect.Shake:
                yield return StartCoroutine(RandomShake(character));
                break;
            case Dialog_CharEffect.Jump:
                yield return StartCoroutine(Jump(character));
                break;
            case Dialog_CharEffect.MoveOut2Left:
                yield return StartCoroutine(MoveOut(character, Vector3.left));
                break;
            case Dialog_CharEffect.MoveOut2Right:
                yield return StartCoroutine(MoveOut(character, Vector3.right));
                break;
            case Dialog_CharEffect.MoveLeft2Out:
                yield return StartCoroutine(MoveFrom(character, Vector3.left));
                break;
            case Dialog_CharEffect.MoveRight2Out:
                yield return StartCoroutine(MoveFrom(character, Vector3.right));
                break;
            case Dialog_CharEffect.MoveVertical:
                yield return StartCoroutine(MoveUpDown(character));
                break;
            case Dialog_CharEffect.MoveUp:
                yield return StartCoroutine(MoveDirection(character, Vector3.up));
                break;
            case Dialog_CharEffect.MoveDown:
                yield return StartCoroutine(MoveDirection(character, Vector3.down));
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


    private IEnumerator VerticalShakeScreen(SpriteRenderer target)
    {
        Vector3 originalPos = target.transform.localPosition;
        for (int i = 0; i < 10; i++)
        {
            target.transform.localPosition = originalPos + new Vector3(0, Random.Range(-5f, 5f), 0);
            yield return new WaitForSeconds(0.02f);
        }
        target.transform.localPosition = originalPos;
    }

    private IEnumerator HorizontalShakeScreen(SpriteRenderer target)
    {
        Vector3 originalPos = target.transform.localPosition;
        for (int i = 0; i < 10; i++)
        {
            target.transform.localPosition = originalPos + new Vector3(Random.Range(-5f, 5f), 0, 0);
            yield return new WaitForSeconds(0.02f);
        }
        target.transform.localPosition = originalPos;
    }

    private IEnumerator RandomShakeScreen(SpriteRenderer target)
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
        Color originalColor = target.color;
        float timer = 0f;
        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            target.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }
        target.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
    }

    private IEnumerator ClearAll(SpriteRenderer target)
    {
        target.sprite = null;
        yield break;
    }

    private IEnumerator ChangeBGColor(SpriteRenderer bg, Color targetColor)
    {
        bg.color = targetColor;
        yield break;
    }

    private IEnumerator ChangeCharacterColor(Color color)
    {
        SpriteRenderer[] characters = GameObject.FindObjectsByType<SpriteRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var sr in characters)
        {
            if (sr.CompareTag("Character"))
            {
                sr.color = color;
            }
        }
        yield break;
    }





    //
    //캐릭터 관련
    //
    private IEnumerator Jump(SpriteRenderer target)
    {
        Vector3 originalPos = target.transform.localPosition;
        float timer = 0f;
        while (timer < moveDuration)
        {
            float y = Mathf.Sin((timer / moveDuration) * Mathf.PI) * jumpHeight;
            target.transform.localPosition = originalPos + new Vector3(0, y, 0);
            timer += Time.deltaTime;
            yield return null;
        }
        target.transform.localPosition = originalPos;
    }

    private IEnumerator VerticalShake(SpriteRenderer target)
    {
        Vector3 originalPos = target.transform.localPosition;
        for (int i = 0; i < 10; i++)
        {
            target.transform.localPosition = originalPos + new Vector3(0, Random.Range(-5f, 5f), 0);
            yield return new WaitForSeconds(0.02f);
        }
        target.transform.localPosition = originalPos;
    }

    private IEnumerator HorizontalShake(SpriteRenderer target)
    {
        Vector3 originalPos = target.transform.localPosition;
        for (int i = 0; i < 10; i++)
        {
            target.transform.localPosition = originalPos + new Vector3(Random.Range(-5f, 5f), 0, 0);
            yield return new WaitForSeconds(0.02f);
        }
        target.transform.localPosition = originalPos;
    }

    private IEnumerator RandomShake(SpriteRenderer target)
    {
        Vector3 originalPos = target.transform.localPosition;
        for (int i = 0; i < 10; i++)
        {
            target.transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * 5f;
            yield return new WaitForSeconds(0.02f);
        }
        target.transform.localPosition = originalPos;
    }

    private IEnumerator MoveOut(SpriteRenderer target, Vector3 direction)
    {
        Vector3 startPos = target.transform.localPosition;
        Vector3 endPos = startPos + direction * moveDistance;
        float timer = 0f;
        while (timer < moveDuration)
        {
            target.transform.localPosition = Vector3.Lerp(startPos, endPos, timer / moveDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        target.transform.localPosition = endPos;
    }

    private IEnumerator MoveFrom(SpriteRenderer target, Vector3 fromDirection)
    {
        Vector3 endPos = target.transform.localPosition;
        Vector3 startPos = endPos + fromDirection * moveDistance;
        target.transform.localPosition = startPos;

        float timer = 0f;
        while (timer < moveDuration)
        {
            target.transform.localPosition = Vector3.Lerp(startPos, endPos, timer / moveDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        target.transform.localPosition = endPos;
    }

    private IEnumerator MoveUpDown(SpriteRenderer target)
    {
        Vector3 originalPos = target.transform.localPosition;
        Vector3 targetPos = originalPos + Vector3.up * 50f;

        float timer = 0f;
        while (timer < moveDuration)
        {
            target.transform.localPosition = Vector3.Lerp(originalPos, targetPos, timer / moveDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        timer = 0f;
        while (timer < moveDuration)
        {
            target.transform.localPosition = Vector3.Lerp(targetPos, originalPos, timer / moveDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        target.transform.localPosition = originalPos;
    }

    private IEnumerator MoveDirection(SpriteRenderer target, Vector3 direction)
    {
        Vector3 startPos = target.transform.localPosition;
        Vector3 endPos = startPos + direction * moveDistance;

        float timer = 0f;
        while (timer < moveDuration)
        {
            target.transform.localPosition = Vector3.Lerp(startPos, endPos, timer / moveDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        target.transform.localPosition = endPos;
    }

    private IEnumerator FadeInCharacter(SpriteRenderer target)
    {
        Color color = target.color;
        float timer = 0f;
        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            target.color = new Color(color.r, color.g, color.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }
        target.color = new Color(color.r, color.g, color.b, 1f);
    }

    private IEnumerator FadeOutCharacter(SpriteRenderer target)
    {
        Color color = target.color;
        float timer = 0f;
        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            target.color = new Color(color.r, color.g, color.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }
        target.color = new Color(color.r, color.g, color.b, 0f);
    }

    private IEnumerator MoveScreenUp(SpriteRenderer target)
    {
        Vector3 originalPos = target.transform.localPosition;
        Vector3 targetPos = originalPos + Vector3.up * moveDistance;

        float timer = 0f;
        while (timer < moveDuration)
        {
            target.transform.localPosition = Vector3.Lerp(originalPos, targetPos, timer / moveDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        target.transform.localPosition = targetPos;
    }

    private IEnumerator MoveScreenDown(SpriteRenderer target)
    {
        Vector3 originalPos = target.transform.localPosition;
        Vector3 targetPos = originalPos + Vector3.down * moveDistance;

        float timer = 0f;
        while (timer < moveDuration)
        {
            target.transform.localPosition = Vector3.Lerp(originalPos, targetPos, timer / moveDuration);
            timer += Time.deltaTime;
            yield return null;
        }
        target.transform.localPosition = targetPos;
    }

    private IEnumerator FadeInScreen(SpriteRenderer target)
    {
        Color color = target.color;
        float timer = 0f;
        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            target.color = new Color(color.r, color.g, color.b, alpha);
            timer += Time.deltaTime;
            yield return null;
        }
        target.color = new Color(color.r, color.g, color.b, 1f);
    }

    private IEnumerator EnableColorEffect(SpriteRenderer target)
    {
        // 예: tint 색을 빨간색으로 바꾸기 (임의 설정)
        Color effectColor = new Color(1f, 0.5f, 0.5f, 1f);
        target.color = effectColor;
        yield break;
    }

    private IEnumerator DisableColorEffect(SpriteRenderer target)
    {
        target.color = Color.white;
        yield break;
    }
    private List<GameObject> activeEffects = new List<GameObject>();

    // 모든 이펙트 정리
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
