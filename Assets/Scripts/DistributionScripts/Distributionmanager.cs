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
    private GameObject foodselect;
    private List<Dropdown> dropdowns = new();

    private NPC currentNPC;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnNPC();

        feedButton = canvas.transform.Find("FeedButton").GetComponent<Button>();
        denyButton = canvas.transform.Find("DenyButton").GetComponent<Button>();
        dialogue = canvas.transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();
        foodselect = canvas.transform.Find("FoodSelect").gameObject;

        for (int i = 0; i <= 4 ; i++)
        {
            string dropdownname = "Dropdown" + i;
            Dropdown dropdown = foodselect.transform.Find(dropdownname).GetComponent<Dropdown>();
            dropdowns.Add(dropdown);
            Debug.Log(dropdown);
        }

        feedButton.onClick.AddListener(HandleAccept);
        denyButton.onClick.AddListener(Deny);
        foodselect.SetActive(false);
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
        // despawns NPC and waits 5 seconds before spawning a new one if an NPC is already spawned
        if (currentNPC != null)
        {
            Destroy(currentNPC.gameObject);
            currentNPC = null;
            Debug.Log("Despawned NPC");
        }
        // Temp text
        currentNPC = spawner.SpawnNPC();
        Debug.Log("Spawned NPC");
    }

    private void HandleAccept()
    {
        dialogue.text = "Accepted food";
        FoodSelector();
        // Check if food can be delivered
    }

    private void FoodSelector()
    {
        // steps
        // take food from npc
        // change toggle text into food needed
        // gray out food that dont have enough of 
        // button change text
        // button remove listeners
        // button add new listeners

        foodselect.SetActive(true);
        // dialogue needs to be reenabled eventually
        dialogue.gameObject.SetActive(false);

        NpcInfoDTO orderinfo = currentNPC.GetInfoDTO();
        // cant do a foreach loop bc there's multiple lists in dto 
        for (int index = 0; index < orderinfo.Needs.Count; index++)
        {
            // temp break bc there's only 4 dropdowns
            if (index >= 5)
                break;
            Dropdown dropdown = dropdowns[index];
            Debug.Log("Searching on: " + dropdown.name);
            TextMeshProUGUI ordertext = dropdown.transform.Find("OrderText").GetComponent<TextMeshProUGUI>();
            Request need = orderinfo.Needs[index];
            Request order = orderinfo.Order[index];
            ordertext.text = "Need: " + need.Amount + "\nOrder: " + order.Amount;
        }
    }


    public void Deny()
    {
        // temp text
        dialogue.text = "Denied NPC Food";
        SpawnNPC();
    }
}
