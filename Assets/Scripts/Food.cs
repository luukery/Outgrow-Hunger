using UnityEngine;

[System.Serializable]
public class Food
{
    public int ID;
    public int size;
    public string name;
    public FoodType.Type foodType;
    public Quality foodQuality;
    public Sprite icon;

    // Spoilage
    public bool isSpoilable = true;

    [Tooltip("Total transport-hours until spoiled (initial value).")]
    public float spoilTimeTotalHours = 0f;

    [Tooltip("Remaining transport-hours until spoiled.")]
    public float spoilTimeRemainingHours = 0f;

    private int price;

    public Food(
        FoodType.Type type,
        Quality quality,
        int size,
        string name,
        Sprite icon = null,
        int id = 0,
        bool isSpoilable = true,
        float spoilTimeInHours = 0f
    )
    {
        this.foodType = type;
        this.foodQuality = quality;
        this.size = size;
        this.name = name;
        this.icon = icon;
        this.ID = id;

        this.isSpoilable = isSpoilable;
        this.spoilTimeTotalHours = Mathf.Max(0f, spoilTimeInHours);
        this.spoilTimeRemainingHours = Mathf.Max(0f, spoilTimeInHours);

        UpdateQualityFromSpoilage(); // ensure quality consistent at creation
    }

    public enum Quality
    {
        Good,
        Medium,
        Low,
        Spoiled
    }

    public void Spoil()
    {
        foodQuality = Quality.Spoiled;
        price = 0;
        spoilTimeRemainingHours = 0f;
    }

    /// <summary>
    /// Advance spoilage by transport-hours. Returns true if food became Spoiled this tick.
    /// </summary>
    public bool AdvanceSpoilage(float hours)
    {
        if (!isSpoilable) return false;
        if (foodQuality == Quality.Spoiled) return false;
        if (hours <= 0f) return false;

        spoilTimeRemainingHours -= hours;

        if (spoilTimeRemainingHours <= 0f)
        {
            Spoil();
            return true;
        }

        UpdateQualityFromSpoilage();
        return false;
    }

    /// <summary>
    /// Maps remaining spoil time to Good/Medium/Low.
    /// Spoiled only when remaining <= 0 (handled elsewhere).
    /// </summary>
    void UpdateQualityFromSpoilage()
    {
        if (!isSpoilable) return;
        if (foodQuality == Quality.Spoiled) return;

        // If no spoil time configured, treat as non-spoilable.
        if (spoilTimeTotalHours <= 0f)
        {
            isSpoilable = false;
            return;
        }

        float pct = Mathf.Clamp01(spoilTimeRemainingHours / spoilTimeTotalHours);

        // You can tweak these thresholds easily.
        if (pct > 0.66f) foodQuality = Quality.Good;
        else if (pct > 0.33f) foodQuality = Quality.Medium;
        else foodQuality = Quality.Low;
    }
}
