using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public List<Food> foods = new List<Food>();

    public InventoryVisualiser vis;

    //Inventory handeling
    public int maxCapacity;
    public int currentCapacity;


    

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (TryAddFoodToInventory(new Food(FoodType.Type.Meat, Food.Quality.Medium, 1, "Porkchops")))
            {
                Debug.Log("Added Meat to inventory.");
            }
            else
            {
                Debug.Log("Not enough capacity to add food.");
            }
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (TryAddFoodToInventory(new Food(FoodType.Type.Fish, Food.Quality.Medium, 1, "Tuna")))
            {
                Debug.Log("Added fish to inventory.");
            }
            else
            {
                Debug.Log("Not enough capacity to add food.");
            }
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (TryAddFoodToInventory(new Food(FoodType.Type.Canned, Food.Quality.Medium, 1, "Beans")))
            {
                Debug.Log("Added Canned to inventory.");
            }
            else
            {
                Debug.Log("Not enough capacity to add food.");
            }
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            if (TryAddFoodToInventory(new Food(FoodType.Type.Fruit, Food.Quality.Medium, 1, "Grape")))
            {
                Debug.Log("Added Fruit to inventory.");
            }
            else
            {
                Debug.Log("Not enough capacity to add food.");
            }
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (TryAddFoodToInventory(new Food(FoodType.Type.Vegetable, Food.Quality.Medium, 1, "Pumpkin")))
            {
                Debug.Log("Added Veggie to inventory.");
            }
            else
            {
                Debug.Log("Not enough capacity to add food.");
            }
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            if (TryAddFoodToInventory(new Food(FoodType.Type.Bread, Food.Quality.Medium, 1, "Baguette")))
            {
                Debug.Log("Added Bread to inventory.");
            }
            else
            {
                Debug.Log("Not enough capacity to add food.");
            }
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (TryAddFoodToInventory(new Food(FoodType.Type.Water, Food.Quality.Medium, 1, "Bottle")))
            {
                Debug.Log("Added Water to inventory.");
            }
            else
            {
                Debug.Log("Not enough capacity to add food.");
            }
        }


        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Current Capacity: " + currentCapacity + " / " + maxCapacity);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(this.foods.Count + " items in inventory.");

            if (this.GetFoodsByType(FoodType.Type.Fish) != null)
            {
                Debug.Log("It is nit null");
            }
            else
            {
                Debug.Log("Failed to visualise inventory.");
            }
            vis.SetProgressBar(this);
        }
    }

    //Boolean logic || A few methods that try and do something. They return true when possibla and complete the action. They return false and dont do anything (yet) when impossible. 
    public bool TryAddFoodToInventory(Food food)
    {
        if ((currentCapacity + food.size) > maxCapacity)
        {
            return false;
        }
        else
        {
            AddFoodToInventory(food);
            return true;
        }
    }

    public bool TryRemoveFoodFromInventory(Food food)
    {
        //when the food miraculously would make the capacity go negative, it wont. 
        if ((food != null) && (currentCapacity - food.size >= 0))
        {
            RemoveFoodFromInventory(food);
        }
        return false;
    }

    //LOGIC || The PRIVATE methods that actually do the adding and removing from inventory.
    private void AddFoodToInventory(Food food)
    {
        foods.Add(food);
        currentCapacity += food.size;
    }

    private void RemoveFoodFromInventory(Food food)
    {
        foods.Remove(food);
        currentCapacity -= food.size;
    }

    public List<Food> GetFoodsByType(FoodType.Type type)
    {
        //return foods by type of food in inv 
        return foods.FindAll(f => f.foodType == type);
    }

    public List<Food> GetFoodsByQuality(Food.Quality quality)
    {
        return foods.FindAll(f => f.foodQuality == quality);
    }
    
    public bool CanSatisfyRequest(int amountRequested, FoodType.Type type)
    {
        int totalAvailable = 0;
        foreach (Food food in GetFoodsByType(type))
        {
            totalAvailable += food.size;
            if (totalAvailable >= amountRequested)
            {
                return true;
            }
        }
        return false;
    }
    public bool CanSatisfyRequest(int amountRequested, FoodType.Type type, Food.Quality quality)
    {
        int totalAvailable = 0;
        foreach (Food food in GetFoodsByType(type))
        {
            if (food.foodQuality == quality)
            {
                totalAvailable += food.size;
                if (totalAvailable >= amountRequested)
                {
                    return true;
                }
            }
        }
        return false;
    }


}




