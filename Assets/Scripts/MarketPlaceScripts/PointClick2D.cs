// PointClick2D.cs (Input System versie)
using UnityEngine;
using UnityEngine.InputSystem; // <-- nieuwe Input System API

public class PointClick2D : MonoBehaviour
{
    Camera cam;

    void Awake() => cam = Camera.main;

    void Update()
    {
        // Geen muis (bijv. mobiel)? Dan niks doen.
        if (Mouse.current == null) return;

        // Klik dit frame?
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        // Scherm -> wereld
        Vector2 screen = Mouse.current.position.ReadValue();
        Vector3 wp3 = cam.ScreenToWorldPoint(new Vector3(screen.x, screen.y, -cam.transform.position.z));
        Vector2 wp = new Vector2(wp3.x, wp3.y);

        // Check collider onder de muis
        Collider2D hit = Physics2D.OverlapPoint(wp);
        if (hit != null)
        {
            var it = hit.GetComponent<IInteractable>();
            if (it != null) it.Interact();
            else Debug.Log($"Geklikt op {hit.name} maar geen IInteractable.");
        }
        else
        {
            Debug.Log("Niets onder de muis.");
        }
    }
}
