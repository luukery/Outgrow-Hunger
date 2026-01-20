using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScreenUI : MonoBehaviour
{
    [Header("Panels")]
    public GameObject defaultPanel;
    public GameObject transportPanel;

    [Header("Default Panel UI")]
    public TMP_Text defaultWorldFactsText;
    public TMP_Text defaultLoadingText;
    public TMP_Text defaultContinueText;
    public GameObject defaultLoadingIcon;
    public Image defaultBackgroundImage;

    [Header("Transport Panel UI")]
    public TMP_Text transportWorldFactsText;
    public TMP_Text transportLoadingText;
    public TMP_Text transportContinueText;
    public GameObject transportLoadingIcon; // optional (can be null if you don't want it)
    public RectTransform transportArrow;    // your Arrow object (RectTransform)
    public RectTransform transportMap;      // MapImage RectTransform (parent)

   [Header("Arrow Targets (Transport)")]
[Tooltip("Anchored positions (pixels) relative to MapImage. Set these by placing the Arrow on each split and copying its Anchored Pos.")]
public Vector2[] splitAnchoredPositions;


    [Header("Text Source")]
    public LoadingTextDatabase textDatabase;
    public bool randomText = true;

    [Header("Optional Background Switching (Default)")]
    public bool randomBackground = false;
    public Sprite[] backgrounds;

    [Header("Arrow Bounce")]
    public float bounceAmplitude = 10f;
    public float bounceSpeed = 2.5f;

    static int lastTextIndex = -1;
    Vector2 arrowBasePos;

    void Start()
    {
        ApplyMode();
        SetupInitialState();
        SetFactText();
        SetDefaultBackgroundIfNeeded();
        PositionArrowIfNeeded();
    }

    void ApplyMode()
    {
        bool isTransport = LoadingContext.Mode == LoadingMode.TransportMap;

        if (defaultPanel != null) defaultPanel.SetActive(!isTransport);
        if (transportPanel != null) transportPanel.SetActive(isTransport);
    }

    void SetupInitialState()
    {
        bool isTransport = LoadingContext.Mode == LoadingMode.TransportMap;

        // Default panel
        if (!isTransport)
        {
            if (defaultLoadingText != null) defaultLoadingText.gameObject.SetActive(true);
            if (defaultLoadingIcon != null) defaultLoadingIcon.SetActive(true);
            if (defaultContinueText != null) defaultContinueText.gameObject.SetActive(false);
        }
        // Transport panel
        else
        {
            if (transportLoadingText != null) transportLoadingText.gameObject.SetActive(true);
            if (transportLoadingIcon != null) transportLoadingIcon.SetActive(true);
            if (transportContinueText != null) transportContinueText.gameObject.SetActive(false);
        }
    }

    public void ShowContinuePrompt()
    {
        bool isTransport = LoadingContext.Mode == LoadingMode.TransportMap;

        if (!isTransport)
        {
            if (defaultLoadingText != null) defaultLoadingText.gameObject.SetActive(false);
            if (defaultLoadingIcon != null) defaultLoadingIcon.SetActive(false);
            if (defaultContinueText != null) defaultContinueText.gameObject.SetActive(true);
        }
        else
        {
            if (transportLoadingText != null) transportLoadingText.gameObject.SetActive(false);
            if (transportLoadingIcon != null) transportLoadingIcon.SetActive(false);
            if (transportContinueText != null) transportContinueText.gameObject.SetActive(true);
        }
    }

    void SetFactText()
    {
        if (textDatabase == null || textDatabase.texts == null || textDatabase.texts.Count == 0)
            return;

        int idx;
        if (randomText)
        {
            if (textDatabase.texts.Count == 1) idx = 0;
            else
            {
                do { idx = Random.Range(0, textDatabase.texts.Count); }
                while (idx == lastTextIndex);
            }
        }
        else
        {
            idx = (lastTextIndex + 1) % textDatabase.texts.Count;
        }

        lastTextIndex = idx;
        string line = textDatabase.texts[idx];

        // Write into whichever panel is active
        if (LoadingContext.Mode == LoadingMode.TransportMap)
        {
            if (transportWorldFactsText != null) transportWorldFactsText.text = line;
        }
        else
        {
            if (defaultWorldFactsText != null) defaultWorldFactsText.text = line;
        }
    }

    void SetDefaultBackgroundIfNeeded()
    {
        if (LoadingContext.Mode == LoadingMode.TransportMap) return;

        if (!randomBackground || defaultBackgroundImage == null || backgrounds == null || backgrounds.Length == 0)
            return;

        defaultBackgroundImage.sprite = backgrounds[Random.Range(0, backgrounds.Length)];
    }

    void PositionArrowIfNeeded()
{
    if (LoadingContext.Mode != LoadingMode.TransportMap) return;

    if (transportArrow == null || splitAnchoredPositions == null || splitAnchoredPositions.Length == 0)
        return;

    int leg = Mathf.Clamp(LoadingContext.LegIndex, 0, splitAnchoredPositions.Length - 1);

    arrowBasePos = splitAnchoredPositions[leg];
    transportArrow.anchoredPosition = arrowBasePos;
    transportArrow.gameObject.SetActive(true);
}


    void Update()
    {
        // Bounce arrow only in transport mode
        if (LoadingContext.Mode != LoadingMode.TransportMap) return;
        if (transportArrow == null) return;

        float y = Mathf.Sin(Time.unscaledTime * bounceSpeed * Mathf.PI * 2f) * bounceAmplitude;
        transportArrow.anchoredPosition = arrowBasePos + new Vector2(0f, y);
    }
}
