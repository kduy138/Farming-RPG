using System;
using UnityEngine;

public enum ColorGrade
{
    grey, green, blue, yellow, red, purple
}

public enum ItemType
{
    [DisplayName("Vũ khí chính")] MainWeapon, [DisplayName("Vũ khí phụ")] SubWeapon,
    [DisplayName("Áo giáp")] Armor, [DisplayName("Khiên")] Shield,
    [DisplayName("Mũ")] Helmet, [DisplayName("Khuyên tai")] Earrings,
    [DisplayName("Cổ vật")] Artifact, [DisplayName("Giày")] Shoes,
    [DisplayName("Đai lưng")] Belt, [DisplayName("Vòng cổ")] Necklace,
    [DisplayName("Găng tay")] Gloves, [DisplayName("Nhẫn")] Ring,
    [DisplayName("Ngọc bội")] PowerStone, [DisplayName("Dụng cụ")] Tool,
    [DisplayName("Có thể sử dụng")] Consumable, [DisplayName("Nguyên liệu")] Material,
    [DisplayName("Thông dụng")] General, [DisplayName("Đặc biệt")] Special,
    [DisplayName("Trao đổi")] Trade,
}

public enum Attributes
{
    [DisplayName("Tấn công")] ATK, [DisplayName("Phòng thủ")] DEF,
    [DisplayName("Né đòn")] Evasion, [DisplayName("Giảm sát thương")] DamageReduction,
}

public enum Effects
{
    [DisplayName("Tốc độ di chuyển")] MoveSpeed, [DisplayName("Thông dụng")] WeightLimit,
    [DisplayName("Sinh lực tối đa")] MaxHP, [DisplayName("Tỉ lệ rơi vật phẩm")] ItemDropRate,
    [DisplayName("Phục hồi sinh lực")] HPRestore, [DisplayName("Thời gian câu cá")] FishingTime,
    [DisplayName("Thời gian thu hoạch")] GatheringTime, [DisplayName("Thời gian nấu ăn")] CookingTime,
    [DisplayName("Thông thạo câu cá")] FishingMastery, [DisplayName("Thông thạo thu hoạch")] GatheringMastery, [DisplayName("Thông thạo nấu ăn")] CookingMastery, [DisplayName("Thông thạo giả kim")] Alchemy,
}

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "Scriptable Objects/Item")]
public class ItemScriptableObject : ScriptableObject
{
    [Header("Basic Infos")]
    [SerializeField]
    private string itemName;
    public string ItemName { get => itemName; private set => itemName = value; }
    [SerializeField]
    private string description;
    [TextArea] public string Description { get => description; private set => description = value; }
    [SerializeField]
    private double price;
    public double Price { get => price; private set => price = value; }
    [SerializeField]
    private Sprite icon;
    public Sprite Icon { get => icon; private set => icon = value; }

    [Header("Item Type")]
    [SerializeField]
    private ItemType type;
    public ItemType Type { get => type; private set => type = value; }
    [SerializeField]
    private ColorGrade colorGrade;
    public ColorGrade ColorGrade { get => colorGrade; private set => colorGrade = value; }
    [SerializeField]
    private bool stackable;
    public bool Stackable { get => stackable; private set => stackable = value; }
    [SerializeField]
    private bool repairable;
    public bool Repairable { get => repairable; private set => repairable = value; }
    [SerializeField]
    private int maxStack;
    public int MaxStack { get => maxStack; private set => maxStack = value; }

    [Header("Weight")]
    [SerializeField]
    private float weight;
    public float Weight { get => weight; set => weight = value; }

    public Item data = new Item();

    public Item CreateItem()
    {
        Item newItem = new Item(this);
        return newItem;
    }
}

[System.Serializable]
public class Item
{
    [Header("Basic Infos")]
    public int ID = -1;
    public String ItemName;
    public ItemAttribute[] Attributes;
    public ItemEffect[] Effects;

    public Item()
    {
        ID = -1;
        ItemName = "";
    }

    public Item(ItemScriptableObject itemSO)
    {
        ID = itemSO.data.ID;
        ItemName = itemSO.ItemName;
        Attributes = new ItemAttribute[itemSO.data.Attributes.Length];
        Effects = new ItemEffect[itemSO.data.Effects.Length];

        for (int i = 0; i < Attributes.Length; i++)
        {
            Attributes[i] = new ItemAttribute(itemSO.data.Attributes[i].Value)
            {
                attribute = itemSO.data.Attributes[i].attribute
            };
        }

        for (int i = 0; i < Effects.Length; i++)
        {
            Effects[i] = new ItemEffect(itemSO.data.Effects[i].Value, itemSO.data.Effects[i].isPercent, itemSO.data.Effects[i].isSecond)
            {
                effect = itemSO.data.Effects[i].effect,
            };
        }
    }
}

[System.Serializable]
public class ItemAttribute
{
    public Attributes attribute;
    public int Value;

    public ItemAttribute(int _value)
    {
        Value = _value;
    }
}

[System.Serializable]
public class ItemEffect
{
    public Effects effect;
    public float Value;
    public bool isPercent;
    public bool isSecond;

    public ItemEffect(float _value, bool _isPercent = false, bool _isSecond = false)
    {
        Value = _value;
        isPercent = _isPercent;
        isSecond = _isSecond;
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class DisplayNameAttribute : Attribute
{
    public string name;
    public DisplayNameAttribute(string _name)
    {
        name = _name;
    }
}
