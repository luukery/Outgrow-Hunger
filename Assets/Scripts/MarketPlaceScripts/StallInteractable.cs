using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StallInteractable : MonoBehaviour, IInteractable
{
    public MarketStallSimple stall;   // dit is de kraam op dit object
    public GameObject stallUIPanel;   // je gedeelde StallPanel in de Canvas

    public void Interact()
    {
        if (stall == null || stallUIPanel == null) return;

        // 1) Genereer/zet voorraad
        stall.OpenStall();

        // 2) Zeg tegen het paneel welke kraam actief is + vernieuw UI
        var ui = stallUIPanel.GetComponent<StallUISimple>();
        if (ui != null)
        {
            ui.stall = stall;   // <<< belangrijk
            ui.Refresh();       // hertekenen van slots/teksten
        }

        // 3) Toon paneel
        stallUIPanel.SetActive(true);
    }
}
