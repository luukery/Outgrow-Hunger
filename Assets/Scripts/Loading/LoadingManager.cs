using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance { get; private set; }

    [Header("Scene Names")]
    public string loadingSceneName = "Loading Screen";

    [Header("Timing")]
    public float minimumLoadingTime = 3f;

    AsyncOperation pendingOperation;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // -------------------------
    // Scene-to-scene loading
    // -------------------------
    public void LoadScene(string targetSceneName) => LoadDefault(targetSceneName);

    public void LoadDefault(string targetSceneName)
    {
        LoadingContext.Mode = LoadingMode.Default;
        StartCoroutine(LoadRoutine(targetSceneName));
    }

    public void LoadTransportMap(string targetSceneName, int legIndex)
    {
        LoadingContext.Mode = LoadingMode.TransportMap;
        LoadingContext.LegIndex = Mathf.Max(0, legIndex);
        StartCoroutine(LoadRoutine(targetSceneName));
    }

    IEnumerator LoadRoutine(string targetSceneName)
    {
        yield return SceneManager.LoadSceneAsync(loadingSceneName);
        yield return null;

        float startTime = Time.unscaledTime;

        pendingOperation = SceneManager.LoadSceneAsync(targetSceneName);
        pendingOperation.allowSceneActivation = false;

        while (pendingOperation.progress < 0.9f)
            yield return null;

        while (Time.unscaledTime - startTime < minimumLoadingTime)
            yield return null;

        LoadingScreenUI ui = FindObjectOfType<LoadingScreenUI>();
        if (ui != null)
            ui.ShowContinuePrompt();

        while (!Input.anyKeyDown)
            yield return null;

        pendingOperation.allowSceneActivation = true;

        // reset after use
        LoadingContext.Mode = LoadingMode.Default;
    }

    // -------------------------
    // NEW: Overlay loading (no scene change)
    // -------------------------
    public void ShowTransportOverlay(int nextLegIndex)
    {
        StartCoroutine(OverlayRoutine(nextLegIndex));
    }

    IEnumerator OverlayRoutine(int nextLegIndex)
    {
        LoadingContext.Mode = LoadingMode.TransportMap;
        LoadingContext.LegIndex = Mathf.Max(0, nextLegIndex);

        // Load loading scene additively ON TOP of current scene
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(loadingSceneName, LoadSceneMode.Additive);
        while (!loadOp.isDone)
            yield return null;

        // Ensure UI Start() runs
        yield return null;

        float startTime = Time.unscaledTime;

        // Enforce minimum time
        while (Time.unscaledTime - startTime < minimumLoadingTime)
            yield return null;

        // Show "Press any key"
        LoadingScreenUI ui = FindObjectOfType<LoadingScreenUI>();
        if (ui != null)
            ui.ShowContinuePrompt();

        // Wait for any key
        while (!Input.anyKeyDown)
            yield return null;

        // Unload the additive loading scene
        yield return SceneManager.UnloadSceneAsync(loadingSceneName);

        // reset mode so other transitions use default
        LoadingContext.Mode = LoadingMode.Default;
    }
}
