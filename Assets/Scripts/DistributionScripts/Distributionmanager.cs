using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Distributionmanager : MonoBehaviour
{
    public SpawnerScript spawner;
    public Canvas canvas;

    private Button feedButton, denyButton;
    private TextMeshProUGUI dialogue;

    private NPC currentNPC;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnNPC());

        feedButton = canvas.transform.Find("FeedButton").GetComponent<Button>();
        denyButton = canvas.transform.Find("DenyButton").GetComponent<Button>();
        dialogue = canvas.transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();

        feedButton.onClick.AddListener(HandleAccept);
        denyButton.onClick.AddListener(Deny);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentNPC != null)
        {

        }
    }

    private IEnumerator SpawnNPC()
    {
        // despawns NPC and waits 5 seconds before spawning a new one if an NPC is already spawned
        if (currentNPC != null)
        {
            Destroy(currentNPC.gameObject);
            currentNPC = null;
            Debug.Log("Despawned NPC");
            yield return new WaitForSeconds(4);
        }
        // Temp text
        currentNPC = spawner.SpawnNPC();
        HandleDialgoue();
        Debug.Log("Spawned NPC");
    }

    private void HandleAccept()
    {
        dialogue.text = "Accepted food";
        StartCoroutine(SpawnNPC());

        // Check if food can be delivered
    }

    public void Deny()
    {
        // temp text
        dialogue.text = "Denied NPC Food";
        StartCoroutine(SpawnNPC());
    }

    private void HandleDialogue()
    {
        // get dto npc request and order
        currentNPC.getInfoDTO();

        // dialogue displays request and order


        // Check if order can be completed, if not disable button/change button text
    }
}
