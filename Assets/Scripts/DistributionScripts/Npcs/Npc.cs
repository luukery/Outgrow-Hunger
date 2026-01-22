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
    int paysPerItem;

    [SerializeField] public NpcDialogue dialogue;
    [SerializeField] private ProductCatalogSO catalog;

    private readonly DeliveryResultService deliveryService = new();
    private System.Random rng;

    public AnimationClip idle;
    public AnimationClip walk;

    public string name;
    public string orderTalk;

    [HideInInspector] public bool hasRetried;
    void OnEnable()
    {
        rng = new System.Random(Guid.NewGuid().GetHashCode());
        GenerateProfile();
    }

    public DeliveryResult Transaction(List<Request> playerInput, int askedAmount)
    {
        if(FairPrice(playerInput, askedAmount))
        {
            return deliveryService.Transaction(Order, Needs, playerInput, dialogue);
        }

        DeliveryResult wrong = new DeliveryResult();
        wrong.reaction = "I ain't paying for this";
        return wrong;

    }

    public bool FairPrice(List<Request> playerInput, int askedAmount)
    {
        int givenAmount = 0;
        for (int i = 0; i < playerInput.Count; i++)
        {
            givenAmount += playerInput[i].Amount;
        }

        int itemCap = givenAmount * paysPerItem;
        int finalMaxSpend = Mathf.Min(Money, itemCap);

        return askedAmount <= finalMaxSpend;
    }

    public void SetCatalog(ProductCatalogSO newCatalog)
    {
        catalog = newCatalog;
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
        List<FoodType.Type> availableTypes = (catalog != null)
            ? catalog.GetAvailableFoodTypes()
            : new List<FoodType.Type>((FoodType.Type[])Enum.GetValues(typeof(FoodType.Type)));

        int totalItems = rng.Next(config.MinTotalItems, config.MaxTotalItems + 1);

        int distinctTypes = rng.Next(
            config.MinDistinctItems,
            Mathf.Min(config.MaxDistinctItems, availableTypes.Count) + 1
        );

        availableTypes = availableTypes.OrderBy(x => rng.Next()).ToList();

        int remaining = totalItems;

        for (int i = 0; i < distinctTypes; i++)
        {
            FoodType.Type type = availableTypes[i];

            int amount;

            if (i == distinctTypes - 1)
            {
                amount = remaining;
            }
            else
            {
                int maxForThisType = remaining - (distinctTypes - i - 1);
                amount = rng.Next(1, maxForThisType + 1);
            }

            remaining -= amount;

            Needs.Add(new Request(amount, type, Food.Quality.Good));
        }
    }

    void GenerateOrder(CategoryConfig config)
    {
        Order.Clear();

        List<FoodType.Type> availableTypes = (catalog != null)
            ? catalog.GetAvailableFoodTypes()
            : new List<FoodType.Type>((FoodType.Type[])Enum.GetValues(typeof(FoodType.Type)));

        int totalItems = rng.Next(config.MinTotalItems, config.MaxTotalItems + 1);

        int maxDistinctAllowed = Mathf.Min(
            config.MaxDistinctItems,
            availableTypes.Count,
            totalItems
        );

        if (maxDistinctAllowed <= 0)
        {
            return;
        }

        int minDistinct = Mathf.Min(config.MinDistinctItems, maxDistinctAllowed);
        int distinctTypes = rng.Next(minDistinct, maxDistinctAllowed + 1);

        availableTypes = availableTypes.OrderBy(x => rng.Next()).ToList();

        int remaining = totalItems;

        for (int i = 0; i < distinctTypes; i++)
        {
            FoodType.Type type = availableTypes[i];

            int amount;

            if (i == distinctTypes - 1)
            {
                amount = remaining;
            }
            else
            {
                int maxForThisType = remaining - (distinctTypes - i - 1);
                amount = rng.Next(1, maxForThisType + 1);
            }

            remaining -= amount;

            if (amount > 0)
            {
                Order.Add(new Request(amount, type, Food.Quality.Good));
            }
        }
    }



    int RollMoney(CategoryConfig config)
    {
        return rng.Next(config.MinMoney, config.MaxMoney + 1);
    }



    CategoryConfig GetCategoryConfig()
    {
        switch (category)
        {
            case NpcCategory.Survival:
                paysPerItem = 3;
                return new CategoryConfig(
                    minNeedAmount: 1,
                    maxNeedAmount: 3,
                    minMoney: 2,
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
                paysPerItem = 4;
                return new CategoryConfig(
                    minNeedAmount: 4,
                    maxNeedAmount: 6,
                    minMoney: 5,
                    maxMoney: 10,
                    exactMatch: false,
                    qualityStrict: false,
                    minOrderBuffer: 0,
                    maxOrderBuffer: 1,
                    minTotalItems: 2,
                    maxTotalItems: 4,
                    minDistinctItems: 1,
                    maxDistinctItems: 3
                );

            case NpcCategory.ExactNeed:
                paysPerItem = 5;
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
                    maxTotalItems: 5,
                    minDistinctItems: 2,
                    maxDistinctItems: 3
                );

            case NpcCategory.PreferenceDriven:
                paysPerItem = 6;
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
                    minDistinctItems: 2,
                    maxDistinctItems: 4
                );

            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}