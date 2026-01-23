using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButtonLoader : MonoBehaviour
{
    [Header("Scene to load when Start is pressed")]
    [SerializeField] private string marketSceneName = "MarketPlace";

    // Hook this up to the Button's OnClick OR call from code
    public void StartGame()
    {
        PlayerPrefs.DeleteKey(Distribution_Tutorial01);
        PlayerPrefs.DeleteKey(Market_Tutorial01);
        PlayerPrefs.DeleteKey(Transport_Tutorial01);
        SceneManager.LoadScene(marketSceneName);
    }

    // Optional: quick keyboard test in Play Mode
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
            StartGame();
    }
}
