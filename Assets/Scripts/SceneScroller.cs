using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneScroller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public int currentSceneIndex = 0;
    void Start()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

    }

    private static SceneScroller instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void NextScene()
    {
        currentSceneIndex++;

        if (currentSceneIndex>= SceneManager.sceneCountInBuildSettings)
        {
            //de check dat je niet een index.outofrange error krijgt
            currentSceneIndex = 0; 
        }
        SceneManager.LoadScene(currentSceneIndex);

    }

    public void PreviousScene()
    {
        currentSceneIndex--;

        if (currentSceneIndex < 0)
            currentSceneIndex = SceneManager.sceneCountInBuildSettings - 1;

        SceneManager.LoadScene(currentSceneIndex);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
