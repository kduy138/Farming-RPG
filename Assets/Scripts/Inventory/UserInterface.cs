using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UserInterface : MonoBehaviour
{
    public InventoryScriptableObject inventory;
    public InventoryScriptableObject equipment;
    public Dictionary<GameObject, InventorySlot> slotsOnInterface = new Dictionary<GameObject, InventorySlot>();

    [System.NonSerialized]
    public StaticInterface staticInterface;
    [System.NonSerialized]
    public DynamicInterface dynamicInterface;

    protected GameObject removeItemBtn;

    protected bool isDiscard = false;

    [Header("Screens")]
    [SerializeField]
    private GameObject inventoryScreen;

    [SerializeField]
    private Sprite emptySlotSprite;

    public virtual void Start()
    {
        removeItemBtn = GameUI.Instance.removeItemBtn;
        CreateSlots();

        for (int i = 0; i < inventory.GetSlots.Length; i++)
        {
            inventory.GetSlots[i].parent = this;
            inventory.GetSlots[i].OnAfterUpdate += OnSlotUpdate;
        }

        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });

        AddEvent(removeItemBtn, EventTriggerType.PointerEnter, delegate { OnEnterRemove(removeItemBtn); });
        AddEvent(removeItemBtn, EventTriggerType.PointerExit, delegate { OnExitRemove(removeItemBtn); });
    }

    private void Update()
    {
        for (int i = 0; i < inventory.GetSlots.Length; i++)
        {
            inventory.GetSlots[i].parent = this;
        }

        for (int i = 0; i < inventory.GetSlots.Length; i++)
        {
            OnSlotUpdate(inventory.GetSlots[i]);
        }
    }

    // private void OnDestroy()
    // {
    //     for (int i = 0; i < inventory.GetSlots.Length; i++)
    //     {
    //         inventory.GetSlots[i].OnAfterUpdate -= OnSlotUpdate;
    //     }
    // }

    public abstract void CreateSlots();

    private void OnSlotUpdate(InventorySlot slot)
    {
        TextMeshProUGUI quantityText = slot.slotDisplay.transform.Find("Quantity")?.GetComponent<TextMeshProUGUI>();
        Outline outline = slot.slotDisplay.transform.Find("Outline")?.GetComponent<Outline>();
        Image icon = slot.slotDisplay.transform.Find("Icon")?.GetComponent<Image>();

        if (slot.item.ID >= 0)
        {
            icon.sprite = slot.itemSO.Icon;
            icon.color = new Color(1, 1, 1, 1);
            if (quantityText != null)
            {
                quantityText.text = slot.quantity == 1 ? "" : slot.quantity.ToString("n0");
            }
            if (outline != null)
            {
                outline.effectColor = ExtensionMethods.GetColorByGrade(slot.itemSO.ColorGrade);
            }
        }
        else
        {
            icon.sprite = emptySlotSprite;
            icon.color = new Color(1, 1, 1, 0);
            if(quantityText != null)
            {
                quantityText.text = "";
            }
            if (outline != null)
            {
                outline.effectColor = Color.black;
            }
        }
    }

    protected void AddEvent(GameObject obj, EventTriggerType eventType, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = eventType;
        eventTrigger.callback.AddListener(action);
        trigger.triggers.Add(eventTrigger);
    }

    public GameObject CreateDragObject(GameObject obj)
    {
        GameObject dragObj = null;
        if (slotsOnInterface[obj].item.ID >= 0)
        {
            dragObj = new GameObject();
            dragObj.name = "Drag Obj";
            var rectTransform = dragObj.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(50, 50);
            dragObj.transform.SetParent(inventoryScreen.transform);

            var img = dragObj.AddComponent<Image>();
            img.sprite = slotsOnInterface[obj].itemSO.Icon;
            img.raycastTarget = false;
        }
        return dragObj;
    }

    public void OnEnterRemove(GameObject obj)
    {
        DraggingData.slotHoverOverRemove = obj;
    }

    public void OnExitRemove(GameObject obj)
    {
        if (DraggingData.slotHoverOverRemove == obj)
        {
            DraggingData.slotHoverOverRemove = null;
        }
    }

    public void OnEnterInterface(GameObject obj)
    {
        DraggingData.ui = obj.GetComponent<UserInterface>();
        staticInterface = obj.GetComponent<StaticInterface>();
        dynamicInterface = obj.GetComponent<DynamicInterface>();
    }

    public void OnExitInterface(GameObject obj)
    {
        if (DraggingData.ui != null) DraggingData.ui = null;
        if (staticInterface != null) staticInterface = null;
        if (dynamicInterface != null) dynamicInterface = null;
    }

    public void OnPointerEnter(GameObject obj)
    {
        DraggingData.slotHoverOver = obj;
        StartCoroutine(ShowItemToolTipWithDelay(obj, 0.1f));
    }

    public void OnPointerExit(GameObject obj)
    {
        DraggingData.slotHoverOver = null;
        ItemToolTip.Instance.HideItemToolTip();
    }

    public void OnDragStart(GameObject obj)
    {
        DraggingData.draggingItem = CreateDragObject(obj);
    }

    public void OnDragEnd(GameObject obj)
    {
        Destroy(DraggingData.draggingItem);

        if (isDiscard) return;

        if (DraggingData.slotHoverOver)
        {
            InventorySlot draggingItemHoverSlotData = DraggingData.ui.slotsOnInterface[DraggingData.slotHoverOver];
            inventory.SwapItemSlot(slotsOnInterface[obj], draggingItemHoverSlotData);
            if (dynamicInterface != null)
            {
                DynamicInterface dynamicInv = FindAnyObjectByType<DynamicInterface>();

                if (dynamicInv == null)
                {
                    Debug.LogError("Không tìm thấy DynamicInterface trong scene!!!");
                    return;
                }

                InventoryScriptableObject inv = dynamicInv.inventory;

                inv.Save();
            }
            else
            {
                inventory.Save();
            }
            equipment.Save();
        }

        if (DraggingData.slotHoverOverRemove && slotsOnInterface[obj].item.ID >= 0)
        {
            GameUI.Instance.confirmRemoveScreen.SetActive(true);
            GameUI.Instance.confirmRemoveText.text = "Hủy <color=" + slotsOnInterface[obj].itemSO.ColorGrade + ">" + slotsOnInterface[obj].item.ItemName + "</color> x" + slotsOnInterface[obj].quantity + "?";
            GameUI.Instance.itemIcon.sprite = slotsOnInterface[obj].itemSO.Icon;
            GameUI.Instance.itemIcon.color = new Color(1, 1, 1, 1);

            GameUI.Instance.confirmRemoveBtn.onClick.RemoveAllListeners();
            GameUI.Instance.confirmRemoveBtn.onClick.AddListener(() => ConfirmRemove(obj));
        }
    }

    public void OnDrag(GameObject obj, PointerEventData data)
    {
        if (DraggingData.draggingItem != null)
        {
            DraggingData.draggingItem.GetComponent<RectTransform>().position = data.position;
        }
    }

    public void OnRMBClick(GameObject obj, PointerEventData data)
    {
        if (data.button == PointerEventData.InputButton.Right)
        {
            if (obj == null) return;

            if (slotsOnInterface[obj].item.ID < 0) return;

            if (isDiscard) return;

            if (dynamicInterface != null)
            {
                for (int i = 0; i < equipment.GetSlots.Length; i++)
                {
                    if (equipment.GetSlots[i].allowedItems.Contains(slotsOnInterface[obj].itemSO.Type))
                    {
                        inventory.SwapItemSlot(slotsOnInterface[obj], equipment.GetSlots[i]);
                        inventory.CurrentWeight -= slotsOnInterface[obj].itemSO.Weight;
                        inventory.Save();
                        equipment.Save();
                        ItemToolTip.Instance.HideItemToolTip();
                        return;
                    }
                }
            }
            else if (staticInterface != null)
            {
                DynamicInterface dynamicInv = FindAnyObjectByType<DynamicInterface>();

                if (dynamicInv == null)
                {
                    Debug.LogError("Không tìm thấy DynamicInterface trong scene!!!");
                    return;
                }

                InventoryScriptableObject inv = dynamicInv.inventory;

                InventorySlot emptySlot = inv.GetEmptySlot();

                if (emptySlot == null)
                {
                    Debug.Log("Kho đồ đã đầy, không thể bỏ vật phẩm vào!!!");
                    return;
                }

                inv.SwapItemSlot(slotsOnInterface[obj], emptySlot);
                inv.Save();
                equipment.Save();
                ItemToolTip.Instance.HideItemToolTip();
            }
        }
    }

    public void ConfirmRemove(GameObject obj)
    {
        if (slotsOnInterface.ContainsKey(obj))
        {
            inventory.RemoveItem(slotsOnInterface[obj].item);
            inventory.CurrentWeight -= slotsOnInterface[obj].itemSO.Weight;
            inventory.Save();
            GameUI.Instance.confirmRemoveScreen.SetActive(false);
        }
    }

    private IEnumerator ShowItemToolTipWithDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (DraggingData.slotHoverOver == obj)
        {
            if (slotsOnInterface[obj].item.ID >= 0)
            {
                RectTransform rect = ItemToolTip.Instance.itemToolTipObj.GetComponent<RectTransform>();
                if (staticInterface != null)
                {
                    rect.anchoredPosition = new Vector2(-166, 92.5f);
                }
                else
                {
                    rect.anchoredPosition = new Vector2(292, 92.5f);
                }
                ItemToolTip.Instance.ShowItemToolTip(slotsOnInterface[obj]);
            }
        }
    }
}

public static class DraggingData
{
    public static UserInterface ui;
    public static GameObject draggingItem;
    public static GameObject slotHoverOver;
    public static GameObject slotHoverOverRemove;
}

public static class ExtensionMethods
{
    public static Color GetColorByGrade(ColorGrade grade)
    {
        switch (grade)
        {
            case ColorGrade.grey:
                return new Color(0.6f, 0.6f, 0.6f); // xám
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
}
