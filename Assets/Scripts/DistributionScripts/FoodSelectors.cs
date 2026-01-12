using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class FoodSelectors : MonoBehaviour
{
    private readonly List<GameObject> Selectors = new();
    private readonly List<int> maxPerSelector = new();
    private readonly List<Button> plusButtons = new();
    private readonly List<Button> minusButtons = new();

    private GameObject moneySelect;
    private int maxMoney = 5;

    void Start()
    {
        Selectors.Clear();
        maxPerSelector.Clear();
        plusButtons.Clear();
        minusButtons.Clear();
        moneySelect = null;

        foreach (Transform child in transform)
        {
            Button minus = child.Find("MinusButton").GetComponent<Button>();
            Button plus = child.Find("PlusButton").GetComponent<Button>();
            TextMeshProUGUI number = child.Find("Number").GetComponent<TextMeshProUGUI>();

            if (child.gameObject.name == "MoneySelect")
            {
                moneySelect = child.gameObject;

                minus.onClick.RemoveAllListeners();
                plus.onClick.RemoveAllListeners();

                minus.onClick.AddListener(() => MinusOne(number));
                plus.onClick.AddListener(() => MoneyPlusOne(number));
            }
            else
            {
                int selectorIndex = Selectors.Count;

                Selectors.Add(child.gameObject);
                maxPerSelector.Add(0);

                plusButtons.Add(plus);
                minusButtons.Add(minus);

                minus.onClick.RemoveAllListeners();
                plus.onClick.RemoveAllListeners();

                minus.onClick.AddListener(() => MinusOne(number));
                plus.onClick.AddListener(() => PlusOne(selectorIndex, number));
            }

            child.gameObject.SetActive(false);
        }
    }

    public GameObject GetSelector(int index) => Selectors[index];

    public int GetValue(int index)
    {
        GameObject selector = Selectors[index];
        TextMeshProUGUI number = selector.transform.Find("Number").GetComponent<TextMeshProUGUI>();
        return int.TryParse(number.text, out int parsed) ? parsed : 0;
    }

    public int GetMoney()
    {
        if (moneySelect == null) return 0;
        TextMeshProUGUI number = moneySelect.transform.Find("Number").GetComponent<TextMeshProUGUI>();
        return int.TryParse(number.text, out int parsed) ? parsed : 0;
    }

    private void MinusOne(TextMeshProUGUI numberText)
    {
        if (numberText == null) return;
        int number = int.TryParse(numberText.text, out int parsed) ? parsed : 0;
        if (number > 0) number--;
        numberText.text = number.ToString();
    }

    private void PlusOne(int selectorIndex, TextMeshProUGUI numberText)
    {
        if (numberText == null) return;

        int number = int.TryParse(numberText.text, out int parsed) ? parsed : 0;
        int max = GetMaxForSelector(selectorIndex);

        if (number < max) number++;
        numberText.text = number.ToString();
    }

    private void MoneyPlusOne(TextMeshProUGUI numberText)
    {
        if (numberText == null) return;

        int number = int.TryParse(numberText.text, out int parsed) ? parsed : 0;
        if (number < maxMoney) number++;
        numberText.text = number.ToString();
    }

    public void HideSelectors()
    {
        foreach (GameObject selector in Selectors)
            if (selector != null) selector.SetActive(false);
    }

    public void ResetValues()
    {
        for (int i = 0; i < Selectors.Count; i++)
        {
            GameObject selector = Selectors[i];
            if (selector == null) continue;

            TextMeshProUGUI number = selector.transform.Find("Number").GetComponent<TextMeshProUGUI>();
            if (number != null) number.text = "0";

            Transform iconTf = selector.transform.Find("Icon");
            if (iconTf != null)
            {
                Image icon = iconTf.GetComponent<Image>();
                if (icon != null) icon.gameObject.SetActive(false);
            }

            // default interactive until Distribution sets max
            SetSelectorInteractable(i, true);
        }

        if (moneySelect != null)
        {
            TextMeshProUGUI money = moneySelect.transform.Find("Number").GetComponent<TextMeshProUGUI>();
            if (money != null) money.text = "0";
        }
    }

    public void ChangeMaxMoney(int max)
    {
        maxMoney = Mathf.Max(0, max);

        if (moneySelect != null)
        {
            TextMeshProUGUI money = moneySelect.transform.Find("Number").GetComponent<TextMeshProUGUI>();
            if (money != null && int.TryParse(money.text, out int current) && current > maxMoney)
                money.text = maxMoney.ToString();
        }
    }

    public void ShowHideMoneySelect(bool show)
    {
        if (moneySelect != null)
            moneySelect.SetActive(show);
    }

    public void AddIcon(int index, Sprite newIcon)
    {
        GameObject selector = GetSelector(index);
        Transform iconTf = selector.transform.Find("Icon");
        if (iconTf == null) return;

        Image icon = iconTf.GetComponent<Image>();
        if (icon == null) return;

        icon.gameObject.SetActive(true);
        icon.sprite = newIcon;
    }

    // ✅ Called by Distribution to cap selection (and disable when 0)
    public void SetMaxForSelector(int index, int max)
    {
        if (index < 0 || index >= maxPerSelector.Count) return;

        max = Mathf.Max(0, max);
        maxPerSelector[index] = max;

        // clamp current value
        GameObject selector = Selectors[index];
        if (selector != null)
        {
            TextMeshProUGUI number = selector.transform.Find("Number").GetComponent<TextMeshProUGUI>();
            if (number != null && int.TryParse(number.text, out int current) && current > max)
                number.text = max.ToString();
        }

        // disable if max == 0
        SetSelectorInteractable(index, max > 0);
    }

    public int GetMaxForSelector(int index)
    {
        if (index < 0 || index >= maxPerSelector.Count) return 0;
        return maxPerSelector[index];
    }

    private void SetSelectorInteractable(int index, bool enabled)
    {
        if (index < 0 || index >= plusButtons.Count) return;
        if (plusButtons[index] != null) plusButtons[index].interactable = enabled;
        if (minusButtons[index] != null) minusButtons[index].interactable = enabled;
    }
}
