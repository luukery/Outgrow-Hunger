using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
<<<<<<< Updated upstream
=======
using UnityEngine.UIElements;
using UnityEngine.UI;
>>>>>>> Stashed changes

public class Inventory : MonoBehaviour
{
    public List<Food> foods = new List<Food>();

<<<<<<< Updated upstream
=======
    public InventoryVisualiser vis;

>>>>>>> Stashed changes
    //Inventory handeling
    public int maxCapacity;
    public int currentCapacity;


<<<<<<< Updated upstream

=======
    

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (TryAddFoodToInventory(new Food(FoodType.Type.Meat, Food.Quality.Medium, 1, "Porkchops")))
            {
                Debug.Log("Added food to inventory.");
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
                Debug.Log("Added food to inventory.");
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
                Debug.Log("Added food to inventory.");
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
                Debug.Log("Added food to inventory.");
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
                Debug.Log("Added food to inventory.");
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
                Debug.Log("Added food to inventory.");
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
                Debug.Log("Added food to inventory.");
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
            vis.SetProgressBar(this);
        }
    }
>>>>>>> Stashed changes

    //Boolean logic || een aantal methods die ook in de normale locig staan, maar dan als een true/false voor het wel en niet lukken. 
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

    //LOGIC || basically de methods die hele simpele logica doen. Inklappen en niet meer naar kijken
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
}

<<<<<<< Updated upstream
public class InventoryVisualiser
{
    private List<Food> ActiveInventory;

    private List<List<Food>> TempList;
    //Filled per category of food.

    public void SortFoodByTypeWithSize(Inventory inventory)
    {
        int[] TypeSizeByType = new int[Enum.GetValues(typeof(FoodType.Type)).Length];


        foreach (FoodType.Type foodType in Enum.GetValues(typeof(FoodType.Type)))
        {
            //do once per type
            List<Food> AllFoodByType = new List<Food>();
            var foodInThatType = inventory.GetFoodsByType(foodType);

            foreach (Food food in foodInThatType)
            {
                AllFoodByType.Add(food);
            }
            TempList.Add(AllFoodByType);
        }

        int i = 0;
        foreach (List<Food> typeCollection in TempList)
        {

            int typeCount = 0;
            foreach (Food food in typeCollection)
            {
                typeCount += food.size;
            }
            TypeSizeByType[i] = typeCount;
            i++;
        }





    }
}
=======

>>>>>>> Stashed changes
