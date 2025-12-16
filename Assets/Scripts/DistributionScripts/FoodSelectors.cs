using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class FoodSelectors : MonoBehaviour
{
    private List<GameObject> Selectors = new();
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (Transform child in transform)
        {
            Selectors.Add(child.gameObject);

            Button minus = child.Find("MinusButton").GetComponent<Button>();
            Button plus = child.Find("PlusButton").GetComponent<Button>();
            TextMeshProUGUI number = child.Find("Number").GetComponent<TextMeshProUGUI>();

            minus.onClick.AddListener(() => MinusOne(number));
            plus.onClick.AddListener(() => PlusOne(number));

            child.gameObject.SetActive(false);
        }

        Debug.Log("Selector children: " + Selectors.Count);
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
            number.text = "0";
        }
    }
}
