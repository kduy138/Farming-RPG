using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StaticInterface : UserInterface
{
    public InventoryScriptableObject combatEquipmentInventory;
    public InventoryScriptableObject lifeSkillEquipmentInventory;

    public GameObject[] combatEquipmentSlots;
    public GameObject[] lifeSkillEquipmentSlots;

    private GameObject combatEquipmentContainer;
    private GameObject lifeSkillEquipmentContainer;

    public override void Start()
    {
        base.Start();
        InitializeUI();
        lifeSkillEquipmentContainer.SetActive(false);
    }

    public override void CreateSlots()
    {
        slotsOnInterface = new Dictionary<GameObject, InventorySlot>();

        for (int i = 0; i < combatEquipmentSlots.Length; i++)
        {
            var obj = combatEquipmentSlots[i];
            SetupSlotEvents(obj);

            var slot = combatEquipmentInventory.GetSlots[i];
            //slot.parent = this;
            slot.slotDisplay = obj;
            slotsOnInterface.Add(obj, slot);
        }

        for (int i = 0; i < lifeSkillEquipmentSlots.Length; i++)
        {
            var obj = lifeSkillEquipmentSlots[i];
            SetupSlotEvents(obj);
            var slot = lifeSkillEquipmentInventory.GetSlots[i];
            //slot.parent = this;
            slot.slotDisplay = obj;
            slotsOnInterface.Add(obj, slot);
        }
    }

    private void SetupSlotEvents(GameObject obj)
    {
        AddEvent(obj, UnityEngine.EventSystems.EventTriggerType.PointerEnter, delegate { OnPointerEnter(obj); });
        AddEvent(obj, EventTriggerType.PointerExit, delegate { OnPointerExit(obj); });
        AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
        AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
        AddEvent(obj, EventTriggerType.Drag, (data) => { OnDrag(obj, (PointerEventData)data); });
        AddEvent(obj, EventTriggerType.PointerClick, (data) => { OnRMBClick_SwapItem(obj, (PointerEventData)data); });
    }

    private void InitializeUI()
    {
        if (GameUI.Instance == null)
        {
            Debug.LogError("Không tìm thấy GameUI.Instance (Null)");
            return;
        }
        var GUIInstance = GameUI.Instance;
        combatEquipmentContainer = GUIInstance.combatEquipmentSlotsContainer;
        lifeSkillEquipmentContainer = GUIInstance.lifeSkillEquipmentSlotsContainer;
    }

    public void SwitchToCombatSlotsContainer()
    {
        lifeSkillEquipmentContainer.SetActive(false);
        combatEquipmentContainer.SetActive(true);
    }

    public void SwitchToLifeSkillSlotsContainer()
    {
        combatEquipmentContainer.SetActive(false);
        lifeSkillEquipmentContainer.SetActive(true);
    }
}
