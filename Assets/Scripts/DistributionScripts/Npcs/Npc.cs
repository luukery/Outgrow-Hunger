using UnityEngine;
using System;
using System.Collections.Generic;

public class NPC : MonoBehaviour
{
    [SerializeField]
    public Dictionary<FoodTypes, int> Request = new Dictionary<FoodTypes, int>();
    [SerializeField]
    public Dictionary<FoodTypes, int> Needs = new Dictionary<FoodTypes, int>();

    public int money;

    [Range(0f, 1f)]
    public float OverRequestChance = 0.125f;

    void Start()
    {
        GenerateProfile();
        DebugPrint();
    }

    void GenerateProfile()
    {
        Array types = Enum.GetValues(typeof(FoodTypes));
        System.Random rng = new System.Random();
        int extramoney = 0;

        foreach (FoodTypes type in types)
        {
            int need = rng.Next(0, 4);
            Needs[type] = need;

            int request = need;

            if (rng.NextDouble() < 0.125f)
            {
                request += rng.Next(1, 3);
                extramoney = 10;
            }

            Request[type] = request;
        }


        money =  extramoney + rng.Next(0, 21);
    }

    public DeliveryResult HandleDelivery(Dictionary<FoodTypes, int> given)
    {
        DeliveryResult result = new DeliveryResult();
        int cost = 0;

        foreach (KeyValuePair<FoodTypes, int> entry in Needs)
        {
            FoodTypes type = entry.Key;
            int need = entry.Value;

            int amountGiven = given.ContainsKey(type) ? given[type] : 0;
            int requested = Request[type];

            if (amountGiven < need)
                result.Shortage += need - amountGiven;

            if (amountGiven > need)
                result.Over += amountGiven - need;

            cost += amountGiven;
        }

        result.CanPay = money > cost;
        return result;
    }

    public void DebugPrint()
    {
        foreach (KeyValuePair<FoodTypes, int> entry in Needs)
            Debug.Log("Need: " + entry.Key + " = " + entry.Value);

        foreach (KeyValuePair<FoodTypes, int> entry in Request)
            Debug.Log("Request: " + entry.Key + " = " + entry.Value);
    }

}
