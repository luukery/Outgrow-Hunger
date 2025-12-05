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
    public int Shortage;
    public int Over;
    public int TotalCost;
    public int Paid;
    public int MoneyBefore;
    public int MoneyAfter;
}
