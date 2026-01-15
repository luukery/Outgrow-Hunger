using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] private AudioSource audioSource;

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
}
