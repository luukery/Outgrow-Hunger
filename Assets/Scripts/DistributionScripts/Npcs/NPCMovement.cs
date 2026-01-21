using UnityEngine;
using System.Collections;

public class NPCMovement : MonoBehaviour
{
    private float moveSpeed = 1.5f;
    private NPCSpriteController spriteController;

    void Awake()
    {
        spriteController = GetComponent<NPCSpriteController>();
    }

    public IEnumerator MoveTo(Vector3 target)
    {
        spriteController.SetWalking(true);
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = target;
        spriteController.SetWalking(false);
    }
}
