using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Distributionmanager : MonoBehaviour
{
    public SpawnerScript spawner;
    public Canvas canvas;

    private Button feedButton, denyButton, continueButton;
    private TextMeshProUGUI dialogue;
    private GameObject foodselect;
    private NPC currentNPC;
    private NpcInfoDTO npcDTO;
    private int npcSpawnCount = 0;
    public int maxNPCs = 2;

    public FoodSelectors foodselectors;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        feedButton = canvas.transform.Find("FeedButton").GetComponent<Button>();
        denyButton = canvas.transform.Find("DenyButton").GetComponent<Button>();
        continueButton = canvas.transform.Find("ContinueButton").GetComponent<Button>();
        continueButton.onClick.AddListener(ContinueAfterInteraction);
        continueButton.gameObject.SetActive(false);

        dialogue = canvas.transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();
        ChangeButtonFunction(1);

        TrySpawnNPC();
    }

    public bool TrySpawnNPC()
    {
        Debug.Log(npcSpawnCount);
        // needs to also stop if there's not any food in inventory eventually
        if (npcSpawnCount < maxNPCs /* || inventory = empty*/)
        {
            SpawnNPC();
            return true;
        }
        else
        {
            dialogue.text = "No more NPCs being spawned";
            return false;
        }
    }

    private void SpawnNPC()
    {
        currentNPC = spawner.SpawnNPC();
        Debug.Log("Spawned NPC");
        npcSpawnCount++;
    }

    private void DespawnNPC()
    {
        Destroy(currentNPC.gameObject);
        currentNPC = null;
        Debug.Log("Despawned NPC");
    }

    private void HandleAccept()
    {
        dialogue.text = "Accepted food";
        FoodSelector();
        // Check if food can be delivered
    }

    private void ChangeButtonFunction(int select)
    {
        // Select 1 = default
        // Select 2 = selecting food
        // Select 3 = confirm deny
        
        feedButton.onClick.RemoveAllListeners();
        denyButton.onClick.RemoveAllListeners();

        TextMeshProUGUI feedbuttonText = feedButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI denybuttonText = denyButton.transform.Find("Text (TMP)").GetComponent<TextMeshProUGUI>();

        switch (select)
        {
            case 1:
                feedbuttonText.text = "Select food";
                denybuttonText.text = "Deny food";

                feedButton.onClick.AddListener(HandleAccept);
                denyButton.onClick.AddListener(Deny);
                break;
            case 2:
                feedbuttonText.text = "Give food";
                denybuttonText.text = "Cancel selection";

                feedButton.onClick.AddListener(ConfirmBeforeDelivery);
                denyButton.onClick.AddListener(CancelSelection);
                break;
            case 3:
                feedbuttonText.text = "Confirm";
                denybuttonText.text = "Return";

                feedButton.onClick.AddListener(SendDelivery);
                denyButton.onClick.AddListener(HandleAccept);
                break;
                
        }       
    }

    private void ConfirmBeforeDelivery()
    {
        dialogue.gameObject.SetActive(true);
        foodselectors.HideSelectors();

        dialogue.text = "Are you sure you want to give the following?\n";
        for (int index = 0; index < npcDTO.Needs.Count; index++)
        {
            int amount = foodselectors.GetValue(index);
            if (amount != 0)
            {
                Request need = npcDTO.Needs[index];
                dialogue.text += amount + " " + need.FoodType + "\n";
            }
        }

        ChangeButtonFunction(3);
    }

    private void EnableDisableConfirmButton(bool enable)
    {
        continueButton.gameObject.SetActive(enable);
        feedButton.gameObject.SetActive(!enable);
        denyButton.gameObject.SetActive(!enable);
    }

    private void CancelSelection()
    {
        Debug.Log("Selection cancelled");
        foodselectors.HideSelectors();
        dialogue.gameObject.SetActive(true);
        dialogue.text = "cancelled selection";
        ChangeButtonFunction(1);
    }

    private void FoodSelector()
    {
        // steps
        // gray out food that dont have enough of 
        // dialogue needs to be reenabled eventually
        dialogue.gameObject.SetActive(false);
        foodselectors.ResetValues();

        List<Request> npcneeds = new();
        List<Request> npcorders = new();

        npcDTO = currentNPC.GetInfoDTO();
        // cant do a foreach loop bc there's multiple lists in dto 
        // skip if need = 0 so it only shows food the npc needs
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
            GameObject selector = foodselectors.GetSelector(index);
            selector.SetActive(true);

            TextMeshProUGUI ordertext = selector.transform.Find("OrderText").GetComponent<TextMeshProUGUI>();
            Image icon = selector.transform.Find("Icon").GetComponent<Image>();

            Request need = npcneeds[index];
            Request order = npcorders[index];

            ordertext.text = "Need: " + need.Amount + "\nOrder: " + order.Amount;
            // icon.sprite = order.FoodType;
        }

        ChangeButtonFunction(2);
    }


    private void SendDelivery()
    {
        List<Request> requests = new();
        bool emptydelivery = true;

        for (int index = 0; index < npcDTO.Needs.Count; index++)
        {
            int value = foodselectors.GetValue(index);

            Request need = npcDTO.Needs[index];

            // geen idee hoe we quality gaan handelen, voor nu is het temp
            Request sendrequest = new(value, need.FoodType, need.Quality);
            requests.Add(sendrequest);

            if (value != 0)
            {
                emptydelivery = false;
            }
        }

        ChangeButtonFunction(1);

        // if players give nothing, it'll go through the denial process instead
        if (emptydelivery)
        {
            Deny();
            foodselect.SetActive(false);
            return;
        }

        ShowResults(currentNPC.Transaction(requests));
        foodselectors.HideSelectors();

        EnableDisableConfirmButton(true);
        DespawnNPC();
    }

    private void ContinueAfterInteraction()
    {
        bool success = TrySpawnNPC();
        if (success)
            EnableDisableConfirmButton(false);
        else
            continueButton.onClick.RemoveAllListeners();

    }

    private void ShowResults(DeliveryResult result)
    {
        string resulttext = string.Empty;
        
        if (result.TotalFoodShortage != 0)
        {
            resulttext += "Amount of food you didn't give: " + result.TotalFoodShortage + "\n";
        }
        if (result.TotalFoodExcess != 0)
        {
            resulttext += "Amount of extra food you gave: " + result.TotalFoodExcess + "\n";
        }
        resulttext += "Money earned: $" + result.AmountPaid;

        dialogue.text = resulttext;
    }


    public void Deny()
    {
        // temp text
        dialogue.text = "Denied NPC Food";
        DespawnNPC();
        EnableDisableConfirmButton(true);
    }
}
