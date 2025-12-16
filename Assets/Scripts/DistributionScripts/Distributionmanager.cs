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
    public int maxNPCs = 20;

    public FoodSelectors foodselectors;

    void Start()
    {
        feedButton = canvas.transform.Find("FeedButton").GetComponent<Button>();
        denyButton = canvas.transform.Find("DenyButton").GetComponent<Button>();
        continueButton = canvas.transform.Find("ContinueButton").GetComponent<Button>();
        continueButton.onClick.AddListener(ContinueAfterInteraction);
        continueButton.gameObject.SetActive(false);
        dialogue = canvas.transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();

        // 🔧 FIX: foodselect initialiseren
        //foodselect = canvas.transform.Find("FoodSelect").gameObject;

        ChangeButtonFunction(1);
        TrySpawnNPC();
    }

    public bool TrySpawnNPC()
    {
        Debug.Log(npcSpawnCount);
        if (npcSpawnCount < maxNPCs)
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

    private void EnableDisableConfirmButton(bool enable)
    {
        continueButton.gameObject.SetActive(enable);
        feedButton.gameObject.SetActive(!enable);
        denyButton.gameObject.SetActive(!enable);
    }

    private void CancelSelection()
    {
        Debug.Log("Selection cancelled");
        foodselect.SetActive(false);
        dialogue.gameObject.SetActive(true);
        dialogue.text = "cancelled selection";
        ChangeButtonFunction(1);
    }

    private void FoodSelector()
    {
        dialogue.gameObject.SetActive(false);
        foodselectors.ResetValues();

        List<Request> npcneeds = new();
        List<Request> npcorders = new();

        npcDTO = currentNPC.GetInfoDTO();
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

    private void SendDelivery()
    {
        List<Request> requests = new();
        bool emptydelivery = true;

        for (int index = 0; index < npcDTO.Order.Count; index++)
        {
            int value = foodselectors.GetValue(index);
            Request order = npcDTO.Order[index];

            Request sendrequest = new Request(value, order.FoodType, order.Quality);
            requests.Add(sendrequest);

            if (value != 0)
            {
                emptydelivery = false;
            }
        }

        ChangeButtonFunction(1);


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
            resulttext += "Amount of food you didn't give: " + result.TotalFoodShortage + "\n";

        if (result.TotalFoodExcess != 0)
            resulttext += "Amount of extra food you gave: " + result.TotalFoodExcess + "\n";

        resulttext += "Money earned: $" + result.AmountPaid;
        dialogue.text = resulttext;
    }

    public void Deny()
    {
        dialogue.text = "Denied NPC Food";
        DespawnNPC();
        EnableDisableConfirmButton(true);
    }
}
