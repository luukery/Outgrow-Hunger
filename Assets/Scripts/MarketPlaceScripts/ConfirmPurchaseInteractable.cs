using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class ConfirmPurchaseInteractable : MonoBehaviour, IInteractable
{
    [Header("UI")]
    [SerializeField] private GameObject confirmCanvas;

    [Header("Scene")]
    [SerializeField] private string transportSceneName = "Transport";

    private bool loading = false;

    public void Interact()
    {
        if (loading) return;

        if (confirmCanvas != null)
            confirmCanvas.SetActive(true);
        else
            Debug.LogWarning("ConfirmPurchaseInteractable: confirmCanvas is not assigned.");
    }

    public void ConfirmTravel()
    {
        if (loading) return;
        loading = true;
        StartCoroutine(LoadAfterRelease());
    }

    public void CancelTravel()
    {
        loading = false;
        if (confirmCanvas != null)
            confirmCanvas.SetActive(false);
    }

    private IEnumerator LoadAfterRelease()
    {
        yield return null;

        if (Mouse.current != null)
        {
            while (Mouse.current.leftButton.isPressed)
                yield return null;
        }
        else
        {
            while (Input.GetMouseButton(0))
                yield return null;
        }

        if (confirmCanvas != null)
            confirmCanvas.SetActive(false);

        LoadingManager.Instance.LoadScene(transportSceneName);
    }
}
