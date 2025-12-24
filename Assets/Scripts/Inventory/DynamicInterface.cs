using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DynamicInterface : UserInterface
{
    public Transform itemContent;
    public GameObject inventoryItem;

    private GameObject discardOptionHolder;
    private GameObject weightAndSlotCountHolder;

    [SerializeField]
    private List<InventorySlot> discardSlots = new List<InventorySlot>();

    public override void Start()
    {
        base.Start();
        discardOptionHolder = GameUI.Instance.discardOptionHolder;
        weightAndSlotCountHolder = GameUI.Instance.weightAndSlotCountHolder;
    }

    public override void CreateSlots()
    {
        slotsOnInterface = new Dictionary<GameObject, InventorySlot>();

        for (int i = 0; i < inventory.GetSlots.Length; i++)
        {
            var obj = Instantiate(inventoryItem, itemContent);

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnPointerEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnPointerExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, (data) => { OnDrag(obj, (PointerEventData)data); });
            AddEvent(obj, EventTriggerType.PointerClick, (data) => { OnRMBClick(obj, (PointerEventData)data); });
            AddEvent(obj, EventTriggerType.PointerClick, (data) => { OnLMBDiscardClick(obj, (PointerEventData)data); });
            AddEvent(removeItemBtn, EventTriggerType.PointerClick, (data) => { OnRemoveRMBClick(removeItemBtn, (PointerEventData)data); });

            inventory.GetSlots[i].slotDisplay = obj;
            slotsOnInterface.Add(obj, inventory.GetSlots[i]);
        }
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
            Debug.Log("Không có vật phẩm cần xóa!");
            return;
        }

        foreach (InventorySlot slot in discardSlots)
        {
            ResetDiscardSlot();
            inventory.CurrentWeight -= slot.itemSO.Weight;
            inventory.RemoveItem(slot.item);
        }
        discardSlots.Clear();
        inventory.Save();
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

    private void OnApplicationQuit()
    {
        inventory.container.slots = new InventorySlot[32];
    }
}
