using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Food> foods = new List<Food>();

    //Inventory handeling
    public int maxCapacity;
    public int currentCapacity;




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
