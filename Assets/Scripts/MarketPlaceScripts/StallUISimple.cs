using System.Collections.Generic;
using System.Collections;
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

    // (optioneel) header/desc, koppel in Inspector als je ze hebt
    public TMP_Text headerText;
    public TMP_Text descText;

    void OnEnable()
    {
        UpdateCoins();
        Refresh();
    }

    public void Refresh()
    {
        // verberg alles eerst
        for (int s = 0; s < slots.Count; s++)
            if (slots[s]?.root) slots[s].root.SetActive(false);

        if (stall == null)
        {
            Debug.LogWarning("[StallUI] Geen stall gekoppeld aan UI.");
            return;
        }

        // (optioneel) dynamische header
        if (headerText)
        {
            string title = stall.stallType.ToString() + " Stall";
            headerText.text = title;
        }

        if (descText)
            descText.text = !string.IsNullOrWhiteSpace(stall.stallStory) ? stall.stallStory : "";



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
            if (s == null)
            {
                Debug.LogWarning($"[StallUI] Slot {i} is null in slots-lijst.");
                continue;
            }

            // --- DEBUG: log alle relevante refs ---
            string itemIconName = (item != null && item.icon != null) ? item.icon.name : "NULL";
            string slotIconRef  = (s.icon != null) ? s.icon.name : "NULL";
            Debug.Log($"[StallUI] Slot {i}: item='{item?.name ?? "NULL"}', itemIcon={itemIconName}, slotIconRef={slotIconRef}");

            if (s.root) s.root.SetActive(true);

            // Icon veilig zetten (en geen wit blok als item geen sprite heeft)
            if (s.icon)
            {
                s.icon.sprite = (item != null) ? item.icon : null;
                bool hasIcon = (item != null && item.icon != null);

                // toggle Image enabled/alpha zodat je geen witte placeholder ziet
                s.icon.enabled = hasIcon;
                var col = s.icon.color;
                col.a = hasIcon ? 1f : 0f;
                s.icon.color = col;

                // handige waarschuwing specifiek voor het 2e slot-issue
                if (!hasIcon)
                {
                    Debug.LogWarning($"[StallUI] Slot {i} heeft GEEN item.icon. Check ProductCatalog entry voor '{item?.name}'.");
                }
            }
            else
            {
                Debug.LogWarning($"[StallUI] Slot {i} mist de Image-referentie (slots[{i}].icon is niet gevuld in de Inspector).");
            }

            if (s.nameText)  s.nameText.text  = (item != null) ? item.name : "(null)";
            if (s.priceText) s.priceText.text = (item != null) ? (item.price + " coins") : "-";

            if (s.buyButton)
            {
                int idx = i; // capture
                s.buyButton.onClick.RemoveAllListeners();
                s.buyButton.onClick.AddListener(() => TryBuy(idx));
            }

            shown++;
        }

        if (shown == 0)
            Debug.LogWarning("[StallUI] Er zijn 0 slots getoond. Heeft deze stall currentStock gevuld?");
    }

    void TryBuy(int index)
    {
        if (stall == null) return;
        if (index < 0 || index >= stall.currentStock.Count) return;

        var item = stall.currentStock[index];
        if (playerCoins >= item.price)
        {
            playerCoins -= item.price;
            UpdateCoins();
            Debug.Log($"[StallUI] Gekocht: {item.name} voor {item.price} coins");

            if (index < slots.Count && slots[index].buyButton != null)
                StartCoroutine(FlashButton(slots[index].buyButton));
        }
        else
        {
            Debug.Log("[StallUI] Niet genoeg coins!");
        }
    }

    void UpdateCoins()
    {
        if (coinsText) coinsText.text = $"Coins: {playerCoins}";
    }

    IEnumerator FlashButton(Button b)
    {
        var c = b.colors; var old = c.normalColor;
        c.normalColor = Color.green; b.colors = c;
        yield return new WaitForSeconds(0.15f);
        c.normalColor = old; b.colors = c;
    }
}
