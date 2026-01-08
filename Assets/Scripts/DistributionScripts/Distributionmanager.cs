using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class Distributionmanager : MonoBehaviour
{
    public SpawnerScript spawner;
    public Canvas canvas;

    private Button feedButton, denyButton;
    private TextMeshProUGUI dialogue;

    private NPC currentNPC;

    [Header("Flow")]
    public int totalNPCsToProcess = 5;
    private int npcProcessed = 0;

    public string marketSceneName = "Market";
    private Button returnButton;

    void Start()
    {
        feedButton = canvas.transform.Find("FeedButton").GetComponent<Button>();
        denyButton = canvas.transform.Find("DenyButton").GetComponent<Button>();
        dialogue = canvas.transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();

        feedButton.onClick.AddListener(HandleAccept);
        denyButton.onClick.AddListener(Deny);

        Transform returnTf = canvas.transform.Find("ReturnButton");
        if (returnTf != null)
        {
            returnButton = returnTf.GetComponent<Button>();
            returnButton.gameObject.SetActive(false);
            returnButton.onClick.AddListener(() => SceneManager.LoadScene(marketSceneName));
        }

        StartCoroutine(SpawnNPC());
    }

    private IEnumerator SpawnNPC()
    {
        if (npcProcessed >= totalNPCsToProcess)
        {
            FinishDistribution();
            yield break;
        }

        if (currentNPC != null)
        {
            Destroy(currentNPC.gameObject);
            currentNPC = null;
            yield return new WaitForSeconds(4);
        }

        currentNPC = spawner.SpawnNPC();
    }

    private void HandleAccept()
    {
        // TODO: Use Inventory.Instance here to fulfill request
        dialogue.text = "Accepted food";

        npcProcessed++;
        StartCoroutine(SpawnNPC());
    }

    public void Deny()
    {
        dialogue.text = "Denied NPC Food";

        npcProcessed++;
        StartCoroutine(SpawnNPC());
    }

    private void FinishDistribution()
    {
        if (currentNPC != null)
        {
            Destroy(currentNPC.gameObject);
            currentNPC = null;
        }

        dialogue.text = "All NPCs processed! Return to Market.";

        feedButton.interactable = false;
        denyButton.interactable = false;

        if (returnButton != null)
        {
            returnButton.gameObject.SetActive(true);
        }
        else
        {
            SceneManager.LoadScene(marketSceneName);
        }
    }
}
