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
        // Generate travel times rounded to the nearest 0.25
        float shortRouteTravelTime = Mathf.Round(Random.Range(2f, 4f) * 4f) / 4f;

        float mediumRouteTravelTime = Random.Range(4f, 6f);
        mediumRouteTravelTime = Mathf.Round(mediumRouteTravelTime * 4f) / 4f;

        float longRouteTravelTime = Random.Range(6, 8) * 4 % 1 / 4;
        longRouteTravelTime = Mathf.Round(longRouteTravelTime * 4f) / 4f;

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
