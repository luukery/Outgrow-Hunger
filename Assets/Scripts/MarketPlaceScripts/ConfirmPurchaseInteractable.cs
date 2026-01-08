using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.InputSystem;

public class ConfirmPurchaseInteractable : MonoBehaviour, IInteractable
{
    public string transportSceneName = "Transport";
    private bool loading = false;

    public void Interact()
    {
        if (loading) return;
        loading = true;
        StartCoroutine(LoadAfterRelease());
    }

    private IEnumerator LoadAfterRelease()
    {
        // wait at least one frame
        yield return null;

        // wait until mouse button is released (prevents click carrying into next scene)
        if (Mouse.current != null)
        {
            while (Mouse.current.leftButton.isPressed)
                yield return null;
        }
        else
        {
            // fallback if old input is used
            while (Input.GetMouseButton(0))
                yield return null;
        }

        SceneManager.LoadScene(transportSceneName);
    }
}
