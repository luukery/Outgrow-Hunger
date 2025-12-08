using System.Collections.Generic;

public enum FoodTypes
{
    WATER,
    BREAD,
    MEAT,
    CANNEDMEAT
}

public class NpcInfoDTO
{
    public List<Request> Needs;
    public List<Request> Order;
    public int Money;
}

public class DeliveryResult
{
    public List<Request> Shortages = new List<Request>();
    public List<Request> Excesses = new List<Request>();

    public int TotalFoodShortage;
    public int TotalFoodExcess;

    public int TotalPrice;
    public int AmountPaid;
    public int PaymentShortfall;

    public int NpcMoneyBefore;
    public int NpcMoneyAfter;
}



