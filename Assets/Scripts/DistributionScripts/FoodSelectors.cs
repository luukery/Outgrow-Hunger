using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class FoodSelectors : MonoBehaviour
{
    private List<GameObject> Selectors = new();
    private GameObject moneySelect;
    private int maxMoney = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform child in transform)
        {
            Button minus = child.Find("MinusButton").GetComponent<Button>();
            Button plus = child.Find("PlusButton").GetComponent<Button>();
            TextMeshProUGUI number = child.Find("Number").GetComponent<TextMeshProUGUI>();

            minus.onClick.AddListener(() => MinusOne(number));
           

            if(child.gameObject.name == "MoneySelect")
            {
                plus.onClick.AddListener(() => MoneyPlusOne(number));
                moneySelect = child.gameObject;

            }
            else
            {
                plus.onClick.AddListener(() => PlusOne(number));
                Selectors.Add(child.gameObject);
            }

            child.gameObject.SetActive(false);
        }
    }

    public GameObject GetSelector(int index)
    {
        return Selectors[index];
    }

    public int GetValue(int index)
    {
        GameObject selector = Selectors[index];
        TextMeshProUGUI number = selector.transform.Find("Number").GetComponent<TextMeshProUGUI>();
        return int.Parse(number.text);
    }

    public int GetMoney()
    {
        TextMeshProUGUI number = moneySelect.transform.Find("Number").GetComponent<TextMeshProUGUI>();
        return int.Parse(number.text);
    }

    private void MinusOne(TextMeshProUGUI numberText)
    {
        int number = int.Parse(numberText.text);
        if (number > 0)
        {
            number--;
        }
        numberText.text = number.ToString();
    }


    private void PlusOne(TextMeshProUGUI numberText)
    {
        int number = int.Parse(numberText.text);
        // add logic max number
        if (number < 10)
        {
            number++;
        }
        numberText.text = number.ToString();
    }   

    private void MoneyPlusOne(TextMeshProUGUI numberText)
    {
        int number = int.Parse(numberText.text);
        // add logic max number
        if (number < maxMoney)
        {
            number++;
        }
        numberText.text = number.ToString();
    }

    public void HideSelectors()
    {
        foreach (GameObject selector in Selectors)
        {
            selector.gameObject.SetActive(false);   
        }
    }

    public void ResetValues()
    {
        foreach (GameObject selector in Selectors)
        {
            TextMeshProUGUI number = selector.transform.Find("Number").GetComponent<TextMeshProUGUI>();
            Image icon = selector.transform.Find("Icon").GetComponent<Image>();
            icon.gameObject.SetActive(false);
            number.text = "0";
        }

        TextMeshProUGUI money = moneySelect.transform.Find("Number").GetComponent<TextMeshProUGUI>();
        money.text = "0";
    }

    public void ChangeMaxMoney(int max)
    {
        maxMoney = max;
    }

    public void ShowHideMoneySelect(bool show)
    {
        moneySelect.gameObject.SetActive(show);
    }


    public void AddIcon(int index, Sprite newIcon)
    {
        GameObject selector = GetSelector(index);
        Image icon = selector.transform.Find("Icon").GetComponent<Image>();
        icon.gameObject.SetActive(true);
        icon.sprite = newIcon;
    }
}
