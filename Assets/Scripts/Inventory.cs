using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance { get; private set; }

    public List<Food> foods = new List<Food>();
    public InventoryVisualiser vis;

    public int maxCapacity;
    public int currentCapacity;

    [Header("Debug")]
    public bool debugSpoilageLogs = false;

    /// <summary> Fired when an item spoils and is removed. </summary>
    public event Action<Food, int> OnFoodSpoiledAndLost; // (food, unitsLost)

    /// <summary> Fired whenever inventory contents change (add/remove/spoil). </summary>
    public event Action OnInventoryChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public int GetTotalFoodUnits()
    {
        int total = 0;
        if (foods != null)
        {
            foreach (var f in foods)
            {
                if (f != null)
                    total += Mathf.Max(0, f.size);
            }
        }
        return total;
    }

    /// <summary>
    /// ONLY call this from transport-time (RouteHandler) via SpoilageManager.
    /// Returns how many FOOD UNITS were lost due to spoilage.
    /// </summary>
    public int AdvanceTime(float travelHours)
    {
        if (travelHours <= 0f) return 0;
        if (foods == null || foods.Count == 0) return 0;

        int spoiledUnitsTotal = 0;
        bool changed = false;

        // Iterate backwards because we might remove.
        for (int i = foods.Count - 1; i >= 0; i--)
        {
            var f = foods[i];
            if (f == null)
            {
                foods.RemoveAt(i);
                changed = true;
                continue;
            }

            bool spoiledNow = f.AdvanceSpoilage(travelHours);
            if (spoiledNow)
            {
                // Lose it: remove entire stack
                int lostUnits = Mathf.Max(0, f.size);
                spoiledUnitsTotal += lostUnits;

                currentCapacity = Mathf.Max(0, currentCapacity - lostUnits);
                foods.RemoveAt(i);
                changed = true;

                if (debugSpoilageLogs)
                    Debug.Log($"[Inventory] Spoiled & removed: {f.name} ({lostUnits} units) after +{travelHours}h travel.");

                OnFoodSpoiledAndLost?.Invoke(f, lostUnits);
            }
        }

        if (changed)
            NotifyChanged();

        return spoiledUnitsTotal;
    }

    void NotifyChanged()
    {
        OnInventoryChanged?.Invoke();

        // If your visualiser has a specific refresh method, call it here.
        // If not, this is still useful for any UI script you write later.
        // (Don’t guess method names, keep it safe.)
    }

    // Removes from ANY foods (not by type). Returns true if it removed the full requested amount.
    public bool TryRemoveFoodUnits(int amount)
    {
        if (amount <= 0) return true;

        int available = GetTotalFoodUnits();
        int toRemove = Mathf.Min(amount, available);
        bool changed = false;

        for (int i = foods.Count - 1; i >= 0 && toRemove > 0; i--)
        {
            var f = foods[i];
            if (f == null)
            {
                foods.RemoveAt(i);
                changed = true;
                continue;
            }

            int take = Mathf.Min(f.size, toRemove);
            f.size -= take;
            toRemove -= take;
            currentCapacity = Mathf.Max(0, currentCapacity - take);
            changed = true;

            if (f.size <= 0)
                foods.RemoveAt(i);
        }

        if (changed)
            NotifyChanged();

        return available >= amount;
    }

    // ✅ TYPE ONLY: how many units of this category do we have (ignores quality)
    public int GetAvailableUnits(FoodType.Type type)
    {
        int total = 0;
        if (foods == null) return 0;

        for (int i = 0; i < foods.Count; i++)
        {
            var f = foods[i];
            if (f == null) continue;
            if (f.foodType == type)
                total += Mathf.Max(0, f.size);
        }
        return total;
    }

    // ✅ TYPE ONLY: remove up to amount from this category (ignores quality)
    // returns how many units were actually removed
    public int RemoveUnits(FoodType.Type type, int amount)
    {
        if (amount <= 0) return 0;
        if (foods == null || foods.Count == 0) return 0;

        int remaining = amount;
        int removed = 0;
        bool changed = false;

        for (int i = foods.Count - 1; i >= 0 && remaining > 0; i--)
        {
            var f = foods[i];
            if (f == null)
            {
                foods.RemoveAt(i);
                changed = true;
                continue;
            }

            if (f.foodType != type) continue;

            int take = Mathf.Min(f.size, remaining);
            f.size -= take;

            remaining -= take;
            removed += take;

            currentCapacity = Mathf.Max(0, currentCapacity - take);
            changed = true;

            if (f.size <= 0)
                foods.RemoveAt(i);
        }

        if (changed)
            NotifyChanged();

        return removed;
    }

    public bool TryAddFoodToInventory(Food food)
    {
        if (food == null) return false;

        if ((currentCapacity + food.size) > maxCapacity)
            return false;

        foods.Add(food);
        currentCapacity += food.size;

        NotifyChanged();
        return true;
    }

    public bool TryRemoveFoodFromInventory(Food food)
    {
        if (food == null) return false;
        if (!foods.Contains(food)) return false;
        if (currentCapacity - food.size < 0) return false;

        foods.Remove(food);
        currentCapacity -= food.size;

        NotifyChanged();
        return true;
    }

    public List<Food> GetFoodsByType(FoodType.Type type)
    {
        return foods.FindAll(f => f != null && f.foodType == type);
    }

    public List<Food> GetFoodsByQuality(Food.Quality quality)
    {
        return foods.FindAll(f => f != null && f.foodQuality == quality);
    }

    // If you still need this later, re-add it with your real Request structure.
    public List<Request> CanSatisfyOrder(List<Request> orderByNPC)
    {
        return orderByNPC;
    }
}
