using UnityEngine;

[RequireComponent(typeof(Animator))]
public class NPCSpriteController : MonoBehaviour
{
    private Animator animator;
    private AnimatorOverrideController overrideController;

    void Awake()
    {
        animator = GetComponent<Animator>();

        overrideController =
            new AnimatorOverrideController(animator.runtimeAnimatorController);

        animator.runtimeAnimatorController = overrideController;
    }

    public void ApplyAnimations(AnimationClip idle, AnimationClip walk)
    {
        if (idle != null)
            overrideController["NPC_Idle"] = idle;

        if (walk != null)
            overrideController["NPC_Walk"] = walk;
    }

    public void SetWalking(bool walking)
    {
        animator.SetBool("Walking", walking);
    }
}
