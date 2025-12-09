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
    private NpcInfoDTO npcDTO;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpawnNPC();

        feedButton = canvas.transform.Find("FeedButton").GetComponent<Button>();
        denyButton = canvas.transform.Find("DenyButton").GetComponent<Button>();
        dialogue = canvas.transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();
        foodselect = canvas.transform.Find("FoodSelect").gameObject;


        for (int i = 0; i < 7 ; i++)
        {
            string dropdownname = "Dropdown" + i;
            TMP_Dropdown dropdown = foodselect.transform.Find(dropdownname).GetComponent<TMP_Dropdown>();
            dropdowns.Add(dropdown);
            dropdown.gameObject.SetActive(false);
        }

        foodselect.SetActive(false);

        ChangeButtonFunction(false);
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

    private void ChangeButtonFunction(bool select)
    {
        feedButton.onClick.RemoveAllListeners();
        denyButton.onClick.RemoveAllListeners();

        TextMeshProUGUI feedbuttonText = feedButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI denybuttonText = denyButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

        if (select)
        {
            feedbuttonText.text = "Give food";
            denybuttonText.text = "Cancel selection";

            feedButton.onClick.AddListener(() => SendDelivery(npcDTO));
            denyButton.onClick.AddListener(CancelSelection);
        }
        else
        {
            feedbuttonText.text = "Select food";
            denybuttonText.text = "Deny food";

            feedButton.onClick.AddListener(HandleAccept);
            denyButton.onClick.AddListener(Deny);
        }
    }

    private void CancelSelection()
    {
        Debug.Log("Selection cancelled");
        foodselect.SetActive(false);
        dialogue.gameObject.SetActive(true);
        dialogue.text = "cancelled selection";
        ChangeButtonFunction(false);
    }

    private void FoodSelector()
    {
        // steps
        // gray out food that dont have enough of 
        // button deny function
        // need 7 dropdowns to make this work without needing the break 

        // buttons needs to be changed back later
        foodselect.SetActive(true);
        // dialogue needs to be reenabled eventually
        dialogue.gameObject.SetActive(false);

        List<Request> npcneeds = new();
        List<Request> npcorders = new();

        npcDTO = currentNPC.GetInfoDTO();
        // cant do a foreach loop bc there's multiple lists in dto 
        // skip if need = 0
        for (int index = 0; index < npcDTO.Needs.Count; index++)
        {
            Request dtoneed = npcDTO.Needs[index];
            if (dtoneed.Amount != 0)
            {
                npcneeds.Add(dtoneed);
                npcorders.Add(npcDTO.Order[index]);
            }
        }


        for (int index = 0; index < npcneeds.Count; index++)
        {
            TMP_Dropdown dropdown = dropdowns[index];
            dropdown.gameObject.SetActive(true);

            TextMeshProUGUI ordertext = dropdown.transform.Find("OrderText").GetComponent<TextMeshProUGUI>();
            // change icon to order food type


            Request need = npcneeds[index];
            Request order = npcorders[index];
            ordertext.text = "Need: " + need.Amount + "\nOrder: " + order.Amount;
            dropdown.ClearOptions();
            List<string> options = new();
            for (int a = 0; a <= need.Amount; a++)
            {
                options.Add(a.ToString());
            }
            dropdown.AddOptions(options);
        }

        ChangeButtonFunction(true);
    }


    private void SendDelivery(NpcInfoDTO npcDTO)
    {
        List<Request> requests = new();

        for (int index = 0; index < npcDTO.Needs.Count; index++)
        {
            TMP_Dropdown dropdown = dropdowns[index];

            Request need = npcDTO.Needs[index];

            // geen idee hoe we quality gaan handelen, voor nu is het temp
            Request sendrequest = new(dropdown.value, need.FoodType, need.Quality);
            requests.Add(sendrequest);
        }

        Debug.Log("NPC recieved delivery");
        currentNPC.Transaction(requests);

        foodselect.SetActive(false);
        dialogue.gameObject.SetActive(true);
        SpawnNPC();
        ChangeButtonFunction(false);
    }


    public void Deny()
    {
        // temp text
        dialogue.text = "Denied NPC Food";
        SpawnNPC();
    }
}
