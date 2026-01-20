using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
// using System.Numerics;
// using System.Diagnostics;
// using System.Numerics;

public class RouteHandler : MonoBehaviour
{
    [HideInInspector] public List<Route> routes;

    [Header("Popup UI (Info Box)")]
    public GameObject infoPopupPanel;
    public GameObject popupBackground;
    
    public TMP_Text popupLegText;
    public TMP_Text popupEventTitleText;
    public TMP_Text popupEventDescText;
    public TMP_Text popupWhyText;
    public TMP_Text totalRouteInfoText;

    public Button popupContinueButton;

    [Header("Journey Settings")]
    public int legsPerJourney = 5;

    [Header("Fallback (only used if Inventory/Wallet are missing)")]
    public int startingFood = 100;
    public int startingGold = 100;

    [Header("Scene Flow")]
    public string distributionSceneName = "Distribution";

    [Header("Event icon roots for each road")]
    public Transform shortRoadEventsRoot;
    public Transform mediumRoadEventsRoot;
    public Transform longRoadEventsRoot;

    [Header("Road clickables (for resetting highlight)")]
    public RoadClickable shortRoadClickable;
    public RoadClickable mediumRoadClickable;
    public RoadClickable longRoadClickable;

    [Header("Journey Progress UI")]
    public GameObject cartIcon;
    private Vector3 cartStartPosition;
    private const float DOT_SPACING = 0.7f;

    [Header("Cart Sprite Animation Settings") ] 
    public Transform cartSprite;
    public List<Vector2> shortRoutePathPoints;
    public List<Vector2> mediumRoutePathPoints;
    public List<Vector2> longRoutePathPoints;
    public float speed = 3f;

    private bool isMoving = false;
    private Coroutine moveCoroutine;

    public static RouteHandler Instance { get; private set; }

    float timePassed = 0f;
    int legsCompleted = 0;
    bool journeyFinished = false;

    // --- NEW: prevents click carryover from Market selecting a road immediately ---
    private bool inputLocked = true;
    public bool CanSelectRoutes => !journeyFinished && !inputLocked;

    // ✅ NEW: spoil tracking
    int spoiledFoodThisJourney = 0;
    int spoiledFoodLastLeg = 0;

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
        public string SituationDescription;
        public string SystematicWhy;
        public float ExtraTime;
        public int FoodLoss;
        public int GoldLoss;
        public int FoodGain;

