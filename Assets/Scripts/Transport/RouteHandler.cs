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

    [Header("Popup UI (Info Box)")]
    public GameObject infoPopupPanel;
    public GameObject popupBackground;
    public TMP_Text popupTitleText;
    public TMP_Text popupBodyText;
    public Button popupContinueButton;

    [Header("Journey Settings")]
    public int legsPerJourney = 5;
    public int startingFood = 100;
    public int startingGold = 100;

    [Header("Event icon roots for each road")]
    public Transform shortRoadEventsRoot;
    public Transform mediumRoadEventsRoot;
    public Transform longRoadEventsRoot;

    [Header("Road clickables (for resetting highlight)")]
    public RoadClickable shortRoadClickable;
    public RoadClickable mediumRoadClickable;
    public RoadClickable longRoadClickable;

    public static RouteHandler Instance { get; private set; }

    float timePassed = 0f;
    int currentFood;
    int currentGold;
    int legsCompleted = 0;
    bool journeyFinished = false;

    private List<Route> routes;

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

    // ------------------ EVENT LISTS BY RARITY ------------------
    List<RouteEvent> commonEvents = new List<RouteEvent>
    {
        new("Fallen tree", 0.5f),
        new("Tree roots", 0.25f, 15),
        new("Heavy rain", 0.5f),
        new("Muddy Roads", 1.25f),
        new("Thick fog", 1f),
        new("Livestock", 1f),
        new("Poor Road", 0.5f),
        new("Deer", 0.75f),
        new("Insect Swarm", 0.25f),
        new("Winds", 0f, 10),
    };

    List<RouteEvent> uncommonEvents = new List<RouteEvent>
    {
        new("Forest Fire", 1f),
        new("Snowfall", 0.5f, 10),
        new("Thunderstorm", 0.5f),
        new("Cart", 0.75f),
        new("River Crossing", 0.75f),   // only allowed on Short Road (routeIndex 0)
        new("Tolls", 0f, 0, 10),
        new("Refugees", 0f, 5),
        new("Predators", 0f, 10),
        new("Tired Horses", 0.5f),
    };

    List<RouteEvent> rareEvents = new List<RouteEvent>
    {
        new("Hail", 1f),
        new("Ice Roads", 0.5f),
        new("Sinkhole", 0f, 15),
        new("Bandits", 0f, 15),
        new("Rockslide", 0.5f),
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

    // ------------------ UNITY LIFECYCLE ------------------
    void Start()
    {
        currentFood = startingFood;
        currentGold = startingGold;
        timePassed = 0f;
        legsCompleted = 0;
        journeyFinished = false;

        if (popupContinueButton != null)
        {
            popupContinueButton.onClick.RemoveAllListeners();
            popupContinueButton.onClick.AddListener(OnPopupContinue);
        }

        if (infoPopupPanel != null)
            infoPopupPanel.SetActive(false);
        if (popupBackground != null)
            popupBackground.SetActive(false);

        ClearAllEventIcons();
        ResetRoadHighlights();

        RouteSetUp();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (!journeyFinished)
                RouteSetUp();
        }
    }

    // ------------------ MAIN SETUP ------------------
    void RouteSetUp()
    {
        if (journeyFinished)
            return;

        ClearAllEventIcons();
        ResetRoadHighlights();

        routes = GenerateRoutes();
        SetUpButtons(routes);
        ShowIconsForCurrentRoutes();
    }

    // ------------------ BUTTON SETUP ------------------
    void SetUpButtons(List<Route> routes)
    {
        bool usingButtons =
            shortRouteButton != null ||
            mediumRouteButton != null ||
            longRouteButton != null;

        if (!usingButtons)
            return;

        if (shortRouteButton != null)
        {
            shortRouteButton.onClick.RemoveAllListeners();
            shortRouteButton.onClick.AddListener(() => HandleRouteSelection(routes[0], 0));
            UpdateButtonText(shortRouteButton, routes[0]);
        }

        if (mediumRouteButton != null)
        {
            mediumRouteButton.onClick.RemoveAllListeners();
            mediumRouteButton.onClick.AddListener(() => HandleRouteSelection(routes[1], 1));
            UpdateButtonText(mediumRouteButton, routes[1]);
        }

        if (longRouteButton != null)
        {
            longRouteButton.onClick.RemoveAllListeners();
            longRouteButton.onClick.AddListener(() => HandleRouteSelection(routes[2], 2));
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

        string eventText = route.EventData == null
            ? "Event: None"
            : $"Event: {route.EventData.Name} (+{route.EventData.ExtraTime}h)";

        tmp.text =
            $"{route.RouteName}\n" +
            $"Time: {route.TravelTime}h\n" +
            $"Chance: {route.EventChance}%\n" +
            $"{eventText}";
    }

    // ------------------ ROUTE SELECTION ------------------
    public void SelectRouteByIndex(int index)
    {
        if (journeyFinished)
            return;

        if (routes == null || routes.Count <= index)
        {
            Debug.LogWarning("Route index out of range.");
            return;
        }

        HandleRouteSelection(routes[index], index);
    }

    void HandleRouteSelection(Route route, int routeIndex)
    {
        if (journeyFinished)
            return;

        float legTime = route.TravelTime;

        string eventDescription;
        if (route.EventData != null)
        {
            legTime += route.EventData.ExtraTime;
            currentFood -= route.EventData.FoodLoss;
            currentGold -= route.EventData.GoldLoss;

            eventDescription =
                $"{route.EventData.Name}\n" +
                $"Extra time: +{route.EventData.ExtraTime}h";

            if (route.EventData.FoodLoss != 0 || route.EventData.GoldLoss != 0)
            {
                eventDescription += "\nLosses: ";
                if (route.EventData.FoodLoss != 0)
                    eventDescription += $"-{route.EventData.FoodLoss} food ";
                if (route.EventData.GoldLoss != 0)
                    eventDescription += $"-{route.EventData.GoldLoss} gold";
            }
        }
        else
        {
            eventDescription = "No event happened on this road.";
        }

        timePassed += legTime;

        currentFood = Mathf.Max(0, currentFood);
        currentGold = Mathf.Max(0, currentGold);

        if (resultText != null)
            resultText.text = eventDescription;
        if (timePassedText != null)
            timePassedText.text = $"Time Passed: {timePassed}h";

        ShowInfoPopup(route, legTime, eventDescription);
    }

    // ------------------ POPUP LOGIC ------------------
    void ShowInfoPopup(Route route, float legTime, string eventDescription)
    {
        if (infoPopupPanel == null)
        {
            ContinueAfterPopup();
            return;
        }

        if (popupTitleText != null)
        {
            popupTitleText.text = $"Leg {legsCompleted + 1}:\n{route.RouteName}";
        }

        if (popupBodyText != null)
        {
            popupBodyText.text =
                $"{eventDescription}\n\n" +
                $"Time this leg: {legTime}h\n" +
                $"Total time travelled: {timePassed}h\n\n" +
                $"Current Food: {currentFood}\n" +
                $"Current Gold: {currentGold}";
        }

        infoPopupPanel.SetActive(true);

        if (popupBackground != null)
            popupBackground.SetActive(true);
    }

    void OnPopupContinue()
    {
        if (infoPopupPanel != null)
            infoPopupPanel.SetActive(false);
        if (popupBackground != null)
            popupBackground.SetActive(false);

        ClearAllEventIcons();
        ResetRoadHighlights();

        ContinueAfterPopup();
    }

    void ContinueAfterPopup()
    {
        legsCompleted++;

        if (legsCompleted >= legsPerJourney)
        {
            ShowFinalSummary();
        }
        else
        {
            RouteSetUp();
        }
    }

    void ShowFinalSummary()
    {
        journeyFinished = true;

        ClearAllEventIcons();
        ResetRoadHighlights();

        if (infoPopupPanel == null)
        {
            Debug.Log($"Journey finished!\nTotal time: {timePassed}h\nFood: {currentFood}\nGold: {currentGold}");
            return;
        }

        if (popupTitleText != null)
            popupTitleText.text = "Journey Complete";

        if (popupBodyText != null)
        {
            popupBodyText.text =
                $"Total legs travelled: {legsCompleted}\n" +
                $"Total time spent: {timePassed}h\n\n" +
                $"Final Food: {currentFood}\n" +
                $"Final Gold: {currentGold}";
        }

        infoPopupPanel.SetActive(true);

        if (popupBackground != null)
            popupBackground.SetActive(true);
    }

    // ------------------ EVENT ICON & HIGHLIGHT HELPERS ------------------
    void ShowIconsForCurrentRoutes()
    {
        if (routes == null) return;

        if (routes.Count > 0) ShowEventIcon(0, routes[0].EventData);
        if (routes.Count > 1) ShowEventIcon(1, routes[1].EventData);
        if (routes.Count > 2) ShowEventIcon(2, routes[2].EventData);
    }

    Transform GetEventsRootForIndex(int routeIndex)
    {
        switch (routeIndex)
        {
            case 0: return shortRoadEventsRoot;
            case 1: return mediumRoadEventsRoot;
            case 2: return longRoadEventsRoot;
            default: return null;
        }
    }

    void ShowEventIcon(int routeIndex, RouteEvent evt)
    {
        if (evt == null)
            return;

        Transform root = GetEventsRootForIndex(routeIndex);
        if (root == null)
        {
            Debug.LogWarning("No events root assigned for route index " + routeIndex);
            return;
        }

        string eventName = evt.Name.ToLowerInvariant();

        foreach (Transform child in root)
        {
            if (child == null) continue;

            string childName = child.name.ToLowerInvariant();
            if (childName.Contains(eventName) || eventName.Contains(childName))
            {
                child.gameObject.SetActive(true);
                return;
            }
        }

        Debug.LogWarning($"No event icon found under {root.name} for event '{evt.Name}'.");
    }

    void ClearAllEventIcons()
    {
        ClearIconsUnderRoot(shortRoadEventsRoot);
        ClearIconsUnderRoot(mediumRoadEventsRoot);
        ClearIconsUnderRoot(longRoadEventsRoot);
    }

    void ClearIconsUnderRoot(Transform root)
    {
        if (root == null) return;

        foreach (Transform child in root)
        {
            if (child != null && child.gameObject.activeSelf)
                child.gameObject.SetActive(false);
        }
    }

    void ResetRoadHighlights()
    {
        if (shortRoadClickable != null) shortRoadClickable.ResetHighlight();
        if (mediumRoadClickable != null) mediumRoadClickable.ResetHighlight();
        if (longRoadClickable != null) longRoadClickable.ResetHighlight();
    }

    // ------------------ RANDOM EVENT GENERATION ------------------
    RouteEvent GetRandomEvent(int eventChance, int routeIndex)
    {
        int roll = Random.Range(1, 101);
        if (roll > eventChance)
            return null;

        int rarityRoll = Random.Range(1, 101);

        List<RouteEvent> list =
            rarityRoll <= 55 ? commonEvents :
            rarityRoll <= 85 ? uncommonEvents :
            rareEvents;

        RouteEvent chosen = null;
        int safety = 20;

        while (chosen == null && safety-- > 0)
        {
            RouteEvent candidate = list[Random.Range(0, list.Count)];
            if (IsEventAllowedForRoute(candidate, routeIndex))
                chosen = candidate;
        }

        return chosen;
    }

    bool IsEventAllowedForRoute(RouteEvent evt, int routeIndex)
    {
        if (evt.Name == "River Crossing" && routeIndex != 0)
            return false;

        return true;
    }

    List<Route> GenerateRoutes()
    {
        float shortRouteTime = Mathf.Round(Random.Range(2f, 4f) * 4f) / 4f;
        float mediumRouteTime = Mathf.Round(Random.Range(4f, 6f) * 4f) / 4f;
        float longRouteTime = Mathf.Round(Random.Range(6f, 8f) * 4f) / 4f;

        return new List<Route>
        {
            new("Short Route",  shortRouteTime, 95, GetRandomEvent(95, 0)),
            new("Medium Route", mediumRouteTime, 80, GetRandomEvent(80, 1)),
            new("Long Route",   longRouteTime, 70, GetRandomEvent(70, 2))
        };
    }
}
