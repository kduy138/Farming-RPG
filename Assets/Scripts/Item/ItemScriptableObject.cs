using System;
using UnityEngine;

public enum ColorGrade
{
    grey, green, blue, yellow, red, purple
}

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "Scriptable Objects/Item")]
public class ItemScriptableObject : ScriptableObject
{
    [Header("Item Prefab")]
    [SerializeField]
    private GameObject itemPrefab;
    public GameObject ItemPrefab { get =>  itemPrefab; private set =>  itemPrefab = value; }
    [Header("AOC")]
    [SerializeField]
    private AnimatorOverrideController itemAnimatorOverrideController;
    public AnimatorOverrideController ItemAnimatorOverrideController { get =>  itemAnimatorOverrideController; private set => itemAnimatorOverrideController = value; }
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
    private bool equipable;
    public bool Equipable { get => equipable; private set => equipable = value; }
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
    public ItemAttributes attribute;
    public int Value;

    public ItemAttribute(int _value)
    {
        Value = _value;
    }
}

[System.Serializable]
public class ItemEffect
{
    public ItemEffects effect;
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
