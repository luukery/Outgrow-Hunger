public enum LoadingMode
{
    Default,
    TransportMap
}

public static class LoadingContext
{
    public static LoadingMode Mode = LoadingMode.Default;
    public static int LegIndex = 0; // used only for TransportMap
}
