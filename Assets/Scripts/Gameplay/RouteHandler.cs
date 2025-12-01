using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class RouteHandler : MonoBehaviour
{
    List<string> eventsList = new List<string>
    {
        "Muddy Road",
        "Heavy Rainfall",
        "Stormy Weather",
        "Snowfall",
        "Thick Fog",

        "Bandits",
        "Wolves",
        "Boars",
        "Horse Panic",
        "High Tolls",

        "Fallen Trees",
        "Cows",
        "Lost Way",
        "Poor Road Conditions",
        "Broken Wheel",
    };

    public class Route
    {
        public string RouteName { get; set; }
        public float TravelTime { get; set; }

        public Route(string routeName, float travelTime)
        {
            RouteName = routeName;
            TravelTime = travelTime;
        }
    }

    void Start()
    {
        print("RouteHandler started.");
    }

    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            print(DisplayRoutes());
        }
    }

    string GetRandomEvent()
    {
        int index = Random.Range(0, eventsList.Count);
        string randomEvent = eventsList[index];
        return randomEvent;
    }

    List<Route> GenerateRoutes()
    {
        float shortRouteTravelTime = Random.Range(2f, 4f);
        shortRouteTravelTime = Mathf.Round(shortRouteTravelTime * 4f) / 4f;

        float mediumRouteTravelTime = Random.Range(4, 6) * 4 % 1 / 4;
        float longRouteTravelTime = Random.Range(6, 8) * 4 % 1 / 4;

        Route shortRoute = new("Short Route", shortRouteTravelTime);
        Route mediumRoute = new("Medium Route", mediumRouteTravelTime);
        Route longRoute = new("Long Route", longRouteTravelTime);

        return new List<Route> { shortRoute, mediumRoute, longRoute };
    }

    string DisplayRoutes()
    {
        List<Route> routes = GenerateRoutes();
        string routeInfo = "Available Routes:\n";
        foreach (var route in routes)
        {
            routeInfo += $"{route.RouteName} - Travel Time: {route.TravelTime} hours\n";
        }
        return routeInfo;
    }
}
