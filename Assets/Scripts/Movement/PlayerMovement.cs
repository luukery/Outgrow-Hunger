using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float interactRange = 1.5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    private List<IPlayerInteractable> nearbyInteractables = new List<IPlayerInteractable>();
    private IPlayerInteractable currentInteractable;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Movement input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize();

        // Find nearest interactable
        UpdateCurrentInteractable();

        // Interact
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
            currentInteractable.Interact();

        // Auto-close if too far
        if (currentInteractable != null)
        {
            float distance = Vector2.Distance(transform.position, currentInteractable.GetPosition());
            if (distance > interactRange)
            {
                if (currentInteractable is StallInteractable stall)
                    stall.Close();
                currentInteractable = null;
            }
        }
    }

    void FixedUpdate()
    {
        // Move player via Rigidbody2D
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    private void UpdateCurrentInteractable()
    {
        if (nearbyInteractables.Count == 0)
        {
            currentInteractable = null;
            return;
        }

        IPlayerInteractable nearest = null;
        float closestDistance = float.MaxValue;

        foreach (var interactable in nearbyInteractables)
        {
            float dist = Vector2.Distance(transform.position, interactable.GetPosition());
            if (dist < closestDistance)
            {
                closestDistance = dist;
                nearest = interactable;
            }
        }

        if (currentInteractable != null && currentInteractable != nearest)
        {
            if (currentInteractable is StallInteractable oldStall)
                oldStall.Close();
        }

        currentInteractable = nearest;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<IPlayerInteractable>(out var interactable))
        {
            if (!nearbyInteractables.Contains(interactable))
                nearbyInteractables.Add(interactable);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<IPlayerInteractable>(out var interactable))
        {
            nearbyInteractables.Remove(interactable);
            if (currentInteractable == interactable && interactable is StallInteractable stall)
            {
                stall.Close();
                currentInteractable = null;
            }
        }
    }
}
