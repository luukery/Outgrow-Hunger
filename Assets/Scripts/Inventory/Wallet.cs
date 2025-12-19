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
    public bool CanSpendMoney(int amount)
    {
        if (amount > Money)
        {
            return false; // Not enough gold
        }
        Money -= amount;
        return true;
    }
}
