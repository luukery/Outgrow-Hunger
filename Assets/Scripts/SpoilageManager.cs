using UnityEngine;

/// <summary>
/// Central system that advances spoilage ONLY when transport time advances.
/// RouteHandler should call AdvanceTravelTime(hours).
/// </summary>
public class SpoilageManager : MonoBehaviour
{
    public static SpoilageManager Instance { get; private set; }

    [Tooltip("Optional: total travel hours tracked (debug only).")]
    public float totalTravelHours;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Advances spoilage by travel time and returns how many FOOD UNITS were spoiled & removed.
    /// </summary>
    public int AdvanceTravelTime(float hours)
    {
        if (hours <= 0f) return 0;

        totalTravelHours += hours;

        if (Inventory.Instance != null)
            return Inventory.Instance.AdvanceTime(hours);

        return 0;
    }
}
