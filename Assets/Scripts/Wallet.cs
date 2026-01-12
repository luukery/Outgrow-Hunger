using UnityEngine;

public class Wallet : MonoBehaviour
{
    public static Wallet Instance { get; private set; }

    public int Money = 0;

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

    public void AddMoney(int amount)
    {
        Money += amount;
        Debug.Log("Money is added, total is now " + Money);
    }

    public bool CanSpendMoney(int amount)
    {
        if (amount > Money) return false;
        Money -= amount;
        return true;
    }

    public void RemoveMoneyClamped(int amount)
    {
        if (amount <= 0) return;
        Money = Mathf.Max(0, Money - amount);
    }
}
