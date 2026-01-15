using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class Distributionmanager : MonoBehaviour
{
    public SpawnerScript spawner;
    public Canvas canvas;

    private Button confirmButton, cancelButton, continueButton;
    private TextMeshProUGUI dialogue, selecttext;

    private NPC currentNPC;
    private NpcInfoDTO npcDTO;

    public FoodSelectors foodselectors;

    [Header("Scene Flow")]
    public string marketSceneName = "MarketPlace";

    private Button returnButton;

    void Start()
    {
        confirmButton = canvas.transform.Find("ConfirmButton").GetComponent<Button>();
        cancelButton = canvas.transform.Find("CancelButton").GetComponent<Button>();
        continueButton = canvas.transform.Find("ContinueButton").GetComponent<Button>();

        selecttext = canvas.transform.Find("SelectText").GetComponent<TextMeshProUGUI>();
        dialogue = canvas.transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();

        Transform returnTf = canvas.transform.Find("ReturnButton");
        if (returnTf != null)
        {
            returnButton = returnTf.GetComponent<Button>();
            returnButton.gameObject.SetActive(false);
            returnButton.onClick.RemoveAllListeners();
            returnButton.onClick.AddListener(ReturnToMarket);
        }

        ChangeContinueButton(true);
        ChangeConfirmButtons(true);
        GetCurrentNPC();
    }


    private void OnDistributionFinished()
    {
        confirmButton.interactable = false;
        cancelButton.interactable = false;

        continueButton.onClick.RemoveAllListeners();
        continueButton.gameObject.SetActive(false);

        if (returnButton != null) returnButton.gameObject.SetActive(true);
        else ReturnToMarket();
    }

    private void ReturnToMarket()
    {
        LoadingManager.Instance.LoadScene(marketSceneName);
    }

    private bool GetCurrentNPC()
    {
        currentNPC = spawner.CurrentNPC;

        if (currentNPC == null)
        {
            OnDistributionFinished();
            return false;
        }

        npcDTO = currentNPC.GetInfoDTO();
        DisplayOrder();
        FoodSelector();
        return true;
    }

    private void DisplayOrder()
    {
        dialogue.text = "I want the following: ";
        dialogue.text += "<b>";
        foreach (Request order in npcDTO.Order)
            dialogue.text += order.Amount + " " + order.FoodType + " ";
        dialogue.text += "</b>";
        dialogue.text += "\nI can pay " + npcDTO.Money + " coins";
    }

    // moneyselect screen = true, confirm sends to confirm before delivery screen, cancel returns to money select
    // confirm delivery screen = false, if confirm it sends deliver, if false it goes back to prev screen
    private void ChangeConfirmButtons(bool confirmDelivery)
    {
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        if (confirmDelivery)
        {
            confirmButton.onClick.AddListener(ConfirmBeforeDelivery);
            cancelButton.onClick.AddListener(FoodSelector);
        }
        else
        {
            confirmButton.onClick.AddListener(SendDelivery);
            cancelButton.onClick.AddListener(FoodSelector);
        }
    }

    // foodselect screen = true, sends from food to money check
    // resultscreen = false, sends from resultscreen to new npc/finish
    private void ChangeContinueButton(bool foodSelect)
    {
        continueButton.onClick.RemoveAllListeners();


        if (foodSelect)
        {
            continueButton.onClick.AddListener(MoneyCheck);
        }
        else
        {
            continueButton.onClick.AddListener(ContinueAfterInteraction);
        }
    }

    private void ShowContinueOrCancelButtons(bool enable)
    {
        continueButton.gameObject.SetActive(enable);
        confirmButton.gameObject.SetActive(!enable);
        cancelButton.gameObject.SetActive(!enable);
    }

    private void FoodSelector()
    {
        ShowContinueOrCancelButtons(true);
        ChangeContinueButton(true);
        selecttext.gameObject.SetActive(false);
        foodselectors.ResetValues();

        for (int index = 0; index < npcDTO.Order.Count; index++)
        {
            GameObject selector = foodselectors.GetSelector(index);
            selector.SetActive(true);

            TextMeshProUGUI ordertext = selector.transform.Find("OrderText").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI foodtype = selector.transform.Find("TempFoodType").GetComponent<TextMeshProUGUI>();

            Request order = npcDTO.Order[index];
            Request need = npcDTO.Needs.Find(n => n.FoodType == order.FoodType);

            int needAmount = need != null ? need.Amount : 0;

            int available = Inventory.Instance != null
                ? Inventory.Instance.GetAvailableUnits(order.FoodType) // ✅ TYPE ONLY
                : 0;

            // ✅ You can only give what they need and what you have
            int maxGive = Mathf.Min(needAmount, available);

            // disable + / - if 0
            foodselectors.SetMaxForSelector(index, maxGive);

            ordertext.text = "Need: " + needAmount + "\nHave: " + available;
            foodtype.text = order.FoodType.ToString();
        }
    }



    private void MoneyCheck()
    {
        ShowContinueOrCancelButtons(false);
        foodselectors.HideSelectors();

        bool emptydelivery = true;
        for (int index = 0; index < npcDTO.Order.Count; index++)
        {
            int value = foodselectors.GetValue(index);
            if (value != 0) emptydelivery = false;

        }

        if (emptydelivery)
        {
            ConfirmEmptyDelivery();
        }
        else
        {
            MoneySelect();
        }
    }

    private void MoneySelect()
    {
        foodselectors.ShowHideMoneySelect(true);
        foodselectors.ChangeMaxMoney(npcDTO.Money);
        ChangeConfirmButtons(true);
    }

    private void ConfirmBeforeDelivery()
    {
        foodselectors.ShowHideMoneySelect(false);
        selecttext.gameObject.SetActive(true);


        selecttext.text = "Are you sure you want to give the following?\n";
        for (int index = 0; index < npcDTO.Order.Count; index++)
        {
            int amount = foodselectors.GetValue(index);
            if (amount != 0)
            {
                Request order = npcDTO.Order[index];
                selecttext.text += amount + " " + order.FoodType + "\n";
            }
        }

        selecttext.text += "\n\n For " + foodselectors.GetMoney() + " coins?";
        ChangeConfirmButtons(false);
    }

    private void ConfirmEmptyDelivery()
    {
        selecttext.gameObject.SetActive(true);
        selecttext.text = "Are you sure you don't want to give them anything?";
        ChangeConfirmButtons(false);
    }

    private void SendDelivery()
    {
        List<Request> intended = new();

        for (int index = 0; index < npcDTO.Order.Count; index++)
        {
            int value = foodselectors.GetValue(index);
            Request order = npcDTO.Order[index];

            intended.Add(new Request(value, order.FoodType, order.Quality)); // quality kept for DTO consistency
            
        }

        List<Request> delivered = new();
        bool deliveredAnything = false;

        Inventory inv = Inventory.Instance;

        for (int i = 0; i < intended.Count; i++)
        {
            Request r = intended[i];
            int deliveredAmount = r.Amount;

            if (inv != null && deliveredAmount > 0)
            {
                // ✅ TYPE ONLY removal
                deliveredAmount = inv.RemoveUnits(r.FoodType, deliveredAmount);
            }

            delivered.Add(new Request(deliveredAmount, r.FoodType, r.Quality));
            if (deliveredAmount > 0) deliveredAnything = true;
        }

        if (!deliveredAnything)
        {
            selecttext.text = "You have no food in those categories.";
            foodselectors.HideSelectors();
            ShowContinueOrCancelButtons(true);
            spawner.Despawn();
            ChangeContinueButton(false);
            return;
        }

        DeliveryResult result = currentNPC.Transaction(delivered);

        int earned = Mathf.Clamp(foodselectors.GetMoney(), 0, npcDTO.Money);
        if (Wallet.Instance != null && earned > 0)
            Wallet.Instance.AddMoney(earned);

        ShowResults(result);

        foodselectors.HideSelectors();
        ShowContinueOrCancelButtons(true);
        spawner.Despawn();
        ChangeContinueButton(false);
    }

    private void ContinueAfterInteraction()
    {
        bool success = GetCurrentNPC();
        if (!success)
            continueButton.onClick.RemoveAllListeners();
    }

    private void ShowResults(DeliveryResult result)
    {
        string resulttext = string.Empty;

        if (result.Shortages.Count != 0)
        {
            resulttext += "Amount of food you didn't give: " + result.Shortages.Count;
            foreach (Request shortage in result.Shortages)
                resulttext += "\n\t" + shortage.Amount + " " + shortage.FoodType;
            resulttext += "\n";
        }

        if (result.Excesses.Count != 0)
        {
            resulttext += "Amount of extra food you gave: " + result.Excesses.Count;
            foreach (Request excess in result.Excesses)
                resulttext += "\n\t" + excess.Amount + " " + excess.FoodType;
            resulttext += "\n";
        }

        resulttext += "Money earned: " + foodselectors.GetMoney() + " coins";
        selecttext.text = resulttext;
    }
}
