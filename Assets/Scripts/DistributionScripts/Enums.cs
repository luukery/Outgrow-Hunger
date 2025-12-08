public enum FoodTypes
{
    WATER,
    BREAD,
    MEAT,
    CANNEDMEAT
}

public struct DeliveryResult
{
    public int Shortage;
    public int Over;
    public bool CanPay;
}
