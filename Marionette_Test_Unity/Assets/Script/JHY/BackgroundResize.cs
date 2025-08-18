using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundFitter : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Camera mainCamera;
    private Vector2 lastScreenSize;

    [SerializeField] private float safetyMargin = 1.05f;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
        lastScreenSize = new Vector2(Screen.width, Screen.height);
        ResizeBackground();
    }

    void Update()
    {
        Vector2 currentScreenSize = new Vector2(Screen.width, Screen.height);
        if (currentScreenSize != lastScreenSize)
        {
            ResizeBackground();
            lastScreenSize = currentScreenSize;
        }
    }

    void ResizeBackground()
    {
        float orthoSize = mainCamera.orthographicSize;
        float screenHeight = orthoSize * 2f;
        float screenWidth = screenHeight * ((float)Screen.width / (float)Screen.height);

        float actualSpriteWorldWidth = (screenWidth / 100f) / safetyMargin;
        float actualSpriteWorldHeight = (screenHeight / 100f) / safetyMargin;

        float scaleX = screenWidth / actualSpriteWorldWidth;
        float scaleY = screenHeight / actualSpriteWorldHeight;

        transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }
}
