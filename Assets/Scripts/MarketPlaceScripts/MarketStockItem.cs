using UnityEngine;

[System.Serializable]
public class MarketStockItem
{
    public string name;
    public Sprite icon;
    public int price;

    [Header("Food mapping")]
    public FoodType.Type foodType;
    public Food.Quality foodQuality = Food.Quality.Medium;
    public int size = 1;

    [Header("Spoilage")]
    public bool isSpoilable = true;

    [Tooltip("How many transport-hours until fully spoiled (only counts RouteHandler travel time).")]
    public float spoilTimeInHours = 12f;

    /// <summary>
    /// Helper: turn this stock item into an actual Food instance for the Inventory.
    /// Call this from your buy logic.
    /// </summary>
    public Food ToFood(int id = 0)
    {
        return new Food(
            foodType,
            foodQuality,
            size,
            name,
            icon,
            id,
            isSpoilable,
            spoilTimeInHours
        );
    }
}
