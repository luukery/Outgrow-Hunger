using UnityEngine;
using UnityEngine.InputSystem;

public class PointClick2D : MonoBehaviour
{
    Camera cam;

    void Awake() => cam = Camera.main;

    void Update()
    {
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Vector2 screen = Mouse.current.position.ReadValue();
        Vector3 world = cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, -cam.transform.position.z));
        Vector2 wp = new Vector2(world.x, world.y);

        Collider2D hit = Physics2D.OverlapPoint(wp);
        if (hit != null)
        {
            var interactable = hit.GetComponent<IInteractable>();
            if (interactable != null)
                interactable.Interact();
        }
    }
}
