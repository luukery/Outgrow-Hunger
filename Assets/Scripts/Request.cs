using System.Collections.Generic;

[System.Serializable]
public class Request
{
    public int Amount;
    public FoodType.Type FoodType;
    public Food.Quality Quality;
    public bool Possible;

    public Request(int amount, FoodType.Type foodType, Food.Quality quality)
    {
        Amount = amount;
        FoodType = foodType;
        Quality = quality;
    }
}