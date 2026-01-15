using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class Distributionmanager : MonoBehaviour
{
    public SpawnerScript spawner;
    public Canvas canvas;

    private Button feedButton, denyButton, continueButton;
    private TextMeshProUGUI selecttext;

    private NPC currentNPC;
    private NpcInfoDTO npcDTO;

    public FoodSelectors foodselectors;

    [Header("Scene Flow")]
    public string marketSceneName = "MarketPlace";

    private Button returnButton;

    void Start()
    {
        feedButton = canvas.transform.Find("FeedButton").GetComponent<Button>();
        denyButton = canvas.transform.Find("DenyButton").GetComponent<Button>();
        continueButton = canvas.transform.Find("ContinueButton").GetComponent<Button>();

        continueButton.onClick.AddListener(ContinueAfterInteraction);
        continueButton.gameObject.SetActive(false);

        selecttext = canvas.transform.Find("SelectText").GetComponent<TextMeshProUGUI>();

        Transform returnTf = canvas.transform.Find("ReturnButton");
        if (returnTf != null)
        {
            returnButton = returnTf.GetComponent<Button>();
            returnButton.gameObject.SetActive(false);
            returnButton.onClick.RemoveAllListeners();
            returnButton.onClick.AddListener(ReturnToMarket);
        }

        ChangeButtonFunction(1);
        GetCurrentNPC();
    }


    private void OnDistributionFinished()
    {
        feedButton.interactable = false;
        denyButton.interactable = false;

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
        return true;
    }

    private void HandleAccept()
    {
        selecttext.text = "Accepted food";
        FoodSelector();
    }

    private void DisplayOrder()
    {
        selecttext.text = "I want the following:\n";
        foreach (Request order in npcDTO.Order)
            selecttext.text += order.Amount + " " + order.FoodType + "\n";

        selecttext.text += "\nI can pay " + npcDTO.Money + " coins";
    }

    private void ChangeButtonFunction(int select)
    {
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
                break;

            case 2:
                feedbuttonText.text = "Next";
                denybuttonText.text = "Cancel selection";
                feedButton.onClick.AddListener(MoneyCheck);
                denyButton.onClick.AddListener(CancelSelection);
                break;

            case 3:
                feedbuttonText.text = "Confirm";
                denybuttonText.text = "Return";
                feedButton.onClick.AddListener(ConfirmBeforeDelivery);
                denyButton.onClick.AddListener(CancelSelection);
                break;

            case 4:
                feedbuttonText.text = "Send";
                denybuttonText.text = "Back";
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
        foodselectors.HideSelectors();
        foodselectors.ShowHideMoneySelect(false);
        selecttext.gameObject.SetActive(true);
        DisplayOrder();
        ChangeButtonFunction(1);
    }

    private void FoodSelector()
    {
        selecttext.gameObject.SetActive(false);
        foodselectors.ResetValues();
        bool emptydelivery = true;

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
        ChangeButtonFunction(2);
    }



    private void MoneyCheck()
    {
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
        ChangeButtonFunction(3);
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
        ChangeButtonFunction(4);
    }

    private void ConfirmEmptyDelivery()
    {
        selecttext.gameObject.SetActive(true);
        selecttext.text = "Are you sure you don't want to give them anything?";
        ChangeButtonFunction(4);
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

        ChangeButtonFunction(1);

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
            EnableDisableConfirmButton(true);
            spawner.Despawn();
            return;
        }

        DeliveryResult result = currentNPC.Transaction(delivered);

        int earned = Mathf.Clamp(foodselectors.GetMoney(), 0, npcDTO.Money);
        if (Wallet.Instance != null && earned > 0)
            Wallet.Instance.AddMoney(earned);

        ShowResults(result);

        foodselectors.HideSelectors();
        EnableDisableConfirmButton(true);
        spawner.Despawn();
    }

    private void ContinueAfterInteraction()
    {
        bool success = GetCurrentNPC();
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
            resulttext += "Amount of food you didn't give: " + result.TotalFoodShortage;
            foreach (Request shortage in result.Shortages)
                resulttext += "\n\t" + shortage.Amount + " " + shortage.FoodType;
            resulttext += "\n";
        }

        if (result.TotalFoodExcess != 0)
        {
            resulttext += "Amount of extra food you gave: " + result.TotalFoodExcess;
            foreach (Request excess in result.Excesses)
                resulttext += "\n\t" + excess.Amount + " " + excess.FoodType;
            resulttext += "\n";
        }

        resulttext += "Money earned: " + foodselectors.GetMoney() + " coins";
        selecttext.text = resulttext;
    }
}
