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
    private List<TMP_Dropdown> dropdowns = new();

    private NPC currentNPC;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnNPC();

        feedButton = canvas.transform.Find("FeedButton").GetComponent<Button>();
        denyButton = canvas.transform.Find("DenyButton").GetComponent<Button>();
        dialogue = canvas.transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();
        foodselect = canvas.transform.Find("FoodSelect").gameObject;

        foreach (Transform child in foodselect.transform)
        {
            Debug.Log("Child: " + child.name);
        }


        for (int i = 0; i <= 4 ; i++)
        {
            string dropdownname = "Dropdown" + i;
            TMP_Dropdown dropdown = foodselect.transform.Find(dropdownname).GetComponent<TMP_Dropdown>();
            dropdowns.Add(dropdown);
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
        if (currentNPC != null)
        {
            Destroy(currentNPC.gameObject);
            currentNPC = null;
            Debug.Log("Despawned NPC");
        }
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
            TMP_Dropdown dropdown = dropdowns[index];
            // fix later
            TextMeshProUGUI ordertext = dropdown.transform.Find("OrderText").GetComponent<TextMeshProUGUI>();
            Request need = orderinfo.Needs[index];
            Request order = orderinfo.Order[index];
            ordertext.text = "Need: " + need.Amount + "\nOrder: " + order.Amount;
            dropdown.ClearOptions();
            for (int a = 0; a < need.Amount; a++)
            {
                dropdown.AddOptions(a);
            }
        }
        // listener needs to be readded
        feedButton.onClick.RemoveListener(HandleAccept);
        feedButton.onClick.AddListener(SendDelivery);
    }


    private void SendDelivery()
    {
        List<Request> requests = new();

        foreach (TMP_Dropdown dropdown in dropdowns)
        {
            // Request request = new(dropdown.value);
        }

        currentNPC.ReceiveDelivery(requests);
    }


    public void Deny()
    {
        // temp text
        dialogue.text = "Denied NPC Food";
        SpawnNPC();
    }
}
