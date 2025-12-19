using NUnit;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryVisualiser : MonoBehaviour
{
	[Header("General settings")]
    public float GeneralScale = 0.3f;//Steps for the progressbar
	public GameObject Parent;

    [Header("General Bar settings")]
	public Transform GeneralBarLocation;
    public List<GameObject> PlaceHolderObjects;
	public float IndividualScale = 0.3f;

    [Header("Individual Bar settings")]
	public List<GameObject> IndivisualBarPositions;
	private List<GameObject> IndividualBarObjects;

    [Header("Legacy settings")]
    public UnityEngine.UI.Slider CapacityBar;


    [Header("Hidden settings")]
    public Inventory SelectedInventory;
	private List<Food> ActiveInventory;



    private List<List<Food>> TempList = new List<List<Food>>();
    //Filled per category of food.

    private void Awake()
    {
		IndividualBarObjects = IndivisualBarPositions;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            CreateGreyinventoryBar(SelectedInventory, GeneralBarLocation.position);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            CreateColourInventoryBar(SelectedInventory, GeneralBarLocation.position);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
			List<Vector3> Locations = new List<Vector3>();
            foreach (var item in PlaceHolderObjects)
			{
				Locations.Add(item.transform.position);
            }
                CreateIndividualBars(SelectedInventory, Locations);
        }
    }

    public Tuple<int[], Inventory> SortFoodByTypeWithSize(Inventory inventory)
	{
		TempList.Clear();
        int[] TypeSizeByType = new int[Enum.GetValues(typeof(FoodType.Type)).Length];
		Inventory ReturnInventory = new Inventory();
		ReturnInventory.maxCapacity = inventory.maxCapacity;
		ReturnInventory.currentCapacity = inventory.currentCapacity;

		List<Food> newFoods = new List<Food>();

        foreach (FoodType.Type foodType in Enum.GetValues(typeof(FoodType.Type)))
		{
			//do once per type
			List<Food> AllFoodByType = new List<Food>();
			var foodInThatType = inventory.GetFoodsByType(foodType) ?? new List<Food>();

			foreach (Food food in foodInThatType)
			{
				AllFoodByType.Add(food);
				newFoods.Add(food);
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
		ReturnInventory.foods = newFoods;
		Tuple<int[], Inventory> result;
		result = Tuple.Create(TypeSizeByType, ReturnInventory);
        return result;


	}

	public void SetProgressBar(Inventory inv)
	{
		//Vector3 UsablePosition = GeneralBarLocation.position;
		//List<Vector3> UsableInvPositions = new List<Vector3>();
		//foreach (GameObject item in IndividualBarObjects)
		//{
		//	UsableInvPositions.Add(item.transform.position);
		//}

		////remove all child objects from Parent object
		//foreach (Transform child in Parent.transform)
		//{
		//	Destroy(child.gameObject);
		//}



		//if (inv == null)
		//{
		//	Debug.LogError("Inventory is NULL!");
		//	return;
		//}

		//if (inv.foods == null)
		//{
		//	Debug.LogError("Inventory.foods is NULL!");
		//	return;
		//}

		//Tuple<int[], Inventory> CountOfFoodByType = SortFoodByTypeWithSize(inv);

		//CreateColourInventoryBar(inv, UsablePosition);

		//for (int i = 0; i < Enum.GetValues(typeof(FoodType.Type)).Length; i++)//do seven times as we have 7 food types
		//{
		//	//happens once every type
		//	for (int j = 0; j < CountOfFoodByType.Item1[i]; j++)
		//	{
		//		GameObject IndividualObject = Instantiate(PlaceHolderObjects[i], Parent.transform);
		//		//General Bar in the bottom
		//		globalBarPosition.x += GeneralScale;
		//		//duplicate the place holder object in the array at position i
		//		GameObject GeneralObject = Instantiate(PlaceHolderObjects[i], Parent.transform);
		//		//set it to the right position

		//		GeneralObject.transform.localScale = new Vector3(GeneralScale, GeneralScale, GeneralScale);
		//		GeneralObject.transform.position = GeneralBarLocation.position;



		//		//update the next position
		//		globalBarPosition = new Vector3(globalBarPosition.x, GeneralBarLocation.position.y, GeneralBarLocation.position.z);



		//		//Individual Bars
		//		Transform Ipos = IndivisualBarPositions[i].transform;
		//		Vector3 pos = new Vector3(Ipos.position.x, Ipos.position.y, Ipos.position.z);
		//		Ipos.position = pos;
		//		//create an object


		//		//setscale
		//		IndividualObject.transform.localScale = new Vector3(IndividualScale, IndividualScale, IndividualScale);

		//		//setlocation
		//		IndividualObject.transform.position = IndivisualBarPositions[i].transform.position;
		//		Debug.Log("ik heb je " + j + "ste blokje van type " + (FoodType.Type)i + " geplaatst op positie " + IndividualObject.transform.position);

		//		//updatePosition
		//		Ipos.position = new Vector3(IndivisualBarPositions[i].transform.position.x, Ipos.position.x, IndivisualBarPositions[i].transform.position.z);
		//		Debug.Log("en toen de volgende positie verdanderd met " + IndividualScale + " naar " + IndivisualBarPositions[i].transform.position);


		//	}
		//}
		//Debug.Log("I have done the visualisation. And i have ...");

	}
	public void CreateGreyinventoryBar(Inventory inventory, Vector3 BarPosition)
	{
		foreach (Food item in inventory.foods)
		{
			GameObject GreybarPlaceholder = Instantiate(PlaceHolderObjects[2], Parent.transform);
			GreybarPlaceholder.transform.position = BarPosition;
			BarPosition.x += GeneralScale;
        }
	}
	public void CreateColourInventoryBar(Inventory inventory, Vector3 BarPosition)
	{
        Tuple<int[], Inventory> CountOfFoodByType = SortFoodByTypeWithSize(inventory);
		//creates a coupled list. If i have 5 meats and 8 fish and 3 canned, it will return a list with [5,8,3]+List<Food> 

		for (int TypeIndex = 0; TypeIndex < CountOfFoodByType.Item1.Length; TypeIndex++)//do this as many times as i have types of food. 
		{
			for (int i = 0; i < CountOfFoodByType.Item1[TypeIndex]; i++)//do this as many times as i have a specific type of food. 
			{
				GameObject NewBarObject = Instantiate(PlaceHolderObjects[i], Parent.transform);
				NewBarObject.transform.position = BarPosition;
				BarPosition.x += GeneralScale;
			}
		}

    }
	public void CreateIndividualBars(Inventory inventory, List<Vector3> BarPositions)
	{
        Tuple<int[], Inventory> CountOfFoodByType = SortFoodByTypeWithSize(inventory);
        //creates a coupled list. If i have 5 meats and 8 fish and 3 canned, it will return a list with [5,8,3]+List<Food> 

        for (int TypeIndex = 0; TypeIndex < CountOfFoodByType.Item1.Length; TypeIndex++)//do this as many times as i have types of food. 
        {
            for (int i = 0; i < CountOfFoodByType.Item1[TypeIndex]; i++)//do this as many times as i have a specific type of food. 
            {
                GameObject NewBarObject = Instantiate(PlaceHolderObjects[i], Parent.transform);
				NewBarObject.transform.position = BarPositions[TypeIndex];
                Vector3 temp = BarPositions[TypeIndex];
                temp.y += IndividualScale;
                BarPositions[TypeIndex] = temp;
            }
        }
    }

}