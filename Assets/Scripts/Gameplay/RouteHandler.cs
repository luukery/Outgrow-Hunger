using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;

public class RouteHandler : MonoBehaviour
{
    [Header("UI References")]
    public Button shortRouteButton;
    public Button mediumRouteButton;
    public Button longRouteButton;
    public TMP_Text resultText;

    List<string> eventsList = new List<string>
    {
        "Muddy Road", "Heavy Rainfall", "Stormy Weather", "Snowfall", "Thick Fog",
        "Bandits", "Wolves", "Boars", "Horse Panic", "High Tolls",
        "Fallen Trees", "Cows", "Lost Way", "Poor Road Conditions", "Broken Wheel",
    };

    public class Route
    {
        public string RouteName { get; set; }
        public float TravelTime { get; set; }
        public int EventChance { get; set; }

        public Route(string routeName, float travelTime, int eventChance)
        {
            RouteName = routeName;
            TravelTime = travelTime;
            EventChance = eventChance;
        }
    }

    List<Route> routes;

    void Start()
    {
        // Generate the three routes when scene loads
        routes = GenerateRoutes();

        // Assign button listeners
        shortRouteButton.onClick.AddListener(() => HandleRouteSelection(routes[0]));
        mediumRouteButton.onClick.AddListener(() => HandleRouteSelection(routes[1]));
        longRouteButton.onClick.AddListener(() => HandleRouteSelection(routes[2]));

        // Set button labels
        shortRouteButton.GetComponentInChildren<TMP_Text>().text =
            $"{routes[0].RouteName}\nTime: {routes[0].TravelTime}h\nEvent: {routes[0].EventChance}%";

        mediumRouteButton.GetComponentInChildren<TMP_Text>().text =
            $"{routes[1].RouteName}\nTime: {routes[1].TravelTime}h\nEvent: {routes[1].EventChance}%";

        longRouteButton.GetComponentInChildren<TMP_Text>().text =
            $"{routes[2].RouteName}\nTime: {routes[2].TravelTime}h\nEvent: {routes[2].EventChance}%";


        resultText.text = "Choose a route!";
    }

    void HandleRouteSelection(Route route)
    {
        if (EventHappened(route))
        {
            resultText.text = $"EVENT HAPPENED!\n{GetRandomEvent()}";
        }
        else
        {
            resultText.text = "No event occurred. Safe travels!";
        }
    }

    string GetRandomEvent()
    {
        float roll = Random.value;
        string rarity = roll < 0.55f ? "Common" : roll < 0.85f ? "Uncommon" : "Rare";

        int index = Random.Range(0, eventsList.Count);
        return $"{rarity}: {eventsList[index]}";
    }

    List<Route> GenerateRoutes()
    {
        float shortRouteTravelTime = Mathf.Round(Random.Range(2f, 4f) * 4f) / 4f;
        float mediumRouteTravelTime = Mathf.Round(Random.Range(4f, 6f) * 4f) / 4f;
        float longRouteTravelTime = Mathf.Round(Random.Range(6f, 8f) * 4f) / 4f;

        int shortRouteEventChance = Random.Range(80, 91);
        int mediumRouteEventChance = Random.Range(60, 71);
        int longRouteEventChance = Random.Range(30, 51);

        Route shortRoute = new("Short Route", shortRouteTravelTime, shortRouteEventChance);
        Route mediumRoute = new("Medium Route", mediumRouteTravelTime, mediumRouteEventChance);
        Route longRoute = new("Long Route", longRouteTravelTime, longRouteEventChance);

        return new List<Route> { shortRoute, mediumRoute, longRoute };
    }

    bool EventHappened(Route route)
    {
        int roll = Random.Range(1, 101);
        return roll <= route.EventChance;
    }
}
