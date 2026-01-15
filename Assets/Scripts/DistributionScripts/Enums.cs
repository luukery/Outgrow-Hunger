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
    public List<Request> TotalOrder = new List<Request>();
    public List<Request> TotalDelivered = new List<Request>();

    public List<Request> Shortages = new List<Request>();
    public List<Request> Excesses = new List<Request>();

    public List<Request> NeedsShortage = new List<Request>();

    public string reaction = new string("default reaction");

}

public readonly struct CategoryConfig
{
    public readonly int MinNeedAmount;
    public readonly int MaxNeedAmount;

    public readonly int MinMoney;
    public readonly int MaxMoney;

    public readonly bool ExactMatch;
    public readonly bool QualityStrict;

    public readonly int MinOrderBuffer;
    public readonly int MaxOrderBuffer;

    public readonly int MinTotalItems;
    public readonly int MaxTotalItems;

    public readonly int MinDistinctItems;
    public readonly int MaxDistinctItems;

    public CategoryConfig(
        int minNeedAmount,
        int maxNeedAmount,
        int minMoney,
        int maxMoney,
        bool exactMatch,
        bool qualityStrict,
        int minOrderBuffer,
        int maxOrderBuffer,
        int minTotalItems,
        int maxTotalItems,
        int minDistinctItems,
        int maxDistinctItems)
    {
        MinNeedAmount = minNeedAmount;
        MaxNeedAmount = maxNeedAmount;
        MinMoney = minMoney;
        MaxMoney = maxMoney;
        ExactMatch = exactMatch;
        QualityStrict = qualityStrict;
        MinOrderBuffer = minOrderBuffer;
        MaxOrderBuffer = maxOrderBuffer;
        MinTotalItems = minTotalItems;
        MaxTotalItems = maxTotalItems;
        MinDistinctItems = minDistinctItems;
        MaxDistinctItems = maxDistinctItems;
    }
}

public enum NpcCategory
{
    Survival,
    Precautious,
    ExactNeed,
    PreferenceDriven
}
