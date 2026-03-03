using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class UserInterface : MonoBehaviour
{
    [Header("Inventories")]
    public List<InventoryScriptableObject> inventories = new List<InventoryScriptableObject>();
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
    [SerializeField]
    private Sprite lockedSlotSprite;

    public virtual void Start()
    {
        removeItemBtn = GameUI.Instance.removeItemBtn;

        if (inventories == null || inventories.Count == 0) {
            Debug.LogError(name + " chưa gán inventory!");
            return;
        }

        CreateSlots();

        foreach(var inv  in inventories)
        {
            RegisterInventory(inv);
        }

        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });
        AddEvent(removeItemBtn, EventTriggerType.PointerEnter, delegate { OnEnterRemove(removeItemBtn); });
        AddEvent(removeItemBtn, EventTriggerType.PointerExit, delegate { OnExitRemove(removeItemBtn); });
    }

    protected void RegisterInventory(InventoryScriptableObject inv)
    {
        if (inv == null) return;

        for (int i = 0; i < inv.GetSlots.Length; i++)
        {
            var slot = inv.GetSlots[i];
            slot.parent = this;
            slot.inventory = inv;
            slot.OnAfterUpdate -= OnSlotUpdate;
            slot.OnAfterUpdate += OnSlotUpdate;
            OnSlotUpdate(slot);
        }
    }

    public abstract void CreateSlots();

    private void OnSlotUpdate(InventorySlot slot)
    {
        if (slot.slotDisplay == null)
        {
            Debug.Log("slotDisplay NULL!");
            return;
        }

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

        if (!slot.isAvailable)
        {
            icon.sprite = lockedSlotSprite;
            icon.color = new Color(1, 1, 1, 0.5f);
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
            var sourceSlot = slotsOnInterface[obj];
            var targetSlot = DraggingData.ui.slotsOnInterface[DraggingData.slotHoverOver];

            var sourceInv = sourceSlot.inventory;
            var targetInv = targetSlot.inventory;

            sourceInv.SwapItemSlot(sourceSlot, targetSlot);

            if (targetInv != sourceInv)
            {
                //sourceInv.CurrentWeight -= sourceSlot.itemSO.Weight * sourceSlot.quantity;
                if (sourceInv.CurrentWeight < 0)
                {
                    sourceInv.CurrentWeight = 0;
                }
                targetInv.Save();
            }
            sourceInv.Save();
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

    public void OnRMBClick_SwapItem(GameObject obj, PointerEventData data)
    {
        if (data.button == PointerEventData.InputButton.Right)
        {
            if (obj == null) return;

            if (slotsOnInterface[obj].item.ID < 0) return;

            if (isDiscard) return;
            var sourceSlot = slotsOnInterface[obj];
            var sourceInv = sourceSlot.inventory;

            var equipmentInv = GameUI.Instance.combatEquipmentSlotsContainer.activeInHierarchy ? inventories[1] : inventories[2];

            foreach (var slot in equipmentInv.GetSlots)
            {
                if (slot.slotType != sourceSlot.itemSO.Type && slot.slotType != ItemType.Universal) continue;

                var targetSlot = slot;
                var targetInv = targetSlot.inventory;

                targetSlot = slot;
                targetInv = targetSlot.inventory;
                sourceInv.CurrentWeight -= sourceSlot.itemSO.Weight * sourceSlot.quantity;
                if (sourceInv.CurrentWeight < 0)
                {
                    sourceInv.CurrentWeight = 0;
                }
                sourceInv.SwapItemSlot(sourceSlot, targetSlot);
                sourceInv.Save();
                targetInv.Save();
                ItemToolTip.Instance.HideItemToolTip();
                return;
            }
        }
    }

    public void ConfirmRemove(GameObject obj)
    {
        if (slotsOnInterface.ContainsKey(obj))
        {
            var slot = slotsOnInterface[obj];
            var inv = slot.inventory;
            inv.CurrentWeight -= slot.itemSO.Weight;
            inv.RemoveItem(slot.item);
            inv.Save();
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
                return new Color(0.6f, 0.6f, 0.6f);
            case ColorGrade.green:
                return new Color(0.1f, 0.9f, 0.1f);
            case ColorGrade.blue:
                return new Color(0.1f, 0.5f, 1f);
            case ColorGrade.yellow:
                return new Color(1f, 0.85f, 0.2f);
            case ColorGrade.red:
                return new Color(1f, 0.2f, 0.2f);
            case ColorGrade.purple:
                return new Color(0.7f, 0.3f, 1f);
            default:
                return Color.white;
        }
    }
}
