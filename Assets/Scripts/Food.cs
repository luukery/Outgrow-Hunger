using UnityEngine;

public class Food : MonoBehaviour
{
    //Public Variables
    public int size;//The amount of space it takes up in the inventory. 
    public string name; //Porkchops, broccoli, tuna etc. Not the food type but the name of the actual food. 

    public enum Quality
    {
        Good,
        Medium,
        Low,
        Spoiled
    }

    public FoodType.Type foodType;
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
