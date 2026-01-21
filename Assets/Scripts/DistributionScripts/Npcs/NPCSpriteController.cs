using UnityEngine;

public class NPCSpriteController : MonoBehaviour
{
    private Animator animator;
    private AnimatorOverrideController overrideController;

    void Awake()
    {
        animator = GetComponent<Animator>();
        overrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = overrideController;
    }

    public void ApplySprite(NPCSprite newSprite)
    {
        overrideController["NPC_Idle"] = newSprite.idle;

        if (newSprite.walk != null)
            overrideController["NPC_Walk"] = newSprite.walk;
    }

    public void SetWalking(bool walking)
    {
        animator.SetBool("Walking", walking);
    }
}
