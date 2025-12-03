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
    public TMP_Text timePassedText;

    float timePassed = 0;

    
    [System.Serializable]
    public class RouteEvent
    {
        public string Name;
        public float ExtraTime;
        public int FoodLoss;    // unused for now
        public int GoldLoss;    // unused for now

        public RouteEvent(string name, float extraTime, int foodLoss = 0, int goldLoss = 0)
        {
            Name = name;
            ExtraTime = extraTime;
            FoodLoss = foodLoss;
            GoldLoss = goldLoss;
        }
    }

    List<RouteEvent> eventList = new List<RouteEvent>
    {
        // Forest / Nature 
        new("Fallen tree blocking passage", 0.5f),
        new("Tree roots shaking the carriage", 0.25f, 15),
        new("Sudden forest fire (smoke visible)", 1f),

        // Weather 
        new("Heavy rain", 0.5f),
        new("Snowfall", 0.5f, 10),
        new("Hail", 1f),
        new("Drizzle turning road to mud", 1.25f),
        new("Thick fog", 1f),
        new("Thunderstorm", 0.5f),
        new("Temperature drop causing icy patches", 0.5f),

        // Road & Terrain 
        new("Cows blocking the road", 1f),
        new("Caravan jam or abandoned cart", 0.75f),
        new("Sinkhole opening near the road", 0f, 15),
        new("Poor road maintenance", 0.5f),
        new("River crossing too deep", 0.75f),

        // Human Activity 
        new("Bandits attempting robbery", 0f, 15),
        new("Toll collectors demanding unfair toll", 0f, 0, 10),
        new("Desperate refugees begging for supplies", 0f, 5),

        // Wildlife 
        new("Grazing deer or elk", 0.75f),
        new("Swarm of insects spooking horses", 0.25f),
        new("Predators tracking the carriage", 0f, 10),

        // Geological / Landmark 
        new("Rockslide", 0.5f),
        new("Steep incline straining the horses", 0.5f),
        new("Stream deeper than expected", 0f, 5),
        new("Winds buffeting the carriage", 0f, 10),
    };

    
    public class Route
    {
        public string RouteName { get; set; }
        public float TravelTime { get; set; }
        public int EventChance { get; set; }
        public RouteEvent EventData { get; set; }

        public Route(string routeName, float travelTime, int eventChance, RouteEvent evt)
        {
            RouteName = routeName;
            TravelTime = travelTime;
            EventChance = eventChance;
            EventData = evt;
        }
    }

    List<Route> routes;

    
    void Start()
    {
        routeSetUp();
    }

    void Update()
    {
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            routes = GenerateRoutes();
            setUpButtons(routes);
        }
    }

    
    void routeSetUp()
    {
        routes = GenerateRoutes();
        setUpButtons(routes);
    }

    void setUpButtons(List<Route> routes)
    {
        // Clear listeners
        shortRouteButton.onClick.RemoveAllListeners();
        mediumRouteButton.onClick.RemoveAllListeners();
        longRouteButton.onClick.RemoveAllListeners();

        // Assign listeners
        shortRouteButton.onClick.AddListener(() => HandleRouteSelection(routes[0]));
        mediumRouteButton.onClick.AddListener(() => HandleRouteSelection(routes[1]));
        longRouteButton.onClick.AddListener(() => HandleRouteSelection(routes[2]));

        UpdateButtonText(shortRouteButton, routes[0]);
        UpdateButtonText(mediumRouteButton, routes[1]);
        UpdateButtonText(longRouteButton, routes[2]);

        resultText.text = "Choose a route!";
    }

    void UpdateButtonText(Button btn, Route route)
    {
        string eventText = route.EventData == null ?
            "Event: None" :
            $"Event: {route.EventData.Name} (+{route.EventData.ExtraTime}h)";

        btn.GetComponentInChildren<TMP_Text>().text =
            $"{route.RouteName}\n" +
            $"Time: {route.TravelTime}h\n" +
            $"Chance: {route.EventChance}%\n" +
            $"{eventText}";
    }


    void HandleRouteSelection(Route route)
    {
        float totalTime = route.TravelTime;

        if (route.EventData != null)
            totalTime += route.EventData.ExtraTime;

        timePassed += totalTime;

        // Event result text
        if (route.EventData == null)
            resultText.text = "No event happened.";
        else
            resultText.text = $"{route.EventData.Name}\n(+{route.EventData.ExtraTime}h)";

        timePassedText.text = $"Time Passed: {timePassed}h";

        routeSetUp();
    }


    RouteEvent GetRandomEvent(int eventChance)
    {
        int roll = Random.Range(1, 101);
        if (roll > eventChance)
            return null;

        return eventList[Random.Range(0, eventList.Count)];
    }


    List<Route> GenerateRoutes()
    {
        float shortRouteTime = Mathf.Round(Random.Range(2f, 4f) * 4f) / 4f;
        float mediumRouteTime = Mathf.Round(Random.Range(4f, 6f) * 4f) / 4f;
        float longRouteTime = Mathf.Round(Random.Range(6f, 8f) * 4f) / 4f;

        int shortChance = 75;
        int mediumChance = 55;
        int longChance = 35;

        Route shortRoute = new("Short Route", shortRouteTime, shortChance, GetRandomEvent(shortChance));
        Route mediumRoute = new("Medium Route", mediumRouteTime, mediumChance, GetRandomEvent(mediumChance));
        Route longRoute = new("Long Route", longRouteTime, longChance, GetRandomEvent(longChance));

        return new List<Route> { shortRoute, mediumRoute, longRoute };
    }
}
