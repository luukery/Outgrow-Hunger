using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class FoodEntry
{
    public string name;
    public Sprite icon;
    [Min(0)] public int minPrice = 1;
    [Min(0)] public int maxPrice = 10;
}

[System.Serializable]
public class StockItem
{
    public string name;
    public Sprite icon;
    public int price;
}

public class MarketStallSimple : MonoBehaviour
{
    [Header("Voedselpool voor deze kraam (stel per kraam in)")]
    public List<FoodEntry> foodPool = new();

    [Header("Instellingen")]
    [Range(1,2)] public int minItems = 1;
    [Range(1,2)] public int maxItems = 2;

    [Header("Huidige voorraad (read-only)")]
    public List<StockItem> currentStock = new();

    [Header("Events")]
    public UnityEvent onStockChanged;

    bool generatedThisSession = false;

    public void OpenStall()
    {
        if (!generatedThisSession)
        {
            GenerateStock();
            generatedThisSession = true;
        }
        onStockChanged?.Invoke();
    }

    public void RefreshStock()
    {
        GenerateStock();
        onStockChanged?.Invoke();
    }

    void GenerateStock()
    {
        currentStock.Clear();
        if (foodPool == null || foodPool.Count == 0)
        {
            Debug.LogWarning($"[MarketStallSimple] Geen foodPool ingesteld op {name}");
            return;
        }

        int count = Random.Range(minItems, maxItems + 1);

        // kies unieke items
        List<int> poolIdx = new List<int>();
        for (int i = 0; i < foodPool.Count; i++) poolIdx.Add(i);

        for (int i = 0; i < count && poolIdx.Count > 0; i++)
        {
            int pick = Random.Range(0, poolIdx.Count);
            var f = foodPool[poolIdx[pick]];
            poolIdx.RemoveAt(pick);

            int price = Random.Range(f.minPrice, f.maxPrice + 1);
            currentStock.Add(new StockItem { name = f.name, icon = f.icon, price = price });
        }
    }
}
