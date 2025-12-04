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

    private GameObject currentNPC;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnNPC();
        
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
    
    private void SpawnNPC()
    {
        if (currentNPC != null) 
        {
            Destroy(currentNPC);
            Debug.Log("Despawned NPC");
        }


        currentNPC = spawner.SpawnNPC();
        Debug.Log("Spawned NPC");
    }

    private void HandleAccept()
    {
        // code om eten te geven
        dialogue.text = "accepted food";
        SpawnNPC();
    }

    public void Deny()
    {
        // temp text
        dialogue.text = "Denied NPC Food";
        SpawnNPC();
    }
}
