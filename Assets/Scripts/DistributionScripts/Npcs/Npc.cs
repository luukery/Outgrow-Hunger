using UnityEngine;
using System;
using System.Collections.Generic;

public class NPC : MonoBehaviour
{
    public List<Request> Needs = new List<Request>();
    public List<Request> Order = new List<Request>();

    public List<Request> testRequest = new List<Request>();
    public int Money;

    void Start()
    {
        GenerateProfile();
        GenerateTestRequest();
        DeliveryResult result = ReceiveDelivery(testRequest);
        DebugPrint(result);
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

    void GenerateTestRequest()
    {
        testRequest.Clear();

        for (int i = 0; i < Needs.Count; i++)
        {
            Request n = Needs[i];
            int deliveredAmount = n.Amount == 0 ? 1 : n.Amount + 1;
            testRequest.Add(new Request(deliveredAmount, n.FoodType, n.Quality));
        }
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

        for (int i = 0; i < Needs.Count; i++)
        {
            Request need = Needs[i];
            Request delivered = given.Find(r => r.FoodType == need.FoodType);
            int amount = delivered != null ? delivered.Amount : 0;

            if (amount < need.Amount)
            {
                int diff = need.Amount - amount;
                result.TotalFoodShortage += diff;
                result.Shortages.Add(new Request(diff, need.FoodType, need.Quality));
            }

            if (amount > need.Amount)
            {
                int diff = amount - need.Amount;
                result.TotalFoodExcess += diff;
                result.Excesses.Add(new Request(diff, need.FoodType, need.Quality));
            }

            result.TotalPrice += amount;
        }

        if (Money >= result.TotalPrice)
        {
            Money -= result.TotalPrice;
            result.AmountPaid = result.TotalPrice;
        }
        else
        {
            result.AmountPaid = Money;
            Money = 0;
        }

        result.PaymentShortfall = result.TotalPrice - result.AmountPaid;
        result.NpcMoneyAfter = Money;

        return result;
    }


    public void DebugPrint(DeliveryResult result)
    {
        Debug.Log("========== NPC PROFILE ==========");
        for (int i = 0; i < Needs.Count; i++)
        {
            Request need = Needs[i];
            Request order = Order[i];

            Debug.Log(
                $"{need.FoodType}  |  Need: {need.Amount}  |  Order: {order.Amount}  |  Quality: {need.Quality}"
            );
        }

        Debug.Log("========== DELIVERY RESULT ==========");

        Debug.Log(
            $"Total Price: {result.TotalPrice}\n" +
            $"Paid: {result.AmountPaid}\n" +
            $"Shortfall: {result.PaymentShortfall}\n" +
            $"NPC Money After: {result.NpcMoneyAfter}"
        );

        Debug.Log("----- Shortages -----");
        if (result.Shortages.Count == 0)
            Debug.Log("None");

        foreach (var s in result.Shortages)
            Debug.Log($"• {s.FoodType}  |  Amount Missing: {s.Amount}  |  Quality: {s.Quality}");

        Debug.Log("----- Excesses -----");
        if (result.Excesses.Count == 0)
            Debug.Log("None");

        foreach (var e in result.Excesses)
            Debug.Log($"• {e.FoodType}  |  Extra Given: {e.Amount}  |  Quality: {e.Quality}");

        Debug.Log("====================================");
    }

}
