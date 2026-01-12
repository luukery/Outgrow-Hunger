using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactRange = 1.5f; // How close you need to be to interact
    private IPlayerInteractable currentInteractable;

    // Keep track of all interactables we're touching
    private List<IPlayerInteractable> nearbyInteractables = new List<IPlayerInteractable>();

    void Update()
    {
        if (currentInteractable != null)
        {
            Debug.Log("Current interactable: " + currentInteractable.GetPosition());
            if (Input.GetKeyDown(KeyCode.E))
            {
                currentInteractable.Interact();
                Debug.Log("Pressed E!");
            }
        }

        // Find the nearest interactable each frame
        UpdateCurrentInteractable();

        // Press E to interact
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            currentInteractable.Interact();
        }

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

    private void UpdateCurrentInteractable()
    {
        if (nearbyInteractables.Count == 0)
        {
            currentInteractable = null;
            return;
        }

        // Find closest interactable
        IPlayerInteractable nearest = null;
        float closestDistance = float.MaxValue;

        foreach (var interactable in nearbyInteractables)
        {
            float distance = Vector2.Distance(transform.position, interactable.GetPosition());
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearest = interactable;
            }
        }

        // If the closest interactable changed, close the old one
        if (currentInteractable != null && currentInteractable != nearest)
        {
            if (currentInteractable is StallInteractable oldStall)
                oldStall.Close();
        }

        currentInteractable = nearest;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger entered: " + other.name);
        var interactable = other.GetComponent<IPlayerInteractable>();
        if (interactable != null)
            Debug.Log("Found interactable: " + interactable);

        if (!nearbyInteractables.Contains(interactable))
            nearbyInteractables.Add(interactable);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var interactable = other.GetComponentInParent<IPlayerInteractable>();
        if (interactable == null) return;

        nearbyInteractables.Remove(interactable);

        if (currentInteractable == interactable)
        {
            if (interactable is StallInteractable stall)
                stall.Close();

            currentInteractable = null;
        }
    }

}
