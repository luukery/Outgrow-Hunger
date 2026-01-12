using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    public List<Food> foods = new List<Food>();
    public InventoryVisualiser vis;

    public int maxCapacity;
    public int currentCapacity;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetTotalFoodUnits()
    {
        int total = 0;
        if (foods != null)
        {
            foreach (var f in foods)
            {
                if (f != null)
                    total += Mathf.Max(0, f.size);
            }
        }
        return total;
    }

    public bool TryRemoveFoodUnits(int amount)
    {
        if (amount <= 0) return true;

        int available = GetTotalFoodUnits();
        int toRemove = Mathf.Min(amount, available);

        for (int i = foods.Count - 1; i >= 0 && toRemove > 0; i--)
        {
            var f = foods[i];
            if (f == null)
            {
                foods.RemoveAt(i);
                continue;
            }

            int take = Mathf.Min(f.size, toRemove);
            f.size -= take;
            toRemove -= take;
            currentCapacity = Mathf.Max(0, currentCapacity - take);

            if (f.size <= 0)
                foods.RemoveAt(i);
        }

        return available >= amount;
    }

    public bool TryAddFoodToInventory(Food food)
    {
        if (food == null) return false;

        if ((currentCapacity + food.size) > maxCapacity)
            return false;

        foods.Add(food);
        currentCapacity += food.size;
        return true;
    }

    public bool TryRemoveFoodFromInventory(Food food)
    {
        if (food == null) return false;
        if (!foods.Contains(food)) return false;
        if (currentCapacity - food.size < 0) return false;

        foods.Remove(food);
        currentCapacity -= food.size;
        return true;
    }

    public List<Food> GetFoodsByType(FoodType.Type type)
    {
        return foods.FindAll(f => f.foodType == type);
    }

    public List<Food> GetFoodsByQuality(Food.Quality quality)
    {
        return foods.FindAll(f => f.foodQuality == quality);
    }

    public List<Request> CanSatisfyOrder(List<Request> orderByNPC)
    {
        foreach (Request r in orderByNPC)
            r.Possible = CanSatisfyRequest(r.Amount, r.FoodType, r.Quality);

        return orderByNPC;
    }

    private bool CanSatisfyRequest(int amountRequested, FoodType.Type type, Food.Quality quality)
    {
        int totalAvailable = 0;

        foreach (Food food in GetFoodsByType(type))
        {
            if (food.foodQuality == quality)
            {
                totalAvailable += food.size;
                if (totalAvailable >= amountRequested)
                    return true;
            }
        }
        return false;
    }
}
