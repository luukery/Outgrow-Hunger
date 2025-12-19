using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class StallInteractable : MonoBehaviour, IInteractable
{
    public MarketStallFromCatalog stall;
    public GameObject stallUIPanel;
    public SceneScroller sceneScroller;

    public void Interact()
    {
        if (stall == null || stallUIPanel == null) return;
        
        if (stall.stallType == StallType.Cart)
        {
            sceneScroller.NextScene();
            Debug.Log("Moving to next scene for cart stall interaction.");
        }


            stall.OpenStall();

        var ui = stallUIPanel.GetComponent<StallUISimple>();
        if (ui != null) { ui.stall = stall; ui.Refresh(); }

        stallUIPanel.SetActive(true);
    }
}
