using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip fallbackClip;

    private void Awake()
    {
        // Singleton: if one already exists, destroy the new one
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Persist across scene loads
        DontDestroyOnLoad(gameObject);

        // Optional safety if you forget to assign it
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        // Assign a clip if missing
        if (audioSource != null && audioSource.clip == null && fallbackClip != null)
            audioSource.clip = fallbackClip;

        EnsureSingleAudioListener();
        EnsureSingleEventSystem();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        EnsureSingleAudioListener();
        EnsureSingleEventSystem();
    }

    void Start()
    {
        PlayIfNotPlaying();
    }

    public void PlayIfNotPlaying()
    {
        if (audioSource != null && !audioSource.isPlaying)
            audioSource.Play();
    }

    public void StopMusic()
    {
        if (audioSource != null) audioSource.Stop();
    }

    void EnsureSingleAudioListener()
    {
        AudioListener[] listeners = FindObjectsOfType<AudioListener>(true);
        if (listeners == null || listeners.Length <= 1)
            return;

        bool keeperFound = false;
        foreach (var al in listeners)
        {
            if (al == null) continue;

            if (!keeperFound)
            {
                keeperFound = true;
                if (!al.enabled) al.enabled = true;
            }
            else
            {
                if (al.enabled) al.enabled = false;
            }
        }
    }

    void EnsureSingleEventSystem()
    {
        EventSystem[] systems = FindObjectsOfType<EventSystem>(true);
        if (systems == null || systems.Length <= 1)
            return;

        bool keeperFound = false;
        foreach (var es in systems)
        {
            if (es == null) continue;

            if (!keeperFound)
            {
                keeperFound = true;
                if (!es.gameObject.activeSelf) es.gameObject.SetActive(true);
                if (!es.enabled) es.enabled = true;
            }
            else
            {
                if (es.enabled) es.enabled = false;
                if (es.gameObject.activeSelf) es.gameObject.SetActive(false);
            }
        }
    }
}
