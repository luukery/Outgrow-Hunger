using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MarketStallFromCatalog : MonoBehaviour
{
    [Header("Catalog")]
    public ProductCatalogSO catalog;

    [Header("Stall settings")]
    public StallType stallType = StallType.Bakery;
    [Range(1,2)] public int minItems = 1;
    [Range(1,2)] public int maxItems = 2;
    public bool useWeightedRandom = false;

    [Header("Visit persistence")]
    [Tooltip("Unieke ID per kraam; wordt automatisch gevuld als leeg.")]
    public string stallId;

    [Header("Current stock (read-only)")]
    public List<MarketStockItem> currentStock = new();

    [Header("Events")]
    public UnityEvent onStockChanged;

    [Header("Stall story (per kraam)")]
    [TextArea(3, 6)]
    public string stallStory;

    void OnValidate()
    {
        if (string.IsNullOrWhiteSpace(stallId))
            stallId = System.Guid.NewGuid().ToString();
    }

    public void OpenStall()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (!TryLoadFromCache(sceneName))
        {
            GenerateStock();
            MarketVisitCache.Set(sceneName, stallId, currentStock);
        }
        onStockChanged?.Invoke();
    }

    bool TryLoadFromCache(string scene)
    {
        if (MarketVisitCache.TryGet(scene, stallId, out var cached))
        {
            currentStock = new List<MarketStockItem>(cached);
            return true;
        }
        return false;
    }

    void GenerateStock()
    {
        currentStock.Clear();

        if (catalog == null)
        {
            Debug.LogWarning($"[MarketStall] Geen catalog gekoppeld op {name}");
            return;
        }

        var pool = catalog.GetPool(stallType);
        if (pool.Count == 0)
        {
            Debug.LogWarning($"[MarketStall] Geen producten voor type {stallType}");
            return;
        }

        int count = Random.Range(minItems, maxItems + 1);
        var picks = catalog.PickRandomUnique(pool, count, useWeightedRandom);

        foreach (var p in picks)
        {
            int price = Random.Range(p.minPrice, p.maxPrice + 1);
            currentStock.Add(new MarketStockItem { name = p.name, icon = p.icon, price = price });
            //currentStock.Add(new MarketStockItem { name = p.name, icon = p.icon, price = price, foodType = p.foodType, foodQuality = Food.Quality.Medium, size = 1 });

        }
    }
}
