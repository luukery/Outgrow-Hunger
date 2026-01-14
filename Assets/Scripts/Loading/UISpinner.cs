using UnityEngine;

public class UISpinner : MonoBehaviour
{
    [Tooltip("Degrees per second (clockwise).")]
    public float speed = 180f;

    RectTransform rt;

    void Awake() => rt = transform as RectTransform;

    void Update()
    {
        // clockwise in UI: negative z rotation usually looks clockwise
        rt.Rotate(0f, 0f, -speed * Time.unscaledDeltaTime);
    }
}
