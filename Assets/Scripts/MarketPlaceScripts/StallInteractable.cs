using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StallInteractable : MonoBehaviour, IPlayerInteractable
{
    public MarketStallFromCatalog stall;
    public GameObject stallUIPanel;

    public void Interact()
    {
        Debug.Log($"Interact called on {gameObject.name} | Stall: {stall} | Panel: {stallUIPanel}");

        if (stall == null || stallUIPanel == null) return;

        stall.OpenStall();

        var ui = stallUIPanel.GetComponent<StallUISimple>();
        if (ui != null) { ui.stall = stall; ui.Refresh(); }

        stallUIPanel.SetActive(true);
    }



    public void Close()
    {
        if (stallUIPanel != null)
            stallUIPanel.SetActive(false);
    }
    public Vector2 GetPosition()
    {
        return transform.position;
    }
}
