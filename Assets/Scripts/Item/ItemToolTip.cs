using System;
using System.Reflection;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class ItemToolTip : MonoBehaviour
{
    public static ItemToolTip Instance;

    public GameObject itemToolTipObj;

    [Header("Item Infos")]
    public TMP_Text itemName;
    public TMP_Text itemDes;
    public TMP_Text itemType;
    public TMP_Text itemPrice;
    public TMP_Text itemWeight;
    public TMP_Text repairText;
    public Image itemImg;
    public Transform attributeContainer;
    public Transform effectContainer;
    public GameObject effectPrefab;
    public GameObject attributePrefab;

    [Header("Item Grade Background Images")]
    public Sprite greyItem;
    public Sprite greenItem;
    public Sprite blueItem;
    public Sprite yellowItem;
    public Sprite redItem;
    public Sprite purpleItem;

    private void Awake()
    {
        Instance = this;
        itemToolTipObj.SetActive(false);
    }

    public void ShowItemToolTip(InventorySlot itemSlot)
    {
        itemToolTipObj.GetComponent<Outline>().effectColor = GetColorByGrade(itemSlot.itemSO.ColorGrade);
        itemToolTipObj.transform.Find("ItemInfoHolder").GetComponent<Image>().sprite = GetBackgroundByGrade(itemSlot.itemSO.ColorGrade);
        itemName.text = $"<color={GetItemNameColorByGrade(itemSlot.itemSO.ColorGrade)}>{itemSlot.itemSO.ItemName}</color>";
        itemDes.text = $"<color=#d1d1d1>Mô tả:</color>\n<color=#d1d1d1>{itemSlot.itemSO.Description}</color>";
        itemImg.sprite = itemSlot.itemSO.Icon;
        itemImg.color = new Color(1, 1, 1, 1);
        itemPrice.text = $"<color=#d1d1d1>Giá trị: </color><color=#FFCD00>{itemSlot.itemSO.Price.ToString("n0")}</color>";
        itemType.text = GetDisplayName(itemSlot.itemSO.Type);
        itemWeight.text = $"<color=#d1d1d1>Trọng lượng: </color><color=#FFCD00>{itemSlot.itemSO.Weight}kg</color>";
        if (!itemSlot.itemSO.Repairable && IsNoRepairOptionType(itemSlot.itemSO.Type))
        {
            repairText.text = "";
        }
        else if (itemSlot.itemSO.Repairable)
        {
            repairText.text = "Có thể sửa chữa";
        }
        else
        {
            repairText.text = "Không thể sửa chữa";
        }

        foreach (Transform child in attributeContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var attribute in itemSlot.item.Attributes)
        {
            GameObject attributeUI = Instantiate(attributePrefab, attributeContainer);
            TMP_Text attributeText = attributeUI.GetComponent<TextMeshProUGUI>();
            attributeText.text = $"{GetDisplayName(attribute.attribute)}: {attribute.Value}\n";
        }

        foreach (Transform child in effectContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var effect in itemSlot.item.Effects)
        {
            GameObject effectUI = Instantiate(effectPrefab, effectContainer);
            TMP_Text effectText = effectUI.GetComponent<TextMeshProUGUI>();
            string valueString;
            if (effect.isPercent)
            {
                valueString = Math.Abs(effect.Value) + "%";
            }
            else if (effect.isSecond)
            {
                valueString = Math.Abs(effect.Value) + "s";
            }
            else
            {
                valueString = Math.Abs(effect.Value).ToString();
            }
            string sign = effect.Value >= 0 ? "+" : "-";
            effectText.text = $"<color=#FFCD00>{GetDisplayName(effect.effect)}: {sign}{valueString}</color>\n";
        }
        itemToolTipObj.SetActive(true);
    }

    public void HideItemToolTip()
    {
        itemToolTipObj.SetActive(false);
    }

    public Sprite GetBackgroundByGrade(ColorGrade grade)
    {
        switch (grade)
        {
            case ColorGrade.grey:
                return greyItem;
            case ColorGrade.green:
                return greenItem;
            case ColorGrade.blue:
                return blueItem;
            case ColorGrade.yellow:
                return yellowItem;
            case ColorGrade.red:
                return redItem;
            case ColorGrade.purple:
                return purpleItem;
            default:
                return greyItem;
        }
    }

    public string GetDisplayName(Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString());
        DisplayNameAttribute attribute = Attribute.GetCustomAttribute(field, typeof(DisplayNameAttribute)) as DisplayNameAttribute;

        return attribute != null ? attribute.name : value.ToString();
    }

    public static Color GetColorByGrade(ColorGrade grade)
    {
        switch (grade)
        {
            case ColorGrade.grey:
                return new Color(0.85f, 0.85f, 0.85f); // xám
            case ColorGrade.green:
                return new Color(0.1f, 0.9f, 0.1f); // xanh lá
            case ColorGrade.blue:
                return new Color(0.1f, 0.5f, 1f);   // xanh dương
            case ColorGrade.yellow:
                return new Color(1f, 0.85f, 0.2f);  // vàng
            case ColorGrade.red:
                return new Color(1f, 0.2f, 0.2f);   // đỏ
            case ColorGrade.purple:
                return new Color(0.7f, 0.3f, 1f);   // tím
            default:
                return Color.white;
        }
    }

    public static string GetItemNameColorByGrade(ColorGrade grade)
    {
        switch (grade)
        {
            case ColorGrade.grey:
                return "#bfbfbf"; // xám
            case ColorGrade.green:
                return "#02c708"; // xanh lá
            case ColorGrade.blue:
                return "#3955f0ff";   // xanh dương
            case ColorGrade.yellow:
                return "#f0d000";  // vàng
            case ColorGrade.red:
                return "#e30202";   // đỏ
            case ColorGrade.purple:
                return "#8802d1";   // tím
            default:
                return "#ffffff";
        }
    }

    public static bool IsNoRepairOptionType(ItemType type)
    {
        if (type == ItemType.Special)
            return true;
        if (type == ItemType.Consumable)
            return true;
        if (type == ItemType.General)
            return true;
        if (type == ItemType.Material)
            return true;
        if (type == ItemType.Artifact)
            return true;
        if (type == ItemType.PowerStone)
            return true;
        if (type == ItemType.Trade)
            return true;
        return false;
    }
}
