using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Global scene-based time advance system.
/// Lives once (DontDestroyOnLoad) and advances spoilage time
/// when scenes change.
/// </summary>
public class SceneTimeAdvance : MonoBehaviour
{
    public static SceneTimeAdvance Instance { get; private set; }

    [System.Serializable]
    public class SceneTimeRule
    {
        public string sceneName;
        public float hoursOnEnter;
    }

    [Header("Scene time rules")]
    [Tooltip("How many hours pass when ENTERING each scene.")]
    public List<SceneTimeRule> sceneRules = new();

    string lastSceneName;
    bool isInitialized = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        // Initialize with the starting scene
        lastSceneName = SceneManager.GetActiveScene().name;
        isInitialized = true;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Skip first load (game boot)
        if (!isInitialized)
            return;

        // Prevent double-fire for same scene reloads
        if (scene.name == lastSceneName)
            return;

        float hours = GetHoursForScene(scene.name);

        if (hours > 0f && SpoilageManager.Instance != null)
        {
            SpoilageManager.Instance.AdvanceTravelTime(hours);
            Debug.Log($"[SceneTimeAdvance] Entered '{scene.name}', +{hours}h time passed.");
        }

        lastSceneName = scene.name;
    }

    float GetHoursForScene(string sceneName)
    {
        for (int i = 0; i < sceneRules.Count; i++)
        {
            if (sceneRules[i].sceneName == sceneName)
                return Mathf.Max(0f, sceneRules[i].hoursOnEnter);
        }
        return 0f;
    }
}
