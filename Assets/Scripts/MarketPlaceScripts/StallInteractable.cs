using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StallInteractable : MonoBehaviour, IInteractable
{
    public MarketStallFromCatalog stall;
    public GameObject stallUIPanel;


    //TODO Add a cartInteracatble to switch scenes

    public void Interact()
    {
        if (stall == null || stallUIPanel == null) return;

        stall.OpenStall();

        var ui = stallUIPanel.GetComponent<StallUISimple>();
        if (ui != null) { ui.stall = stall; ui.Refresh(); }

        stallUIPanel.SetActive(true);
    }
}
