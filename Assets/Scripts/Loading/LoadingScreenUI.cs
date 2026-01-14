using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScreenUI : MonoBehaviour
{
    [Header("TMP References")]
    public TMP_Text worldFactsText;     // your "WorldFacts" TMP_Text
    public TMP_Text loadingLabelText;   // your "LoadingText" TMP_Text (optional)

    [Header("Background Image (optional)")]
    public Image backgroundImage;

    [Header("Text Source")]
    public LoadingTextDatabase textDatabase;
    public bool randomText = true;

    [Header("Optional Background Switching")]
    public bool randomBackground = false;
    public Sprite[] backgrounds;

    static int lastTextIndex = -1;

    void Start()
    {
        SetLoadingLabel();
        SetRandomFact();
        SetBackground();
    }

    void SetLoadingLabel()
    {
        if (loadingLabelText != null)
            loadingLabelText.text = "Loading";
    }

    void SetRandomFact()
    {
        if (worldFactsText == null || textDatabase == null || textDatabase.texts.Count == 0)
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
        worldFactsText.text = textDatabase.texts[idx];
    }

    void SetBackground()
    {
        if (!randomBackground || backgroundImage == null || backgrounds == null || backgrounds.Length == 0)
            return;

        backgroundImage.sprite = backgrounds[Random.Range(0, backgrounds.Length)];
    }
}
