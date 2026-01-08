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
}
