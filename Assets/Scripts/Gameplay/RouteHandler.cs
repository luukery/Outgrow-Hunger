using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;

public class RouteHandler : MonoBehaviour
{
    [Header("UI References (Optional — leave empty if using roads instead of buttons)")]
    public Button shortRouteButton;
    public Button mediumRouteButton;
    public Button longRouteButton;
    public TMP_Text resultText;
    public TMP_Text timePassedText;

    public static RouteHandler Instance { get; private set; }

    float timePassed = 0f;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // ------------------ ROUTE EVENT CLASS ------------------
    [System.Serializable]
    public class RouteEvent
    {
        public string Name;
        public float ExtraTime;
        public int FoodLoss;
        public int GoldLoss;

        public RouteEvent(string name, float extraTime, int foodLoss = 0, int goldLoss = 0)
        {
            Name = name;
            ExtraTime = extraTime;
            FoodLoss = foodLoss;
            GoldLoss = goldLoss;
        }
    }

    List<RouteEvent> commonEvents = new List<RouteEvent>
    {
        new("Fallen tree blocking passage", 0.5f),
        new("Tree roots shaking the carriage", 0.25f, 15), 
        new("Heavy rain", 0.5f),
        new("Drizzle turning road to mud", 1.25f),
        new("Thick fog", 1f),
        new("Cows blocking the road", 1f),
        new("Poor road maintenance", 0.5f),
        new("Grazing deer or elk", 0.75f),
        new("Swarm of insects spooking horses", 0.25f),
        new("Winds buffeting the carriage", 0f, 10),

    };

    List<RouteEvent> uncommonEvents = new List<RouteEvent>
    {

        new("Sudden forest fire (smoke visible)", 1f),
        new("Snowfall", 0.5f, 10),
        new("Thunderstorm", 0.5f),
        new("Caravan jam or abandoned cart", 0.75f),
        new("River crossing too deep", 0.75f),
        new("Toll collectors demanding unfair toll", 0f, 0, 10),
        new("Desperate refugees begging for supplies", 0f, 5),
        new("Predators tracking the carriage", 0f, 10),
        new("Steep incline straining the horses", 0.5f),

    };

    List<RouteEvent> rareEvents = new List<RouteEvent>
    {
        new("Hail", 1f),
        new("Temperature drop causing icy patches", 0.5f),
        new("Sinkhole opening near the road", 0f, 15),
        new("Bandits attempting robbery", 0f, 15),
        new("Rockslide", 0.5f),
        new("Stream deeper than expected", 0f, 5),

    };

    // ------------------ ROUTE CLASS ------------------
    public class Route
    {
        public string RouteName;
        public float TravelTime;
        public int EventChance;
        public RouteEvent EventData;

        public Route(string routeName, float travelTime, int eventChance, RouteEvent evt)
        {
            RouteName = routeName;
            TravelTime = travelTime;
            EventChance = eventChance;
            EventData = evt;
        }
    }

    
    void Start()
    {
        RouteSetUp();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            RouteSetUp();
        }
    }

    // ------------------ MAIN SETUP ------------------
    void RouteSetUp()
    {
        List<Route> routes = GenerateRoutes();
        SetUpButtons(routes);
    }

    // ------------------ BUTTON SETUP ------------------
    void SetUpButtons(List<Route> routes)
    {
        bool usingButtons =
            shortRouteButton != null ||
            mediumRouteButton != null ||
            longRouteButton != null;

        if (!usingButtons)
            return; // you are using roads, not buttons — skip everything

        // SHORT BUTTON
        if (shortRouteButton != null)
        {
            shortRouteButton.onClick.RemoveAllListeners();
            shortRouteButton.onClick.AddListener(() => HandleRouteSelection(routes[0]));
            UpdateButtonText(shortRouteButton, routes[0]);
        }

        // MEDIUM BUTTON
        if (mediumRouteButton != null)
        {
            mediumRouteButton.onClick.RemoveAllListeners();
            mediumRouteButton.onClick.AddListener(() => HandleRouteSelection(routes[1]));
            UpdateButtonText(mediumRouteButton, routes[1]);
        }

        // LONG BUTTON
        if (longRouteButton != null)
        {
            longRouteButton.onClick.RemoveAllListeners();
            longRouteButton.onClick.AddListener(() => HandleRouteSelection(routes[2]));
            UpdateButtonText(longRouteButton, routes[2]);
        }

        if (resultText != null)
            resultText.text = "Choose a route!";
    }

    void UpdateButtonText(Button btn, Route route)
    {
        if (btn == null) return;

        TMP_Text tmp = btn.GetComponentInChildren<TMP_Text>();
        if (tmp == null) return;

        string eventText = route.EventData == null ?
            "Event: None" :
            $"Event: {route.EventData.Name} (+{route.EventData.ExtraTime}h)";

        tmp.text =
            $"{route.RouteName}\n" +
            $"Time: {route.TravelTime}h\n" +
            $"Chance: {route.EventChance}%\n" +
            $"{eventText}";
    }

    // ------------------ ROUTE SELECTION ------------------
    public void SelectRouteByIndex(int index)
    {
        if (routes == null || routes.Count <= index)
        {
            Debug.LogWarning("Route index out of range.");
            return;
        }

        HandleRouteSelection(routes[index]);
    }

    void HandleRouteSelection(Route route)
    {
        float totalTime = route.TravelTime;

        if (route.EventData != null)
            totalTime += route.EventData.ExtraTime;

        timePassed += totalTime;

        // Update UI only if it exists
        if (resultText != null)
        {
            if (route.EventData == null)
                resultText.text = "No event happened.";
            else
                resultText.text = $"{route.EventData.Name}\n(+{route.EventData.ExtraTime}h)";
        }

        if (timePassedText != null)
            timePassedText.text = $"Time Passed: {timePassed}h";

        RouteSetUp();
    }

    // ------------------ RANDOM EVENT GENERATION ------------------
    RouteEvent GetRandomEvent(int eventChance)
    {
        int roll = Random.Range(1, 101);
        if (roll > eventChance) return null;

        List<RouteEvent> eventList;
        int rarityRoll = Random.Range(1, 101);

        eventList = rarityRoll <= 55 ? commonEvents :
                    rarityRoll <= 85 ? uncommonEvents :
                    rareEvents;

        return eventList[Random.Range(0, eventList.Count)];
    }

    List<Route> GenerateRoutes()
    {
        float shortRouteTime = Mathf.Round(Random.Range(2f, 4f) * 4f) / 4f;
        float mediumRouteTime = Mathf.Round(Random.Range(4f, 6f) * 4f) / 4f;
        float longRouteTime = Mathf.Round(Random.Range(6f, 8f) * 4f) / 4f;

        Route shortRoute = new("Short Route", shortRouteTime, 75, GetRandomEvent(75));
        Route mediumRoute = new("Medium Route", mediumRouteTime, 55, GetRandomEvent(55));
        Route longRoute = new("Long Route", longRouteTime, 35, GetRandomEvent(35));

        return new List<Route> { shortRoute, mediumRoute, longRoute };
    }
}
