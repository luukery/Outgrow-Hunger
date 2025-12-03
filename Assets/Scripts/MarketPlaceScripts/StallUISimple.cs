using System.Collections.Generic;
using System.Collections;              // voor IEnumerator / coroutines
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StallUISimple : MonoBehaviour
{
    [System.Serializable]
    public class Slot
    {
        public GameObject root;
        public Image icon;
        public TMP_Text nameText;      // TMP i.p.v. Text
        public TMP_Text priceText;     // TMP i.p.v. Text
        public Button buyButton;
    }

    [Header("Link naar kraam")]
    public MarketStallSimple stall;

    [Header("Slots (max 2)")]
    public List<Slot> slots = new();   // vul in Inspector (2 stuks)

    [Header("Speler coins (demo)")]
    public int playerCoins = 100;
    public TMP_Text coinsText;         // TMP-tekst voor Coins

    void OnEnable()
    {
        UpdateCoins();
        Refresh();
    }

    /// <summary>
    /// Vult/refresh de UI-slots vanuit stall.currentStock.
    /// </summary>
    public void Refresh()
    {
        // verberg alles eerst
        foreach (var s in slots)
            if (s.root) s.root.SetActive(false);

        if (stall == null) return;

        var stock = stall.currentStock;

        for (int i = 0; i < stock.Count && i < slots.Count; i++)
        {
            var item = stock[i];
            var s = slots[i];

            if (s.root) s.root.SetActive(true);
            if (s.icon) s.icon.sprite = item.icon;
            if (s.nameText) s.nameText.text = item.name;
            if (s.priceText) s.priceText.text = item.price + " coins";

            if (s.buyButton)
            {
                int idx = i; // capture voor lambda
                s.buyButton.onClick.RemoveAllListeners();
                s.buyButton.onClick.AddListener(() => TryBuy(idx));
            }
        }
    }

    /// <summary>
    /// Koop het item op index. Item blijft zichtbaar (verdwijnt NIET).
    /// </summary>
    void TryBuy(int index)
    {
        if (stall == null) return;
        if (index < 0 || index >= stall.currentStock.Count) return;

        var item = stall.currentStock[index];

        if (playerCoins >= item.price)
        {
            playerCoins -= item.price;
            UpdateCoins();
            Debug.Log($"Gekocht: {item.name} voor {item.price} coins");

            // Item niet verwijderen; je kunt het opnieuw kopen.
            // Kleine visuele feedback op de koopknop:
            if (index < slots.Count && slots[index].buyButton != null)
                StartCoroutine(FlashButton(slots[index].buyButton));
        }
        else
        {
            Debug.Log("Niet genoeg coins!");
        }
    }

    void UpdateCoins()
    {
        if (coinsText) coinsText.text = $"Coins: {playerCoins}";
    }

    IEnumerator FlashButton(Button b)
    {
        if (b == null) yield break;

        // Korte kleur-flash als bevestiging
        var colors = b.colors;
        Color original = colors.normalColor;

        colors.normalColor = Color.green;
        b.colors = colors;

        yield return new WaitForSeconds(0.15f);

        colors.normalColor = original;
        b.colors = colors;
    }
}
