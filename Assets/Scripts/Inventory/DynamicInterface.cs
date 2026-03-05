using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DynamicInterface : UserInterface
{
    private InventoryScriptableObject Inventory
    {
        get
        {
            if (inventories == null || inventories.Count == 0)
            {
                Debug.Log("DynamicInterface không có inventory!");
                return null;
            }
            return inventories[0];
        }
    }
    public Transform itemContent;
    public GameObject inventoryItem;

    private GameObject discardOptionHolder;
    private GameObject weightAndSlotCountHolder;

    [SerializeField]
    private List<InventorySlot> discardSlots = new List<InventorySlot>();

    private void Awake()
    {
        discardOptionHolder = GameUI.Instance.discardOptionHolder;
        weightAndSlotCountHolder = GameUI.Instance.weightAndSlotCountHolder;
    }

    public override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if (GameUI.Instance == null)
        {
            Debug.LogError("GameUI.Instance is NULL");
            return;
        }
        HandleWeightAndSlotBar();
    }

    public override void CreateSlots()
    {
        if (Inventory == null)
        {
            Debug.Log("Inventory Null!");
            return;
        }

        slotsOnInterface = new Dictionary<GameObject, InventorySlot>();

        for (int i = 0; i < Inventory.GetSlots.Length; i++)
        {
            var obj = Instantiate(inventoryItem, itemContent);
            SetupSlotEvents(obj);
            Inventory.GetSlots[i].slotDisplay = obj;
            slotsOnInterface.Add(obj, Inventory.GetSlots[i]);
        }
    }

    private void SetupSlotEvents(GameObject obj)
    {
        AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnPointerEnter(obj); });
        AddEvent(obj, EventTriggerType.PointerExit, delegate { OnPointerExit(obj); });
        AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
        AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
        AddEvent(obj, EventTriggerType.Drag, (data) => { OnDrag(obj, (PointerEventData)data); });
        AddEvent(obj, EventTriggerType.PointerClick, (data) => { OnRMBClick_SwapItem(obj, (PointerEventData)data); });
        AddEvent(obj, EventTriggerType.PointerClick, (data) => { OnLMBDiscardClick(obj, (PointerEventData)data); });
        AddEvent(removeItemBtn, EventTriggerType.PointerClick, (data) => { OnRemoveRMBClick(removeItemBtn, (PointerEventData)data); });
    }

    private void HandleWeightAndSlotBar()
    {
        if (Inventory == null) return;

        GameUI.Instance.weightText.text = $"<color=#FFCD00>Trọng lượng:</color> {Inventory.CurrentWeight}/{Inventory.WeightLimit}";
        GameUI.Instance.weightBar.fillAmount = Inventory.CurrentWeight / Inventory.WeightLimit;
        GameUI.Instance.slotText.text = $"<color=#FFCD00>Số ô đồ:</color> {Inventory.GetHasItemSlotCount}/{Inventory.CurrentAvailableSlotCount}";
        GameUI.Instance.slotBar.fillAmount = (float)Inventory.CurrentSlotCount / Inventory.MaxSlot;
    }

    private void OnLMBDiscardClick(GameObject obj, PointerEventData data)
    {
        InventorySlot slot = slotsOnInterface[obj];
        if (!isDiscard) return;
        if (slot.item.ID < 0) return;

        if (data.button == PointerEventData.InputButton.Left)
        {
            if (slot.isSelected)
            {
                discardSlots.Remove(slot);
                slot.isSelected = false;
            }
            else
            {
                discardSlots.Add(slot);
                slot.isSelected = true;
            }

            Image slotDiscardBackground = slot.slotDisplay.transform.Find("DiscardBackground")?.GetComponent<Image>();
            Image slotDiscardIcon = slot.slotDisplay.transform.Find("DiscardIcon")?.GetComponent<Image>();

            if ((slotDiscardBackground != null && slotDiscardIcon != null) && slot.isSelected)
            {
                slotDiscardBackground.gameObject.SetActive(true);
                slotDiscardIcon.gameObject.SetActive(true);
            }
            else
            {
                slotDiscardBackground.gameObject.SetActive(false);
                slotDiscardIcon.gameObject.SetActive(false);
            }
        }
    }

    private void ResetDiscardSlot()
    {
        foreach (InventorySlot slot in discardSlots)
        {
            Image slotDiscardImg = slot.slotDisplay.transform.Find("DiscardBackground")?.GetComponent<Image>();
            Image slotDiscardIcon = slot.slotDisplay.transform.Find("DiscardIcon")?.GetComponent<Image>();

            if (slot.isSelected)
            {
                slot.isSelected = false;
            }

            if (slotDiscardImg != null && slotDiscardIcon != null)
            {
                slotDiscardImg.gameObject.SetActive(false);
                slotDiscardIcon.gameObject.SetActive(false);
            }
        }
    }

    public void HandleItemsDiscard()
    {
        if (discardSlots.Count <= 0)
        {
            FloatingMessageManager.Instance.ShowMessage("Không có vật phẩm cần xóa", FloatingMessageType.Info);
            return;
        }

        foreach (InventorySlot slot in discardSlots)
        {
            ResetDiscardSlot();
            Inventory.CurrentWeight -= slot.itemSO.Weight;
            Inventory.RemoveItem(slot.item);
        }
        discardSlots.Clear();
        Inventory.Save();
    }

    public void CancelItemsDiscard()
    {
        if (!isDiscard) return;

        discardOptionHolder.SetActive(false);
        weightAndSlotCountHolder.SetActive(true);
        GameUI.Instance.removeItemBtn.GetComponent<Image>().sprite = GameUI.Instance.removeIcon;
        ResetDiscardSlot();
        discardSlots.Clear();
        isDiscard = false;
    }

    public void OnRemoveRMBClick(GameObject obj, PointerEventData data)
    {
        if (data.button == PointerEventData.InputButton.Right)
        {
            isDiscard = true;
            obj.GetComponent<Image>().sprite = GameUI.Instance.discardIcon;
            discardOptionHolder.SetActive(true);
            weightAndSlotCountHolder.SetActive(false);
        }
    }
}
