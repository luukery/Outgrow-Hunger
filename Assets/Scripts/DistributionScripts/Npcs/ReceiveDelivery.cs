using System;
using System.Collections.Generic;

public class DeliveryResultService
{
    public DeliveryResult Transaction(List<Request> order, int npcMoney, List<Request> given)
    {
        DeliveryResult result = new DeliveryResult();
        result.NpcMoneyBefore = npcMoney;

        CalculateOrderResult(order, given, result);
        npcMoney = ProcessPayment(result, npcMoney);

        result.NpcMoneyAfter = npcMoney;
        result.PaymentShortfall = result.TotalPrice - result.AmountPaid;

        return result;
    }

    void CalculateOrderResult(List<Request> order, List<Request> given, DeliveryResult result)
    {
        for (int i = 0; i < order.Count; i++)
        {
            Request wanted = order[i];
            Request delivered = given.Find(r => r.FoodType == wanted.FoodType);

            int deliveredAmount = delivered != null ? delivered.Amount : 0;
            int paidAmount = Math.Min(deliveredAmount, wanted.Amount);

            result.TotalPrice += wanted.Amount;

            if (deliveredAmount < wanted.Amount)
            {
                int shortage = wanted.Amount - deliveredAmount;
                result.TotalFoodShortage += shortage;
                result.Shortages.Add(new Request(shortage, wanted.FoodType, wanted.Quality));
            }
            else if (deliveredAmount > wanted.Amount)
            {
                int excess = deliveredAmount - wanted.Amount;
                result.TotalFoodExcess += excess;
                result.Excesses.Add(new Request(excess, wanted.FoodType, wanted.Quality));
            }
        }
    }

    int ProcessPayment(DeliveryResult result, int npcMoney)
    {
        if (npcMoney >= result.TotalPrice)
        {
            result.AmountPaid = result.TotalPrice;
            return npcMoney - result.TotalPrice;
        }

        result.AmountPaid = npcMoney;
        return 0;
    }
}
