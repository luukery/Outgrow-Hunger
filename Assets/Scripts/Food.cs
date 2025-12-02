using UnityEngine;

public class Food : MonoBehaviour
{
    //Public Variables
    public int size;//The amount of space it takes up in the inventory. 
    public string name; //Porkchops, broccoli, tuna etc. Not the food type but the name of the actual food. 
<<<<<<< Updated upstream

=======
    public FoodType.Type foodType;

    public Food (FoodType.Type type, Quality quality, int size, string name)
    {
        this.foodType = type;
        this.foodQuality = quality;
        this.size = size;
        this.name = name;
    }
>>>>>>> Stashed changes
    public enum Quality
    {
        Good,
        Medium,
        Low,
        Spoiled
    }

<<<<<<< Updated upstream
    public FoodType.Type foodType;
=======
>>>>>>> Stashed changes
    public Quality foodQuality;



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
