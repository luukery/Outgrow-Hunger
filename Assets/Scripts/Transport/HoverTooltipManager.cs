using UnityEngine;
using TMPro;

public class HoverTooltipManager : MonoBehaviour
{
    public static HoverTooltipManager Instance;

    public GameObject tooltipPanel;
    public TMP_Text tooltipText;

    void Awake()
    {
        Instance = this;
        HideTooltip();
    }

    void Update()
    {
        if (tooltipPanel.activeSelf)
        {
            Vector2 mousePos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                tooltipPanel.transform.parent as RectTransform,
                Input.mousePosition,
                null,
                out mousePos
            );
            tooltipPanel.GetComponent<RectTransform>().localPosition = mousePos + new Vector2(10, -10);
        }
    }

    public void ShowTooltip(string text)
    {
        tooltipText.text = text;
        tooltipPanel.SetActive(true);
    }

    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
    }
}
