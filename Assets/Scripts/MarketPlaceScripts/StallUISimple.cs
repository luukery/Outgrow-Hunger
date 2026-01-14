using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StallUISimple : MonoBehaviour
{
    [System.Serializable]
    public class Slot
    {
        public GameObject root;
        public Image icon;
        public TMP_Text nameText;
        public TMP_Text priceText;
        public Button buyButton;
    }

    [Header("Link naar kraam")]
    public MarketStallFromCatalog stall;

    [Header("Slots (max 2)")]
    public List<Slot> slots = new();

    [Header("UI")]
    public TMP_Text coinsText;
    public TMP_Text headerText;
    public TMP_Text descText;

    void OnEnable()
    {
        UpdateCoins();
        Refresh();
    }

    public void Refresh()
    {
        for (int s = 0; s < slots.Count; s++)
            if (slots[s]?.root) slots[s].root.SetActive(false);

        if (stall == null)
        {
            Debug.LogWarning("[StallUI] Geen stall gekoppeld aan UI.");
            return;
        }

        if (headerText)
            headerText.text = stall.stallType.ToString() + " Stall";

        List<MarketStockItem> stock = stall.currentStock;
        if (stock == null)
        {
            Debug.LogWarning("[StallUI] stall.currentStock == null");
            return;
        }

        int shown = 0;
        for (int i = 0; i < stock.Count && i < slots.Count; i++)
        {
            var item = stock[i];
            var s = slots[i];
            if (s == null) continue;

            if (s.root) s.root.SetActive(true);

            if (s.icon)
            {
                s.icon.sprite = item != null ? item.icon : null;
                bool hasIcon = item != null && item.icon != null;
                s.icon.enabled = hasIcon;

                var col = s.icon.color;
                col.a = hasIcon ? 1f : 0f;
                s.icon.color = col;
            }

            if (s.nameText) s.nameText.text = item != null ? item.name : "(null)";
            if (s.priceText) s.priceText.text = item != null ? (item.price + " coins") : "-";

            if (s.buyButton)
            {
                int idx = i;
                s.buyButton.onClick.RemoveAllListeners();
                s.buyButton.onClick.AddListener(() => TryBuy(idx));
            }

            shown++;
        }

        if (shown == 0)
            Debug.LogWarning("[StallUI] 0 slots getoond. Is currentStock gevuld?");
    }

    void TryBuy(int index)
    {
        if (stall == null) return;
        if (index < 0 || index >= stall.currentStock.Count) return;

        if (Wallet.Instance == null || Inventory.Instance == null)
        {
            Debug.LogWarning("[StallUI] Missing Wallet or Inventory (make sure they exist in the first scene).");
            return;
        }

        var item = stall.currentStock[index];

        // Spend
        if (!Wallet.Instance.CanSpendMoney(item.price))
        {
            Debug.Log("[StallUI] Niet genoeg coins!");
            return;
        }

        // Convert MarketStockItem -> Food (includes spoilage data)
        Food bought = item.ToFood();

        // Add to inventory (refund if full)
        if (!Inventory.Instance.TryAddFoodToInventory(bought))
        {
            Wallet.Instance.AddMoney(item.price);
            Debug.Log("[StallUI] Inventory full - refunded.");
            UpdateCoins();
            return;
        }

        UpdateCoins();
        Debug.Log($"[StallUI] Gekocht: {item.name} ({item.foodType}, {item.foodQuality}, size {item.size}) voor {item.price} coins");

        if (index < slots.Count && slots[index].buyButton != null)
            StartCoroutine(FlashButton(slots[index].buyButton));
    }

    void UpdateCoins()
    {
        int money = Wallet.Instance != null ? Wallet.Instance.Money : 0;
        if (coinsText) coinsText.text = $"Coins: {money}";
    }

    IEnumerator FlashButton(Button b)
    {
        var c = b.colors;
        var old = c.normalColor;
        c.normalColor = Color.green;
        b.colors = c;
        yield return new WaitForSeconds(0.15f);
        c.normalColor = old;
        b.colors = c;
    }
}
