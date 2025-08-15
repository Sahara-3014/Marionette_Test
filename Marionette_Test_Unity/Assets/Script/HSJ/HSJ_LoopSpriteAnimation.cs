using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HSJ_LoopSpriteAnimation : MonoBehaviour
{
    public static HSJ_LoopSpriteAnimation instance;
    public List<Sprite> sprites;
    public float animTime = 0.1f; // Time between sprite changes
    public Sprite pauseSprite;
    public Sprite stopSprite;
    SpriteRenderer spriteRenderer;
    Image image;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        image = GetComponent<Image>();

        StopAnim();
    }

    private void OnDisable()
    {
        //CancelInvoke(nameof(PlayAnim));
        //StopAnim();
    }

    public void PlayAnim()
    {
        var sprite = spriteRenderer != null ? spriteRenderer.sprite : image.sprite;

        int nowIndex = sprites.IndexOf(sprite);
        nowIndex += 1;
        if(nowIndex >= sprites.Count)
            nowIndex = 0;

        if (spriteRenderer != null)
            spriteRenderer.sprite = sprites[nowIndex];
        else if (image != null)
            image.sprite = sprites[nowIndex];

        Invoke(nameof(PlayAnim), animTime);
    }

    public void PauseAnim()
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = pauseSprite;
        else if (image != null)
            image.sprite = pauseSprite;
    }

    public void StopAnim()
    {
        if (spriteRenderer != null)
            spriteRenderer.sprite = stopSprite;
        else if (image != null)
            image.sprite = stopSprite;
    }
}
