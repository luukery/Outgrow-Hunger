using System;
using UnityEngine;

public class Wallet : MonoBehaviour
{
    public int Money = 0;


    public void AddMoney(int amount)
    {
        Money += amount;
        Debug.Log("Money is added, total is now " + Money);
    }

    private void RemoveMoney(int amount)
    {
        Money -= amount;
        Debug.Log("Money is removed, total is now " + Money);
    }
    public bool TrySpendMoney(int amount)
    {
        if (amount > Money)
        {
            return false; // Not enough gold
        }
        RemoveMoney(amount);
        return true;
    }
}
