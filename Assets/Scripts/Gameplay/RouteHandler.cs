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
        public string EventText { get; set; }

        public Route(string routeName, float travelTime, int eventChance, string eventDescription = "No event yet")
        {
            RouteName = routeName;
            TravelTime = travelTime;
            EventChance = eventChance;
            EventText = eventDescription;
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
        resultText.text = route.EventText;
    }

    string GetRandomEvent(int eventChance)
    {
        int happenedRoll = Random.Range(1, 101);
        if (happenedRoll > eventChance)
        {
            return "no event happened";
        }

        float rarityRoll = Random.value;
        string rarity = rarityRoll < 0.55f ? "Common" : rarityRoll < 0.85f ? "Uncommon" : "Rare";

        int index = Random.Range(0, eventsList.Count);
        return $"{rarity}: {eventsList[index]}";
    }

    List<Route> GenerateRoutes()
    {
        float shortRouteTravelTime = Mathf.Round(Random.Range(2f, 4f) * 4f) / 4f;
        float mediumRouteTravelTime = Mathf.Round(Random.Range(4f, 6f) * 4f) / 4f;
        float longRouteTravelTime = Mathf.Round(Random.Range(6f, 8f) * 4f) / 4f;

        int shortRouteEventChance = 75;
        int mediumRouteEventChance = 55;
        int longRouteEventChance = 35;

        string shortRouteEvent = GetRandomEvent(shortRouteEventChance);
        string mediumRouteEvent = GetRandomEvent(mediumRouteEventChance);
        string longRouteEvent = GetRandomEvent(longRouteEventChance);

        Route shortRoute = new("Short Route", shortRouteTravelTime, shortRouteEventChance, shortRouteEvent);
        Route mediumRoute = new("Medium Route", mediumRouteTravelTime, mediumRouteEventChance, mediumRouteEvent);
        Route longRoute = new("Long Route", longRouteTravelTime, longRouteEventChance, longRouteEvent);

        return new List<Route> { shortRoute, mediumRoute, longRoute };
    }
}
