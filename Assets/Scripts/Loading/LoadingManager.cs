using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance { get; private set; }

    [Header("Scene Names")]
    [Tooltip("Must match EXACT name in Build Settings.")]
    public string loadingSceneName = "Loading Screen";

    [Header("Timing")]
    [Tooltip("Minimum seconds to show the loading scene.")]
    public float minimumLoadingTime = 2f;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadScene(string targetSceneName)
    {
        StartCoroutine(LoadRoutine(targetSceneName));
    }

    IEnumerator LoadRoutine(string targetSceneName)
    {
        // 1) Show loading scene
        yield return SceneManager.LoadSceneAsync(loadingSceneName);

        // Let UI initialize
        yield return null;

        float start = Time.unscaledTime;

        // 2) Load target scene in background
        AsyncOperation op = SceneManager.LoadSceneAsync(targetSceneName);
        op.allowSceneActivation = false;

        // Wait until ready (0.9 = loaded, waiting for activation)
        while (op.progress < 0.9f)
            yield return null;

        // 3) Enforce minimum display time
        while (Time.unscaledTime - start < minimumLoadingTime)
            yield return null;

        // 4) Activate
        op.allowSceneActivation = true;

        while (!op.isDone)
            yield return null;
    }
}
