using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryVisualiser : MonoBehaviour
{
	public Inventory SelectedInventory;
	private List<Food> ActiveInventory;
    public UnityEngine.UI.Slider CapacityBar;

    public List<GameObject> PlaceHolderObjects;

    private List<List<Food>> TempList;
    //Filled per category of food.

    private void Update()
    {

    }

    public int[] SortFoodByTypeWithSize(Inventory inventory)
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

		return TypeSizeByType;


	}

	public void SetProgressBar(Inventory inv)
	{
		ActiveInventory = inv.foods;
		SelectedInventory = inv;


        int pos = 0;

		int[] iets = SortFoodByTypeWithSize(SelectedInventory);

		for (int i = 0; i < Enum.GetValues(typeof(FoodType.Type)).Length; i++)//do seven times as we have 7 food types
		{
			//happens once every type
			for (int j = 0; j < i; j++)
			{
                //duplicate the place holder object in the array at position i
				
				GameObject d = Instantiate(PlaceHolderObjects[i]);
				d.transform.position = new Vector3(pos, 0, 0);
				pos++;
				//happens as much as the amount of food in the category with index i as type index. 
			}
		}

	}

}