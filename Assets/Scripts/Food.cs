using UnityEngine;

[System.Serializable]
public class Food : MonoBehaviour
{
    //Public Variables
    public int ID; //Unique identifier for each food item.
    public int size;//The amount of space it takes up in the inventory. 
    new public string name; //Porkchops, broccoli, tuna etc. Not the food type but the name of the actual food. 
    public FoodType.Type foodType;
    public Quality foodQuality;
    public Sprite icon;          // runtime referentie (Wordt niet door JSON geserialiseerd)
    public string iconName;      // <-- serialiseerbare verwijzing (sprite naam zonder .png)
    public int price = 1;
    public enum Quality
    {
        Good,
        Medium,
        Low,
        Spoiled
    }

    // CONSTRUCTORS (Unity roept deze niet aan bij MonoBehaviours; laten staan op jouw verzoek)
    public Food(FoodType.Type _type, Quality _quality, int _size, string _name)
    {
        foodType = _type;
        foodQuality = _quality;
        size = _size;
        name = _name;
    }
    public Food(FoodType.Type _type, int _size, string _name, int _price, Sprite _icon)
    {
        foodType = _type;
        foodQuality = Quality.Good;
        size = _size;
        name = _name;
        price = _price;
        icon = _icon;
        iconName = _icon ? _icon.name : null; // <-- houd iconName in sync
    }

    // Kleine auto-resolve bij runtime: als icon ontbreekt maar iconName is bekend, laad sprite
    void Awake()
    {
        if (icon == null && !string.IsNullOrEmpty(iconName))
            ResolveIconFromResources();
    }

    /// <summary>
    /// Laad de sprite op basis van iconName.
    /// Zet je sprites onder: Assets/Resources/Art/Icons/MarketItemIcons/
    /// </summary>
    public void ResolveIconFromResources()
    {
        if (string.IsNullOrEmpty(iconName)) return;
        icon = Resources.Load<Sprite>($"Art/Icons/MarketItemIcons/{iconName}");
        #if UNITY_EDITOR
        if (icon == null)
            Debug.LogWarning($"[Food] Sprite not found for '{iconName}'. Check Resources path/name.");
        #endif
    }

    /// <summary>
    /// Handige helper: zet icon én iconName tegelijk (zodat JSON later kan herstellen).
    /// </summary>
    public void SetIcon(Sprite s)
    {
        icon = s;
        iconName = s ? s.name : null;
    }

    //Private Variables
    private float SpoilChance;
    private void Spoil(Food food)
    {
        //Spoil logic here
        food.foodQuality = Quality.Spoiled;
        food.price = 0;//Magic Number,  maar moet hier kunnen
    }
}