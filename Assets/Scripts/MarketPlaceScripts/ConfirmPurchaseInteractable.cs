using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;

public class ConfirmPurchaseInteractable : MonoBehaviour, IInteractable
{
    [Header("UI")]
    [SerializeField] private GameObject confirmCanvas;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    [Header("Tutorial lock")]
    [Tooltip("Sleep hier Canvas/DialogBox in (van je tutorial).")]
    [SerializeField] private GameObject tutorialDialogBox;

    [Tooltip("Sleep hier Canvas/Background in (van je tutorial).")]
    [SerializeField] private GameObject tutorialBackground;

    [Header("Scene")]
    [SerializeField] private string transportSceneName = "Transport";

    private bool loading = false;

    private void Start()
    {
        // Auto-find buttons if not assigned
        if (confirmCanvas != null && confirmButton == null)
            confirmButton = confirmCanvas.transform.Find("ConfirmButton")?.GetComponent<Button>();

        if (confirmCanvas != null && cancelButton == null)
            cancelButton = confirmCanvas.transform.Find("CancelButton")?.GetComponent<Button>();

        // Wire up button listeners
        if (confirmButton != null)
        {
            confirmButton.onClick.RemoveAllListeners();
            confirmButton.onClick.AddListener(ConfirmTravel);
        }
        if (cancelButton != null)
        {
            cancelButton.onClick.RemoveAllListeners();
            cancelButton.onClick.AddListener(CancelTravel);
        }
    }

    public void Interact()
    {
        if (loading) return;

        // ❌ tutorial nog zichtbaar → niet klikbaar
        if ((tutorialDialogBox != null && tutorialDialogBox.activeInHierarchy) ||
            (tutorialBackground != null && tutorialBackground.activeInHierarchy))
            return;

        // ❌ als stall UI open is, ook geen travel confirm openen
        if (StallUISimple.IsAnyStallOpen)
            return;

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

        LoadingManager.Instance.LoadTransportMap(transportSceneName, 0);
    }
}
