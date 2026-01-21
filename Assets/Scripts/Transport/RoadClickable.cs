using UnityEngine;
using UnityEngine.EventSystems;

public class RoadClickable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Route Index (0 = short, 1 = medium, 2 = long)")]
    public int routeIndex = 0;

    [Header("Highlight Settings")]
    public Renderer[] targetRenderers;
    public Color baseColor = Color.white;
    public Color hoverColor = Color.yellow;
    public Color selectedColor = Color.green;
    public string colorPropertyName = "_Color";  // or "_BaseColor" depending on your shader

    private bool isSelected = false;

    void Reset()
    {
        targetRenderers = GetComponentsInChildren<Renderer>();
    }

    void Start()
    {
        if (targetRenderers == null || targetRenderers.Length == 0)
            targetRenderers = GetComponentsInChildren<Renderer>();

        SetColor(baseColor);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isSelected) return;
        SetColor(hoverColor);

        if (RouteHandler.Instance != null && RouteHandler.Instance.routes != null && routeIndex < RouteHandler.Instance.routes.Count)
        {
            var evt = RouteHandler.Instance.routes[routeIndex].EventData;
            if (evt != null)
                HoverTooltipManager.Instance.ShowTooltip(evt.Name);
            else
                HoverTooltipManager.Instance.ShowTooltip("No Event");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isSelected) return;
        SetColor(baseColor);

        HoverTooltipManager.Instance.HideTooltip();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (RouteHandler.Instance == null) return;
        if (!RouteHandler.Instance.CanSelectRoutes) return;

        RouteHandler.Instance.SelectRouteByIndex(routeIndex);

        isSelected = true;
        SetColor(selectedColor);
    }


    private void SetColor(Color color)
    {
        if (targetRenderers == null) return;

        foreach (var rend in targetRenderers)
        {
            if (rend == null) continue;

            foreach (var mat in rend.materials)
            {
                if (mat.HasProperty(colorPropertyName))
                    mat.SetColor(colorPropertyName, color);
            }
        }
    }

    // Called by RouteHandler to clear highlight between legs / when scene resets
    public void ResetHighlight()
    {
        isSelected = false;
        SetColor(baseColor);
    }
}
