using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class StaticInterface : UserInterface
{
    public GameObject[] slots;

    public override void CreateSlots()
    {
        slotsOnInterface = new Dictionary<GameObject, InventorySlot>();

        for (int i = 0; i < inventory.GetSlots.Length; i++)
        {
            var obj = slots[i];
            AddEvent(obj, UnityEngine.EventSystems.EventTriggerType.PointerEnter, delegate { OnPointerEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnPointerExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, (data) => { OnDrag(obj, (PointerEventData)data); });
            AddEvent(obj, EventTriggerType.PointerClick, (data) => { OnRMBClick(obj, (PointerEventData)data); });

            inventory.GetSlots[i].slotDisplay = obj;
            slotsOnInterface.Add(obj, inventory.GetSlots[i]);
        }
    }
}
