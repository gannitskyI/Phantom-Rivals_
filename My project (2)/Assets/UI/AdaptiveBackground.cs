using UnityEngine;

public class AdaptiveBackground : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (spriteRenderer.sprite == null)
        {
            Debug.LogError("Sprite not found on " + gameObject.name);
            return;
        }

        float worldScreenHeight = Camera.main.orthographicSize * 2f;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;

        Vector2 newScale = transform.localScale;
        newScale.x = worldScreenWidth / spriteSize.x;
        newScale.y = worldScreenHeight / spriteSize.y;
        transform.localScale = newScale;
    }
}