        public RouteEvent(string name, string situationDescription, string systematicWhy, float extraTime, int foodLoss = 0, int goldLoss = 0, int foodGain = 0)
        {
            Name = name;
            SituationDescription = situationDescription;
            SystematicWhy = systematicWhy;
            ExtraTime = extraTime;
            FoodLoss = foodLoss;
            GoldLoss = goldLoss;
            FoodGain = foodGain;
        }
    }

    // ------------------ EVENT LISTS BY RARITY ------------------
    List<RouteEvent> commonEvents = new()
    {
        new RouteEvent(
            "Fallen Tree",
            "A fallen tree blocks the road. With no maintenance crew nearby, you clear it yourself, losing {time} hours.",
            "When rural roads go uncleared, food takes longer to reach people. Some of it never arriving.",
            2.5f
        ),

        new RouteEvent(
            "Tree Roots",
            "Thick roots break through the road, forcing you to slow and carefully pass, losing {time} hours and {foodLoss} food.",
            "Poor road quality damages vehicles and supplies, reducing how much food survives the journey.",
            1f,
            foodLoss: 5
        ),

        new RouteEvent(
            "Heavy Rain",
            "Rain pours down, soaking the road and slowing your progress by {time} hours.",
            "When transport slows, food spends more time in transit and is more likely to spoil before reaching communities.",
            2f
        ),

        new RouteEvent(
            "Muddy Roads",
            "The road turns to mud, bogging down the carriage and costing {time} hours.",
            "Unpaved roads make food transport unreliable, especially during bad weather.",
            2f
        ),

        new RouteEvent(
            "Thick Fog",
            "Dense fog limits visibility, forcing you to move cautiously and lose {time} hours.",
            "When travel becomes unpredictable, food deliveries are delayed or canceled altogether.",
            3f
        ),

        new RouteEvent(
            "Livestock",
            "Cattle block the road, and guiding them aside takes {time} hour.",
            "In rural areas, shared roads slow transport and reduce how much food reaches markets on time.",
            1f
        ),

        new RouteEvent(
            "Poor Road",
            "Cracked ground and missing stones force a slower pace, costing {time} hours.",
            "Weak infrastructure turns ordinary travel into delays that affect food access.",
            2f
        ),

        new RouteEvent(
            "Deer",
            "A herd crosses the road, bringing you to a halt for {time} hour.",
            "Even small interruptions can disrupt supply routes where timing matters for food delivery.",
            1f
        ),

        new RouteEvent(
            "Insect Swarm",
            "A swarm of insects spooks the horses, forcing a stop that costs {time} hours.",
            "Delays like this add up, making long journeys riskier for food transport.",
            2f
        ),

        new RouteEvent(
            "Winds",
            "Strong winds batter the carriage, rocking it back and forth. You can’t secure everything in time and lose {foodLoss} food.",
            "Without protected transport, food is easily lost long before it reaches communities.",
            0f,
            foodLoss: 10
        ),
    };


    List<RouteEvent> uncommonEvents = new()
    {
        new RouteEvent(
            "Forest Fire",
            "Smoke fills the air as a distant fire forces a wide detour, costing {time} hours.",
            "Environmental disasters cut off routes, stopping food from reaching entire regions.",
            4f
        ),

        new RouteEvent(
            "Snowfall",
            "Snow begins falling mid-journey, turning the road slick and slow and adding {time} hours.",
            "When roads aren’t built for winter, food arrives late… if it arrives at all.",
            5f
        ),

        new RouteEvent(
            "Thunderstorm",
            "A violent storm halts travel, grounding you for {time} hours.",
            "Extreme weather regularly disrupts transport, breaking supply chains that people rely on for food.",
            5f
        ),

        new RouteEvent(
            "River Crossing",
            "The river runs deeper than expected. Crossing carefully costs {time} hours and {foodLoss} food lost to water damage.",
            "Unsafe crossings make food transport risky, especially where bridges are missing or broken.",
            3f,
            foodLoss: 5
        ),

        new RouteEvent(
            "Tolls",
            "Armed collectors demand payment to pass. You lose {goldLoss} gold to continue.",
            "Extra costs along trade routes raise food prices and limit who can afford to move supplies.",
            0f,
            goldLoss: 15
        ),

        new RouteEvent(
            "Refugees",
            "Desperate travelers beg for help. Sharing supplies costs {foodLoss} food.",
            "When people are displaced, already scarce food must stretch even further.",
            0f,
            foodLoss: 5
        ),

        new RouteEvent(
            "Predators",
            "Predators stalk the caravan, forcing you to rush and abandon {foodLoss} food to escape.",
            "In unsafe regions, food is often lost just to keep moving.",
            0f,
            foodLoss: 15
        ),

        new RouteEvent(
            "Tired Horses",
            "The horses slow to a crawl from exhaustion, costing {time} hours.",
            "Limited resources and overuse make long food journeys harder to sustain.",
            5f
        ),
    };


    List<RouteEvent> rareEvents = new()
    {
        new RouteEvent(
            "Hail",
            "Hail pounds the road, damaging supplies and delaying you {time} hours while {foodLoss} food is severely damaged and lost.",
            "Severe weather can destroy food outright before it ever reaches people.",
            4f,
            foodLoss: 15
        ),

        new RouteEvent(
            "Icy Roads",
            "Ice coats the road, forcing extreme caution and adding {time} hours.",
            "Without proper road treatment, winter conditions make food transport dangerous and slow.",
            5f
        ),

        new RouteEvent(
            "Sinkhole",
            "The ground collapses nearby, causing your carriage to slip and crash. You are able to recover most supplies barring {foodLoss} food.",
            "Infrastructure failure can instantly cut off food routes with no warning.",
            0f,
            foodLoss: 15
        ),

        new RouteEvent(
            "Bandits",
            "Bandits ambush the caravan demanding supplies. You escape, but lose {foodLoss} food and {goldLoss} gold.",
            "Conflict and crime along supply routes directly reduce how much food reaches communities.",
            0f,
            foodLoss: 10,
            goldLoss: 10
        ),

        new RouteEvent(
            "Rockslide",
            "Falling rocks block the path, forcing a long detour that costs {time} hours.",
            "In unstable terrain, food transport depends on a few fragile routes.",
            7f
        ),

        new RouteEvent(
            "Cart",
            "An abandoned cart blocks the road. Clearing it takes {time} hours, but you recover {foodGain} food from what was left behind.",
            "When transport fails, food is often wasted long before it can be used.",
            0.5f,
            foodGain: 20
        ),
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

    // ------------------ LIVE VALUES (Inventory/Wallet) ------------------
    int GetFoodLive()
    {
        if (Inventory.Instance != null) return Inventory.Instance.GetTotalFoodUnits();
        return startingFood;
    }

    int GetGoldLive()
    {
        if (Wallet.Instance != null) return Wallet.Instance.Money;
        return startingGold;
    }

    void ApplyFoodLoss(int amount)
    {
        if (amount <= 0) return;

        if (Inventory.Instance != null)
            Inventory.Instance.TryRemoveFoodUnits(amount);
        else
            startingFood = Mathf.Max(0, startingFood - amount);
    }

    void ApplyFoodGain(int amount)
    {
        if (amount <= 0) return;

        if (Inventory.Instance != null)
        {
            // Get a random food type from available catalog
            ProductCatalogSO catalog = Resources.Load<ProductCatalogSO>("ProductCatalog");
            FoodType.Type randomFoodType = FoodType.Type.Meat; // default fallback
            
            if (catalog != null)
            {
                List<FoodType.Type> availableTypes = catalog.GetAvailableFoodTypes();
                if (availableTypes.Count > 0)
                {
                    randomFoodType = availableTypes[Random.Range(0, availableTypes.Count)];
                }
            }
            
            Inventory.Instance.TryAddFoodToInventory(new Food(randomFoodType, Food.Quality.Medium, amount, "Supplies"));
        }
        else
        {
            startingFood += amount;
        }
    }

    void ApplyGoldLoss(int amount)
    {
        if (amount <= 0) return;

        if (Wallet.Instance != null)
            Wallet.Instance.RemoveMoneyClamped(amount);
        else
            startingGold = Mathf.Max(0, startingGold - amount);
    }

    // ------------------ UNITY LIFECYCLE ------------------
    void Start()
    {
        if (cartIcon != null)
        {
            RectTransform cartRect = cartIcon.GetComponent<RectTransform>();
            if (cartRect != null)
                cartStartPosition = cartRect.localPosition;
        }

        timePassed = 0f;
        legsCompleted = 0;
        journeyFinished = false;

        // ✅ reset spoil counters
        spoiledFoodThisJourney = 0;
        spoiledFoodLastLeg = 0;

        StartCoroutine(UnlockAfterMouseRelease());

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

    IEnumerator UnlockAfterMouseRelease()
    {
        inputLocked = true;
        yield return null;

        if (Mouse.current != null)
        {
            while (Mouse.current.leftButton.isPressed)
                yield return null;
        }
        else
        {
            while (Input.GetMouseButton(0))
                yield return null;
        }

        inputLocked = false;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            if (!journeyFinished)
                RouteSetUp();
        }

        if (Keyboard.current != null && Keyboard.current.dKey.wasPressedThisFrame && !isMoving)
        {
            Debug.Log("test");
            moveCoroutine = StartCoroutine(MoveCartAlongRoute(0));
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
        if (!CanSelectRoutes)
            return;

        if (routes == null || routes.Count <= index)
        {
            Debug.LogWarning("Route index out of range.");
            return;
        }

        HandleRouteSelection(routes[index], index);
    }

    string FormatSituation(RouteEvent e)
    {
        string text = e.SituationDescription;

        if (e.ExtraTime > 0)
        {
            text = text.Replace("{time}", $"<color=#ff4d4d>{e.ExtraTime}</color>");
        }

        if (e.FoodLoss > 0)
        {
            text = text.Replace("{foodLoss}", $"<color=#ff4d4d>{e.FoodLoss}</color>");
        }

        if (e.GoldLoss > 0)
        {
            text = text.Replace("{goldLoss}", $"<color=#ff4d4d>{e.GoldLoss}</color>");
        }

        if (e.FoodGain > 0)
        {
            text = text.Replace("{foodGain}", $"<color=#4dff4d>{e.FoodGain}</color>");
        }

        return text;
    }


    void HandleRouteSelection(Route route, int routeIndex)
    {
        if (!CanSelectRoutes) return;

        moveCoroutine = StartCoroutine(MoveCartAlongRoute(routeIndex));
        float legTime = route.TravelTime;

        int roll = Random.Range(1, 101);
        bool eventHappened = roll <= route.EventTakePlaceChance;

        RouteEvent occurredEvent = null;

        if (eventHappened)
        {
            occurredEvent = route.EventData;

            ApplyFoodLoss(route.EventData.FoodLoss);
            ApplyGoldLoss(route.EventData.GoldLoss);
            ApplyFoodGain(route.EventData.FoodGain);
        }


        timePassed += legTime;

        // ✅ Spoilage advances ONLY here (transport time)
        spoiledFoodLastLeg = 0;
        if (SpoilageManager.Instance != null)
        {
            spoiledFoodLastLeg = SpoilageManager.Instance.AdvanceTravelTime(legTime);
            spoiledFoodThisJourney += spoiledFoodLastLeg;
        }
        
        // Wait a second before showing popup
        StartCoroutine(ShowPopupAfterDelay());

        IEnumerator ShowPopupAfterDelay()
        {
            yield return new WaitForSeconds(2.5f);
            ShowInfoPopup(route, legTime, eventHappened, occurredEvent);
        }    
    }


    // ------------------ POPUP LOGIC ------------------
    void ShowInfoPopup(Route route, float legTime, bool eventHappened, RouteEvent e)
    {
        if (infoPopupPanel == null)
        {
            ContinueAfterPopup();
            return;
        }

        // --- Leg header ---
        if (popupLegText != null && !journeyFinished)
        {
            popupLegText.text = $"Leg {legsCompleted + 1}:\n{route.RouteName}";
        }

        // --- Event title ---
        if (popupEventTitleText != null)
        {
            popupEventTitleText.text = eventHappened && e != null
                ? $"<b>{e.Name}</b>"
                : "<b>No Event</b>";
        }

        // --- Event description ---
        if (popupEventDescText != null)
        {
            popupEventDescText.text = eventHappened && e != null
                ? FormatSituation(e) // keeps red colouring
                : "You were not affected by this event.";
        }

        // --- Systematic why (italic stays) ---
        if (popupWhyText != null)
        {
            popupWhyText.text = eventHappened && e != null
                ? $"<i>{e.SystematicWhy}</i>"
                : "";
        }

        // --- Route totals ---
        if (totalRouteInfoText != null)
        {
            totalRouteInfoText.text =
                $"Time this leg: {legTime}h\n" +
                $"Total time travelled: {timePassed}h\n\n" +
                $"Current Food: {GetFoodLive()}\n" +
                $"Current Gold: {GetGoldLive()}";
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
        if (!journeyFinished)
            legsCompleted++;

        UpdateCartPosition();
        ResetCartSpritePosition();
        StopCoroutine(moveCoroutine);
        moveCoroutine = null;

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
            LoadingManager.Instance.LoadScene(distributionSceneName);
            return;
        }

        if (popupEventTitleText != null)
            popupEventTitleText.text = "<b>Journey Complete</b>";

        if (popupEventDescText != null)
        {
            popupEventDescText.text =
                $"Total legs travelled: {legsCompleted}\n" +
                $"Total time spent: {timePassed}h\n\n" +
                $"Spoiled during journey: -{spoiledFoodThisJourney} Food\n";
        }

        if (popupWhyText != null)
        {
            popupWhyText.text =
                $"\n\n" +
                $"Current Food: {GetFoodLive()}\n" +
                $"Current Gold: {GetGoldLive()}\n\n" +
                $"Press Continue to deliver.";
        }

        if (totalRouteInfoText != null)
        {
            totalRouteInfoText.text = "";
        }


        infoPopupPanel.SetActive(true);

        if (popupBackground != null)
            popupBackground.SetActive(true);

        if (popupContinueButton != null)
        {
            popupContinueButton.onClick.RemoveAllListeners();
            popupContinueButton.onClick.AddListener(() =>
            {
                LoadingManager.Instance.LoadScene(distributionSceneName);
            });
        }
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
    RouteEvent GetRandomEvent(int routeIndex)
    {
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
            new("Medium Route", mediumRouteTime, 80, GetRandomEvent(1)),
            new("Long Route",   longRouteTime, 60, GetRandomEvent(2))
        };
    }

    void UpdateCartPosition()
    {
        if (cartIcon != null)
        {
            RectTransform cartRect = cartIcon.GetComponent<RectTransform>();
            if (cartRect != null)
            {
                Vector3 newPosition = cartStartPosition;
                newPosition.x += legsCompleted * DOT_SPACING;
                cartRect.localPosition = newPosition;
            }
        }
    }

    IEnumerator MoveCartAlongRoute(int index)
    {
        isMoving = true;

        List<Vector2> pathPoints = index switch
        {
            0 => shortRoutePathPoints,
            1 => mediumRoutePathPoints,
            2 => longRoutePathPoints,
            _ => shortRoutePathPoints
        };

        foreach (Vector2 target in pathPoints)
        {
            while ((Vector2)cartSprite.position != target)
            {
                cartSprite.position = Vector2.MoveTowards(
                    cartSprite.position,
                    target,
                    speed * Time.deltaTime
                );

                yield return null; // wait for next frame
            }
        }

        isMoving = false;
    }

    void ResetCartSpritePosition()
    {
        if (cartSprite != null && shortRoutePathPoints.Count > 0)
        {
            cartSprite.position = new Vector3(-8.5f, -0.3f, cartSprite.position.z);
        }
    }
}
