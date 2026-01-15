using UnityEngine;

public class NPCSpriteController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] idleSprites;

    private NPCSprite sprite;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        spriteRenderer.sprite = idleSprites[0]; 
    }

    public void ApplySprite (NPCSprite newSprite)
    {
        sprite = newSprite;

        // Simple version (no animation yet)
        if (sprite.idleSprites.Length > 0)
            spriteRenderer.sprite = sprite.idleSprites[0];
    }
}
