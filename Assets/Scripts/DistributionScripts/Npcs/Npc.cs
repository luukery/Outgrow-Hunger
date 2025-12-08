using UnityEngine;
using System;
using System.Collections.Generic;

public class NPC : MonoBehaviour
{
    public List<Request> Needs = new List<Request>();
    public List<Request> Order = new List<Request>();
    public List<Request> testRequest = new List<Request>();

    public int Money;

    ReceiveDelivery deliveryService = new ReceiveDelivery();

    void Start()
    {
        GenerateProfile();

        //GenerateTestRequest();
       // DeliveryResult result = Transaction(Order, Money, testRequest);       //for testing
      //  DebugPrint(result);
    }

    public DeliveryResult Transaction(List<Request> playerInput )
    {
        DeliveryResult result = deliveryService.Transaction(Order, Money, playerInput);
        return result;

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
            int amount = rng.Next(0, 4);

            Needs.Add(new Request(amount, type, quality));      //for now the same, can add iterantions later
            Order.Add(new Request(amount, type, quality));      
        }

        Money = rng.Next(0, 21);
    }

    void GenerateTestRequest()
    {
        testRequest.Clear();

        System.Random rng = new System.Random();
        Array qualities = Enum.GetValues(typeof(Food.Quality));

        for (int i = 0; i < Order.Count; i++)
        {
            Request o = Order[i];

            int roll = rng.Next(3);

            int deliveredAmount =
                roll == 0 ? Math.Max(0, o.Amount - rng.Next(1, 3)) :       
                roll == 1 ? o.Amount :                                     
                            o.Amount + rng.Next(1, 3);                    

            Food.Quality deliveredQuality =
                (Food.Quality)qualities.GetValue(rng.Next(qualities.Length));

            testRequest.Add(new Request(deliveredAmount, o.FoodType, deliveredQuality));
        }
    }



    public NpcInfoDTO GetInfoDTO()
    {
        NpcInfoDTO dto = new NpcInfoDTO
        {
            Needs = CopyRequests(Needs),
            Order = CopyRequests(Order),
            Money = Money
        };

        return dto;
    }

    List<Request> CopyRequests(List<Request> source)
    {
        List<Request> result = new List<Request>();

        for (int i = 0; i < source.Count; i++)
        {
            Request r = source[i];
            result.Add(new Request(r.Amount, r.FoodType, r.Quality));
        }

        return result;
    }

    public void DebugPrint(DeliveryResult result)
    {
        Debug.Log("========== NPC PROFILE ==========");

        Debug.Log($"Money Before: {result.NpcMoneyBefore}");
        Debug.Log($"Money After: {result.NpcMoneyAfter}");

        for (int i = 0; i < Needs.Count; i++)
        {
            Request need = Needs[i];
            Request order = Order[i];
            Debug.Log($"{need.FoodType} | Need: {need.Amount} | Order: {order.Amount} | Quality: {need.Quality}");
        }

        Debug.Log("========== DELIVERY RESULT ==========");
        Debug.Log(
            $"Total Price: {result.TotalPrice}\n" +
            $"Paid: {result.AmountPaid}\n" +
            $"Unpaid: {result.PaymentShortfall}"
        );

        Debug.Log("----- Shortages -----");
        if (result.Shortages.Count == 0) Debug.Log("None");
        else
            for (int i = 0; i < result.Shortages.Count; i++)
            {
                Request s = result.Shortages[i];
                Debug.Log($"{s.FoodType} | Missing: {s.Amount} | Quality: {s.Quality}");
            }

        Debug.Log("----- Excess (Overdelivery + Unpaid) -----");
        if (result.Excesses.Count == 0) Debug.Log("None");
        else
            for (int i = 0; i < result.Excesses.Count; i++)
            {
                Request e = result.Excesses[i];
                Debug.Log($"{e.FoodType} | Excess: {e.Amount} | Quality: {e.Quality}");
            }

        Debug.Log("====================================");
    }
}
