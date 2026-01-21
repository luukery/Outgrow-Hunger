using UnityEngine;

public class NPCSpriteController : MonoBehaviour
{
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetWalking(bool walking)
    {
        animator.SetBool("Walking", walking);
    }
}
