using System;
using System.Collections.Generic;

public class DeliveryResultService
{
    public DeliveryResult Transaction(List<Request> order, int npcMoney, List<Request> given)
    {
        DeliveryResult result = new DeliveryResult();
        result.NpcMoneyBefore = npcMoney;

        CalculateShortageAndOverDelivery(order, given, result);
        npcMoney = ProcessPayment(order, given, npcMoney, result);

        result.PaymentShortfall = result.TotalPrice - result.AmountPaid;
        result.NpcMoneyAfter = npcMoney;

        return result;
    }

    void CalculateShortageAndOverDelivery(List<Request> order, List<Request> given, DeliveryResult result)
    {
        for (int i = 0; i < order.Count; i++)
        {
            Request wanted = order[i];
            Request delivered = given.Find(r => r.FoodType == wanted.FoodType);
            int amount = delivered != null ? delivered.Amount : 0;

            if (amount < wanted.Amount)
            {
                int diff = wanted.Amount - amount;
                result.TotalFoodShortage += diff;
                result.Shortages.Add(new Request(diff, wanted.FoodType, wanted.Quality));
            }
            else if (amount > wanted.Amount)
            {
                int diff = amount - wanted.Amount;
                result.Excesses.Add(new Request(diff, wanted.FoodType, wanted.Quality));
                result.TotalFoodExcess += diff;
            }

            result.TotalPrice += amount;
        }
    }

    int ProcessPayment(List<Request> order, List<Request> given, int npcMoney, DeliveryResult result)
    {
        if (npcMoney >= result.TotalPrice)
        {
            result.AmountPaid = result.TotalPrice;
            return npcMoney - result.TotalPrice;
        }

        result.AmountPaid = npcMoney;
        int unpaid = result.TotalPrice - npcMoney;

        for (int i = 0; i < order.Count; i++)
        {
            Request wanted = order[i];
            Request delivered = given.Find(r => r.FoodType == wanted.FoodType);

            if (delivered == null || delivered.Amount == 0)
                continue;

            int unpaidPart = Math.Min(unpaid, delivered.Amount);

            if (unpaidPart > 0)
            {
                result.Excesses.Add(new Request(unpaidPart, wanted.FoodType, wanted.Quality));
                result.TotalFoodExcess += unpaidPart;
                unpaid -= unpaidPart;
            }

            if (unpaid <= 0)
                break;
        }

        return 0;
    }
}
