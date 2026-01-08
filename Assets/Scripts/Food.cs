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

    private float spoilChance;
    private int price;

    public Food(FoodType.Type type, Quality quality, int size, string name, Sprite icon = null, int id = 0)
    {
        this.foodType = type;
        this.foodQuality = quality;
        this.size = size;
        this.name = name;
        this.icon = icon;
        this.ID = id;
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
    }
}
