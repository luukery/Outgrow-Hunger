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
    public List<Food> currentStock = new();

    [Header("Events")]
    public UnityEvent onStockChanged;

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
            currentStock = new List<Food>(cached);
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
            Debug.Log("My icon here has a name " + p.icon.name);

            int price = Random.Range(p.minPrice, p.maxPrice + 1);
            int typeNumber = (int) p.type;
            Food addingFood = new Food((FoodType.Type)typeNumber, 1, p.name, price, p.icon);
            Debug.Log($"[MarketStall] Toegevoegd aan voorraad: {addingFood.name} voor {addingFood.price} munten. Ook heb ik denk ik een icon want {(p.icon.name)} bestaat");
            currentStock.Add(addingFood);
        }
    }
}
