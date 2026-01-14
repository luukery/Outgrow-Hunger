using System.Collections.Generic;
using UnityEngine;

// Types staan bij jou al als StallType (Bakery, Fish, ...)

[System.Serializable]
public class ProductDef
{
    public string name;
    public Sprite icon;                 // sleep je icoon hier in
    [Min(0)] public int minPrice = 1;
    [Min(0)] public int maxPrice = 10;
    public StallType type;
    [Min(1)] public int weight = 1;     // optioneel: kansgewicht

    [Header("Spoilage")]
    public bool isSpoilable = true;

    [Tooltip("How many transport-hours until fully spoiled (only counts RouteHandler travel time).")]
    [Min(0f)] public float spoilTimeInHours = 12f;
}

[CreateAssetMenu(menuName = "Market/Product Catalog", fileName = "ProductCatalog")]
public class ProductCatalogSO : ScriptableObject
{
    public List<ProductDef> products = new();

    public List<ProductDef> GetPool(StallType t)
    {
        var pool = new List<ProductDef>();
        foreach (var p in products) if (p.type == t) pool.Add(p);
        return pool;
    }

    public List<ProductDef> PickRandomUnique(List<ProductDef> pool, int count, bool weighted)
    {
        var result = new List<ProductDef>();
        if (pool == null || pool.Count == 0) return result;

        if (!weighted)
        {
            var temp = new List<ProductDef>(pool);
            for (int i = 0; i < count && temp.Count > 0; i++)
            {
                int idx = Random.Range(0, temp.Count);
                result.Add(temp[idx]);
                temp.RemoveAt(idx);
            }
            return result;
        }

        // gewogen selectie zonder herhaling
        var bag = new List<ProductDef>();
        foreach (var p in pool)
            for (int i = 0; i < Mathf.Max(1, p.weight); i++) bag.Add(p);

        var used = new HashSet<ProductDef>();
        while (result.Count < count && bag.Count > 0)
        {
            int idx = Random.Range(0, bag.Count);
            var pick = bag[idx];
            if (!used.Contains(pick))
            {
                result.Add(pick);
                used.Add(pick);
                bag.RemoveAll(x => x == pick);
            }
        }
        return result;
    }
}
