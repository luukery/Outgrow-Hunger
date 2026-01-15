using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class NPCSpriteController : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private NPCSprite spriteProfile;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
            Debug.LogError("NPCSpriteController: Missing SpriteRenderer!");
    }

    public void ApplySprite(NPCSprite newSprite)
    {
        if (newSprite == null)
        {
            Debug.LogError("NPCSpriteController: newSprite is NULL!");
            return;
        }

        spriteProfile = newSprite;

        if (spriteProfile.idleSprites == null || spriteProfile.idleSprites.Length == 0)
        {
            Debug.LogError($"NPCSpriteController: Sprite '{spriteProfile.name}' has NO idle sprites!");
            return;
        }

        spriteRenderer.sprite = spriteProfile.idleSprites[0];
    }
}
