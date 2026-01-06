using UnityEngine;

public class Food : MonoBehaviour
{
    //Public Variables
    public int ID; //Unique identifier for each food item.
    public int size;//The amount of space it takes up in the inventory. 
    public string name; //Porkchops, broccoli, tuna etc. Not the food type but the name of the actual food. 
    public FoodType.Type foodType;
    public Quality foodQuality;
    public Sprite icon;
    int minPrice;
    int maxPrice;

    public Food (FoodType.Type type, Quality quality, int size, string name)
    {
        this.foodType = type;
        this.foodQuality = quality;
        this.size = size;
        this.name = name;
    }

    public Food GetFoodByID(int ID)
    {
        //get food by ID logic
        return this;
    }
    public enum Quality
    {
        Good,
        Medium,
        Low,
        Spoiled
    }




    //Private Variables
    private float SpoilChance;
    private int price;



    private void Spoil(Food food)
    {
        //Spoil logic here
        food.foodQuality = Quality.Spoiled;
        food.price = 0;//Magic Number,  maar moet hier kunnen
    }

}
