using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StallUISimple : MonoBehaviour
{
    // ✅ globale lock zodat maar 1 kraam tegelijk open kan
    public static bool IsAnyStallOpen { get; private set; }

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

    [Header("Speler coins (demo)")]
    public int playerCoins = 100;
    public TMP_Text coinsText;

    [Header("UI Tekst")]
    public TMP_Text headerText;
    public TMP_Text descText;

    void OnEnable()
    {
        // 🔒 UI lock actief
        IsAnyStallOpen = true;

        UpdateCoins();
        Refresh();
    }

    void OnDisable()
    {
        // 🔓 UI lock vrijgeven
        IsAnyStallOpen = false;
    }

    public void Refresh()
    {
        // verberg eerst alle slots
        for (int s = 0; s < slots.Count; s++)
            if (slots[s]?.root)
                slots[s].root.SetActive(false);

        if (stall == null)
        {
            Debug.LogWarning("[StallUI] Geen stall gekoppeld.");
            return;
        }

        // header
        if (headerText)
            headerText.text = stall.stallType + " Stall";

        // verhaal per kraam
        if (descText)
            descText.text = !string.IsNullOrWhiteSpace(stall.stallStory)
                ? stall.stallStory
                : "";

        List<MarketStockItem> stock = stall.currentStock;
        if (stock == null) return;

        for (int i = 0; i < stock.Count && i < slots.Count; i++)
        {
            var item = stock[i];
            var s = slots[i];

            if (s == null) continue;

            if (s.root) s.root.SetActive(true);

            if (s.icon)
            {
                s.icon.sprite = item.icon;
                bool hasIcon = item.icon != null;
                s.icon.enabled = hasIcon;

                var col = s.icon.color;
                col.a = hasIcon ? 1f : 0f;
                s.icon.color = col;
            }

            if (s.nameText) s.nameText.text = item.name;
            if (s.priceText) s.priceText.text = item.price + " coins";

            if (s.buyButton)
            {
                int idx = i;
                s.buyButton.onClick.RemoveAllListeners();
                s.buyButton.onClick.AddListener(() => TryBuy(idx));
            }
        }
    }

    void TryBuy(int index)
    {
        if (stall == null) return;
        if (index < 0 || index >= stall.currentStock.Count) return;

        var item = stall.currentStock[index];

        int availableCoins =
            Wallet.Instance != null ? Wallet.Instance.Money : playerCoins;

        if (availableCoins < item.price)
        {
            Debug.Log("[StallUI] Not enough coins");
            return;
        }

        if (Wallet.Instance != null)
            Wallet.Instance.CanSpendMoney(item.price);
        else
            playerCoins -= item.price;

        if (Inventory.Instance != null)
        {
            Food food = item.ToFood();
            Inventory.Instance.TryAddFoodToInventory(food);
        }

        UpdateCoins();
    }

    void UpdateCoins()
    {
        int coins =
            Wallet.Instance != null ? Wallet.Instance.Money : playerCoins;

        if (coinsText)
            coinsText.text = $"Coins: {coins}";
    }

    // ✅ aan close button hangen
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
