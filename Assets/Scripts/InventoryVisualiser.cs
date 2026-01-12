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
	public Transform barLocation;
    public List<GameObject> PlaceHolderObjects;
	public float IndividualScale = 0.3f;

    [Header("Individual Bar settings")]
	public List<GameObject> IndivisualBarPositions;

    [Header("Legacy settings")]
    public UnityEngine.UI.Slider CapacityBar;


    [Header("Hidden settings")]
    public Inventory SelectedInventory;
	private List<Food> ActiveInventory;



    private List<List<Food>> TempList = new List<List<Food>>();
    //Filled per category of food.

    private void Update()
    {

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
        //DOES NOW
        //some random fucking bulshit


        //Should do
        //set thee location for where to place objects in a variable. 
        //change those values, not the root value
        //clear the visualiser when starting. 
        //Maybe even better to clear when active is fasle and fill when active is true.

        //Split the general bar and the individual bars into 2 different functions.
        //Add better logs
        //update the bars when adding and removing food. With some sort of update function not called "private void Update" because unity. 
        //Make it so the prefabs that make the bar now are not in the editor but in a map in the assets. 

		var globalBarPosition = barLocation.position;

		//remove all child objects from Parent object
		foreach (Transform child in Parent.transform)
		{
			Destroy(child.gameObject);
		}



        if (inv == null)
        {
            Debug.LogError("Inventory is NULL!");
            return;
        }

        if (inv.foods == null)
        {
            Debug.LogError("Inventory.foods is NULL!");
            return;
        }
		ActiveInventory = inv.foods;
		SelectedInventory = inv;

		Tuple<int[], Inventory> CountOfFoodByType = SortFoodByTypeWithSize(SelectedInventory);



		for (int i = 0; i < Enum.GetValues(typeof(FoodType.Type)).Length; i++)//do seven times as we have 7 food types
		{
            //happens once every type
            for (int j = 0; j < CountOfFoodByType.Item1[i]; j++)
			{
			//General Bar in the bottom
				globalBarPosition.x += GeneralScale;
				//duplicate the place holder object in the array at position i
				GameObject GeneralObject = Instantiate(PlaceHolderObjects[i], Parent.transform);
				//set it to the right position
				
				GeneralObject.transform.localScale = new Vector3(GeneralScale, GeneralScale, GeneralScale);
				GeneralObject.transform.position = barLocation.position;

				

				//update the next position
				globalBarPosition = new Vector3(globalBarPosition.x, barLocation.position.y, barLocation.position.z);



			//Individual Bars
				Transform Ipos = IndivisualBarPositions[i].transform;
				Vector3 pos = new Vector3 (Ipos.position.x, Ipos.position.y, Ipos.position.z);
                Ipos.position = pos;
				//create an object
				GameObject IndividualObject = Instantiate(PlaceHolderObjects[i], Parent.transform);
				
				
				//setscale
				IndividualObject.transform.localScale = new Vector3(IndividualScale, IndividualScale, IndividualScale);
				
				//setlocation
				IndividualObject.transform.position = IndivisualBarPositions[i].transform.position;
                Debug.Log("ik heb je "+j+"ste blokje van type "+ (FoodType.Type)i+" geplaatst op positie "+ IndividualObject.transform.position);

                //updatePosition
                Ipos.position = new Vector3(IndivisualBarPositions[i].transform.position.x, Ipos.position.x, IndivisualBarPositions[i].transform.position.z);
				Debug.Log("en toen de volgende positie verdanderd met "+IndividualScale+" naar "+ IndivisualBarPositions[i].transform.position);


            }
		}
		Debug.Log("I have done the visualisation. And i have ...");

	}

}