using UnityEngine;

public class ArrowLegSetter : MonoBehaviour
{
    public RectTransform arrowRect;
    public Vector2[] splitPositions;

    Vector2 basePos;

    void Start()
    {
        int leg = Mathf.Clamp(LoadingContext.LegIndex, 0, splitPositions.Length - 1);
        basePos = splitPositions[leg];

        arrowRect.anchoredPosition = basePos;
        arrowRect.gameObject.SetActive(true);
    }

    void Update()
    {
        // simple bounce
        float bounce = Mathf.Sin(Time.unscaledTime * 6f) * 8f;
        arrowRect.anchoredPosition = basePos + new Vector2(0, bounce);
    }
}
