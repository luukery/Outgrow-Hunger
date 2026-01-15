using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class NPC : MonoBehaviour
{
    [SerializeField] private NpcCategory category;

    [SerializeField] private List<Request> Needs = new();
    [SerializeField] private List<Request> Order = new();

    public int Money;

    [SerializeField] public NpcDialogue dialogue;

    private readonly DeliveryResultService deliveryService = new();
    private System.Random rng;

    void OnEnable()
    {
        rng = new System.Random(Guid.NewGuid().GetHashCode());
        GenerateProfile();
    }

    public DeliveryResult Transaction(List<Request> playerInput)
    {
        return deliveryService.Transaction(Order, Needs, playerInput, dialogue);
    }
    public NpcInfoDTO GetInfoDTO()
    {
        List<Request> needsCopy = new(Needs.Count);
        List<Request> orderCopy = new(Order.Count);

        for (int i = 0; i < Needs.Count; i++)
        {
            Request r = Needs[i];
            needsCopy.Add(new Request(r.Amount, r.FoodType, r.Quality));
        }

        for (int i = 0; i < Order.Count; i++)
        {
            Request r = Order[i];
            orderCopy.Add(new Request(r.Amount, r.FoodType, r.Quality));
        }

        return new NpcInfoDTO
        {
            Needs = needsCopy,
            Order = orderCopy,
            Money = Money
        };
    }


    void GenerateProfile()
    {
        CategoryConfig config = GetCategoryConfig();
        GenerateNeeds(config);
        GenerateOrder(config);
        Money = RollMoney(config);
    }

    void GenerateNeeds(CategoryConfig config)
    {
        FoodType.Type[] foodTypes = (FoodType.Type[])Enum.GetValues(typeof(FoodType.Type));
        Food.Quality[] qualities = (Food.Quality[])Enum.GetValues(typeof(Food.Quality));

        List<FoodType.Type> availableTypes = foodTypes.ToList();

        int amountOfdistinctTypes = rng.Next(config.MinDistinctItems, config.MaxDistinctItems + 1);

        for (int i = 0; i < amountOfdistinctTypes; i++)
        {
            int type = rng.Next(0, availableTypes.Count);

            FoodType.Type selectedType = availableTypes[type];
            availableTypes.Remove(availableTypes[type]);

            int amount = rng.Next(config.MinNeedAmount, config.MaxNeedAmount + 1);
            Food.Quality quality = Food.Quality.Good;       //temporary

            Needs.Add(new Request(amount, selectedType, quality));
        }
    }

    void GenerateOrder(CategoryConfig config)
    {
        FoodType.Type[] foodTypes = (FoodType.Type[])Enum.GetValues(typeof(FoodType.Type));
        Food.Quality[] qualities = (Food.Quality[])Enum.GetValues(typeof(Food.Quality));

        List<FoodType.Type> availableTypes = foodTypes.ToList();

        int amountOfDistinctTypes = rng.Next(config.MinDistinctItems, config.MaxDistinctItems + 1);

        for (int i = 0; i < amountOfDistinctTypes; i++)
        {
            int index = rng.Next(availableTypes.Count);
            FoodType.Type selectedType = availableTypes[index];
            availableTypes.RemoveAt(index);

            Food.Quality quality = Food.Quality.Good; // temporary

            int amount = rng.Next(config.MinNeedAmount, config.MaxNeedAmount + 1);
            Order.Add(new Request(amount, selectedType, quality));
        }
    }
    //
    int RollMoney(CategoryConfig config)
    {
        return rng.Next(config.MinMoney, config.MaxMoney + 1);
    }



    CategoryConfig GetCategoryConfig()
    {
        category = (NpcCategory)rng.Next(
       0,
       Enum.GetValues(typeof(NpcCategory)).Length
   );

        switch (category)
        {
            case NpcCategory.Survival:
                return new CategoryConfig(
                    minNeedAmount: 1,
                    maxNeedAmount: 3,
                    minMoney: 0,
                    maxMoney: 5,
                    exactMatch: false,
                    qualityStrict: false,
                    minOrderBuffer: 0,
                    maxOrderBuffer: 1,
                    minTotalItems: 1,
                    maxTotalItems: 2,
                    minDistinctItems: 1,
                    maxDistinctItems: 2
                );

            case NpcCategory.Precautious:
                return new CategoryConfig(
                    minNeedAmount: 4,
                    maxNeedAmount: 6,
                    minMoney: 5,
                    maxMoney: 10,
                    exactMatch: false,
                    qualityStrict: false,
                    minOrderBuffer: 0,
                    maxOrderBuffer: 1,
                    minTotalItems: 4,
                    maxTotalItems: 6,
                    minDistinctItems: 2,
                    maxDistinctItems: 3
                );

            case NpcCategory.ExactNeed:
                return new CategoryConfig(
                    minNeedAmount: 4,
                    maxNeedAmount: 6,
                    minMoney: 8,
                    maxMoney: 12,
                    exactMatch: true,
                    qualityStrict: true,
                    minOrderBuffer: 0,
                    maxOrderBuffer: 0,
                    minTotalItems: 4,
                    maxTotalItems: 6,
                    minDistinctItems: 3,
                    maxDistinctItems: 4
                );

            case NpcCategory.PreferenceDriven:
                return new CategoryConfig(
                    minNeedAmount: 4,
                    maxNeedAmount: 6,
                    minMoney: 20,
                    maxMoney: 30,
                    exactMatch: false,
                    qualityStrict: true,
                    minOrderBuffer: 2,
                    maxOrderBuffer: 4,
                    minTotalItems: 6,
                    maxTotalItems: 10,
                    minDistinctItems: 4,
                    maxDistinctItems: 6
                );

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}