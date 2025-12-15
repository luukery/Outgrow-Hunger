using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;

public class RouteHandler : MonoBehaviour
{
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
        public int FoodGain;
        public RouteEvent(string name, float extraTime, int foodLoss = 0, int goldLoss = 0, int foodGain = 0)
        {
            Name = name;
            ExtraTime = extraTime;
            FoodLoss = foodLoss;
            GoldLoss = goldLoss;
            FoodGain = foodGain;
        }
    }

    // ------------------ EVENT LISTS BY RARITY ------------------
    List<RouteEvent> commonEvents = new()
    {
        new("Fallen tree", 2.5f),
        new("Tree roots", 1f, 5),
        new("Heavy rain", 2f),
        new("Muddy Roads", 2f),
        new("Thick fog", 3f),
        new("Livestock", 1f),
        new("Poor Road", 2f),
        new("Deer", 1f),
        new("Insect Swarm", 2f),
        new("Winds", 0f, 10),
    };

    List<RouteEvent> uncommonEvents = new()
    {
        new("Forest Fire", 4f),
        new("Snowfall", 5f),
        new("Thunderstorm", 5f),
        new("River Crossing", 3f, 5),   // only allowed on Short Road (routeIndex 0)
        new("Tolls", 0f, 0, 15),
        new("Refugees", 0f, 5),
        new("Predators", 0f, 15),
        new("Tired Horses", 5f),
    };

    List<RouteEvent> rareEvents =new()
    {
        new("Hail", 4f, 15),
        new("Icy Roads", 5f),
        new("Sinkhole", 0f, 20),
        new("Bandits", 0f, 15, 15),
        new("Rockslide", 7f),
        new("Cart", 0f, 0, 0, 20),
    };

    // ------------------ ROUTE CLASS ------------------
    public class Route
    {
        public string RouteName;
        public float TravelTime;
        public int EventTakePlaceChance;
        public RouteEvent EventData;

        public Route(string routeName, float travelTime, int eventChance, RouteEvent evt)
        {
            RouteName = routeName;
            TravelTime = travelTime;
            EventTakePlaceChance = eventChance;
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
        ShowIconsForCurrentRoutes();
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
        if (journeyFinished) return;

        float legTime = route.TravelTime;

        int roll = Random.Range(1, 101);
        bool eventHappened = roll <= route.EventTakePlaceChance;

        string eventDescription;

        if (eventHappened)
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
            eventDescription = $"{route.EventData.Name} didn't happen.";
        }

        timePassed += legTime;

        currentFood = Mathf.Max(0, currentFood);
        currentGold = Mathf.Max(0, currentGold);

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

        if (popupTitleText != null && !journeyFinished)
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
        if(!journeyFinished){
            legsCompleted++;
        }

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
        if(shortRoadClickable != null) shortRoadClickable.ResetHighlight();
        if(mediumRoadClickable != null) mediumRoadClickable.ResetHighlight();
        if(longRoadClickable != null) longRoadClickable.ResetHighlight();
    }

    // ------------------ RANDOM EVENT GENERATION ------------------
    RouteEvent GetRandomEvent(int routeIndex)
    {
        int roll = Random.Range(1, 101);
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
            new("Short Route",  shortRouteTime, 100, GetRandomEvent(0)),
            new("Medium Route", mediumRouteTime, 85, GetRandomEvent(1)),
            new("Long Route",   longRouteTime, 70, GetRandomEvent(2))
        };
    }
}
