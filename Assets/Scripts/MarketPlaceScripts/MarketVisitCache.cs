using System.Collections.Generic;

public static class MarketVisitCache
{
    // key: sceneName + "::" + stallId
    static readonly Dictionary<string, List<MarketStockItem>> cache = new();

    static string Key(string sceneName, string stallId) => sceneName + "::" + stallId;

    public static bool TryGet(string sceneName, string stallId, out List<MarketStockItem> stock)
        => cache.TryGetValue(Key(sceneName, stallId), out stock);

    public static void Set(string sceneName, string stallId, List<MarketStockItem> stock)
        => cache[Key(sceneName, stallId)] = new List<MarketStockItem>(stock); // kopie

    public static void ClearForScene(string sceneName)
    {
        var keys = new List<string>(cache.Keys);
        foreach (var k in keys)
            if (k.StartsWith(sceneName + "::"))
                cache.Remove(k);
    }

    public static void ClearAll() => cache.Clear();
}
