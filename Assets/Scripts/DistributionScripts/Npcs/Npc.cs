using UnityEngine;
using System;
using System.Collections.Generic;

public class NPC : MonoBehaviour
{
    public List<Request> Needs = new List<Request>();
    public List<Request> Order = new List<Request>();
    public int Money;

    void Start()
    {
        GenerateProfile();
        DebugPrint();
    }

    void GenerateProfile()
    {
        Needs.Clear();
        Order.Clear();

        System.Random rng = new System.Random();

        Array foodTypes = Enum.GetValues(typeof(FoodType.Type));
        Array qualities = Enum.GetValues(typeof(Food.Quality));

        foreach (FoodType.Type type in foodTypes)
        {
            Food.Quality quality = (Food.Quality)qualities.GetValue(rng.Next(qualities.Length));
            int needAmount = rng.Next(0, 4);

            Request need = new Request(needAmount, type, quality);
            Request order = new Request(needAmount, type, quality);

            Needs.Add(need);
            Order.Add(order);
        }

        Money = rng.Next(0, 21);
    }

    public NpcInfoDTO GetInfoDTO()
    {
        NpcInfoDTO dto = new NpcInfoDTO();

        List<Request> needsCopy = new List<Request>();
        for (int i = 0; i < Needs.Count; i++)
        {
            Request n = Needs[i];
            needsCopy.Add(new Request(n.Amount, n.FoodType, n.Quality));
        }

        List<Request> orderCopy = new List<Request>();
        for (int i = 0; i < Order.Count; i++)
        {
            Request o = Order[i];
            orderCopy.Add(new Request(o.Amount, o.FoodType, o.Quality));
        }

        dto.Needs = needsCopy;
        dto.Order = orderCopy;
        dto.Money = Money;

        return dto;
    }


    public DeliveryResult ReceiveDelivery(List<Request> given)
    {
        DeliveryResult result = new DeliveryResult();
        result.MoneyBefore = Money;

        for (int i = 0; i < Needs.Count; i++)
        {
            Request need = Needs[i];
            Request delivered = given.Find(r => r.FoodType == need.FoodType);

            int amount = delivered != null ? delivered.Amount : 0;

            if (amount < need.Amount)
                result.Shortage += need.Amount - amount;

            if (amount > need.Amount)
                result.Over += amount - need.Amount;

            result.TotalCost += amount;
        }

        if (Money >= result.TotalCost)
        {
            Money -= result.TotalCost;
            result.Paid = result.TotalCost;
        }
        else
        {
            result.Paid = Money;
            Money = 0;
        }

        result.MoneyAfter = Money;

        return result;
    }


    public void DebugPrint()
    {
        for (int i = 0; i < Needs.Count; i++)
        {
            Request need = Needs[i];
            Request order = Order[i];

            Debug.Log(need.FoodType + " | Need: " + need.Amount + " | Order: " + order.Amount + " | Quality: " + need.Quality);
        }
    }
}
