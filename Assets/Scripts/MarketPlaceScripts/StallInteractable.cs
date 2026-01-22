using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StallInteractable : MonoBehaviour, IInteractable
{
    [Header("Stall")]
    public MarketStallFromCatalog stall;
    public GameObject stallUIPanel;

    [Header("Tutorial lock")]
    [Tooltip("Sleep hier de DialogBox uit de Canvas in")]
    [SerializeField] private GameObject tutorialDialogBox;

    [Tooltip("Sleep hier de Background uit de Canvas in (optioneel)")]
    [SerializeField] private GameObject tutorialBackground;

    public void Interact()
    {
        // ❌ tutorial nog zichtbaar → geen interactie
        if ((tutorialDialogBox != null && tutorialDialogBox.activeInHierarchy) ||
            (tutorialBackground != null && tutorialBackground.activeInHierarchy))
            return;

        // ❌ al een kraam open
        if (StallUISimple.IsAnyStallOpen)
            return;

        if (stall == null || stallUIPanel == null)
            return;

        stall.OpenStall();

        var ui = stallUIPanel.GetComponent<StallUISimple>();
        if (ui != null)
        {
            ui.stall = stall;
            ui.Refresh();
        }

        stallUIPanel.SetActive(true);
    }
}
