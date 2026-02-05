using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryScriptableObject", menuName = "Scriptable Objects/Inventory")]
public class InventoryScriptableObject : ScriptableObject
{
    [SerializeField]
    private string savePath;
    public ItemDatabaseObject itemDatabase;
    public Inventory container;
    public InventorySlot[] GetSlots { get { return container.slots; } }

    [Header("Weight")]
    private float currentWeight;
    public float CurrentWeight { get => currentWeight; set => currentWeight = value; }
    [SerializeField]
    private float weightLimit;
    public float WeightLimit { get => weightLimit; private set => weightLimit = value; }

    [Header("Slot")]
    private int currentSlotCount;
    public int CurrentSlotCount { get => currentSlotCount; private set => currentSlotCount = value; }
    [SerializeField]
    private int currentAvailableSlotCount;
    public int CurrentAvailableSlotCount { get => currentAvailableSlotCount; private set => currentAvailableSlotCount = value; }
    [SerializeField]
    private int maxSlot;
    public int MaxSlot { get => maxSlot; private set => maxSlot = value; }

    private void OnEnable()
    {
        if (container == null || container.slots == null || container.slots.Length != MaxSlot)
        {
            container = new Inventory(MaxSlot);
        }

        SyncAvailableSlots();

        foreach(var slot in container.slots)
        {
            if (slot.item == null)
            {
                slot.UpdateSlot(new Item(), 0);
            }
        }
    }

    private void SyncAvailableSlots()
    {
        for (int i = 0; i < container.slots.Length; i++)
        {
            container.slots[i].isAvailable = false;
            if (i < CurrentAvailableSlotCount)
            {
                container.slots[i].isAvailable = true;
            }
        }
    }

    public AddItemReturnCode CheckAddItem(Item _item, int _quantity)
    {
        InventorySlot slotWithThisItem = FindItemOnInventory(_item);

        if (GetEmptySlotCount <= 0 && (!itemDatabase.itemSO[_item.ID].Stackable || slotWithThisItem == null))
        {
            return AddItemReturnCode.NoEmptySlot;
        }

        if (currentWeight >= weightLimit)
        {
            return AddItemReturnCode.TooHeavy;
        }

        if (!itemDatabase.itemSO[_item.ID].Stackable || slotWithThisItem == null)
        {
            return AddItemReturnCode.Allow;
        }

        return AddItemReturnCode.Allow;
    }

    public void AddItem(Item _item, int _quantity)
    {
        InventorySlot slotWithThisItem = FindItemOnInventory(_item);

        if (!itemDatabase.itemSO[_item.ID].Stackable || slotWithThisItem == null)
        {
            SetItemToEmptySlot(_item, _quantity);
            currentWeight += itemDatabase.itemSO[_item.ID].Weight;
            FloatingMessageManager.Instance.ShowMessage("Đã thêm Item: " + _item.ItemName + " - " + _quantity, FloatingMessageType.Info);
            return;
        }

        slotWithThisItem.AddQuantity(_quantity);
        currentWeight += itemDatabase.itemSO[_item.ID].Weight;
        FloatingMessageManager.Instance.ShowMessage("Đã thêm Item: " + _item.ItemName + " với số lượng x" + _quantity, FloatingMessageType.Info);
    }

    public InventorySlot FindItemOnInventory(Item _item)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            if (GetSlots[i].item.ID == _item.ID)
            {
                return GetSlots[i];
            }
        }
        return null;
    }

    public int GetEmptySlotCount
    {
        get
        {
            int counter = 0;
            for (int i = 0; i < GetSlots.Length; i++)
            {
                if (GetSlots[i].item.ID <= -1 && GetSlots[i].isAvailable)
                {
                    counter++;
                }
            }
            return counter;
        }
    }

    public int GetHasItemSlotCount
    {
        get
        {
            int count = 0;
            foreach (var slot in GetSlots)
            {
                if (slot.isAvailable && slot.item.ID >= 0)
                {
                    count++;
                }
            }
            return count;
        }
    }

    public InventorySlot GetEmptySlot()
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            if (GetSlots[i].item.ID < 0)
            {
                return GetSlots[i];
            }
        }
        return null;
    }

    public InventorySlot SetItemToEmptySlot(Item _item, int _quantity)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            if (GetSlots[i].item.ID <= -1)
            {
                GetSlots[i].UpdateSlot(_item, _quantity);
                return GetSlots[i];
            }
        }
        return null;
    }

    public void SwapItemSlot(InventorySlot _itemSlot1, InventorySlot _itemSlot2)
    {
        if (!_itemSlot1.isAvailable || !_itemSlot2.isAvailable) return;

        if (_itemSlot2.CanStoreInSlot(_itemSlot1.itemSO) && _itemSlot1.CanStoreInSlot(_itemSlot2.itemSO))
        {
            InventorySlot temp = new InventorySlot(_itemSlot2.item, _itemSlot2.quantity);
            _itemSlot2.UpdateSlot(_itemSlot1.item, _itemSlot1.quantity);
            _itemSlot1.UpdateSlot(temp.item, temp.quantity);
        }
        else
        {
            FloatingMessageManager.Instance.ShowMessage("Không thể di chuyển vật phẩm!", FloatingMessageType.Warning);
        }
    }

    public void RemoveItem(Item _item)
    {
        for (int i = 0; i < GetSlots.Length; i++)
        {
            if (GetSlots[i].item == _item)
            {
                GetSlots[i].RemoveItem();
            }
        }
    }

    [ContextMenu("Save")]
    public void Save()
    {
        string fullSavePath = string.Concat(Application.persistentDataPath, savePath);
        InventorySaveData saveData = new InventorySaveData();
        saveData.itemIDs = new int[GetSlots.Length];
        saveData.quantities = new int[GetSlots.Length];

        for (int i = 0; i < GetSlots.Length; i++) {
            if (GetSlots[i] == null || GetSlots[i].item == null)
            {
                saveData.itemIDs[i] = -1;
                saveData.quantities[i] = 0;
                continue;
            }
            saveData.itemIDs[i] = GetSlots[i].item.ID;
            saveData.quantities[i] = GetSlots[i].quantity;
        }
        saveData.currentWeight = currentWeight;

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(fullSavePath, json);
        Debug.Log("Đã lưu dữ liệu kho đồ tại: " + fullSavePath);
    }

    [ContextMenu("Load")]
    public void Load()
    {
        string fullSavePath = string.Concat(Application.persistentDataPath, savePath);

        if (!File.Exists(fullSavePath)) return;

        string json = File.ReadAllText(fullSavePath);
        InventorySaveData saveData = JsonUtility.FromJson<InventorySaveData>(json);

        for(int i = 0; i < GetSlots.Length; i++)
        {
            GetSlots[i].UpdateSlot(saveData.itemIDs[i] >= 0 ? itemDatabase.itemSO[saveData.itemIDs[i]].CreateItem() : new Item(), saveData.quantities[i]);
        }
        currentWeight = saveData.currentWeight;

        Debug.Log("Đã tải dữ liệu kho đồ được lưu tại: " + fullSavePath);
    }

    [ContextMenu("Clear")]
    public void ClearInventory()
    {
        container.ClearInventory();
    }
}

[System.Serializable]
public class Inventory
{
    public InventorySlot[] slots;

    public Inventory(int slotCount)
    {
        slots = new InventorySlot[slotCount];
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = new InventorySlot();
        }
    }

    public void ClearInventory()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].RemoveItem();
        }
    }
}

public delegate void SlotUpdated(InventorySlot _slot);

[System.Serializable]
public class InventorySlot
{
    public InventoryScriptableObject inventory;
    public ItemType slotType;
    [System.NonSerialized]
    public UserInterface parent;
    [System.NonSerialized]
    public GameObject slotDisplay;
    public Item item;
    public int quantity;
    public bool isSelected = false;
    public bool isAvailable = false;

    [System.NonSerialized]
    public SlotUpdated OnBeforeUpdate;
    [System.NonSerialized]
    public SlotUpdated OnAfterUpdate;

    public ItemScriptableObject itemSO
    {
        get
        {
            if (item.ID >= 0)
            {
                return inventory.itemDatabase.itemSO[item.ID];
            }
            return null;
        }
    }

    public InventorySlot()
    {
        UpdateSlot(new Item(), 0);
    }

    public InventorySlot(Item _item, int _quantity)
    {
        UpdateSlot(_item, _quantity);
    }

    public void UpdateSlot(Item _item, int _quantity)
    {
        if (_item == null)
        {
            _item = new Item();
        }

        if (OnBeforeUpdate != null)
        {
            OnBeforeUpdate.Invoke(this);
        }
        item = _item;
        quantity = _quantity;
        if (OnAfterUpdate != null)
        {
            OnAfterUpdate.Invoke(this);
        }
    }

    public void RestoreItem(ItemDatabaseObject database, int ID)
    {
        if (database != null && ID >= 0)
        {
            item = database.itemSO[ID].CreateItem();
        }
        else if (database == null)
        {
            Debug.LogWarning("Database không tồn tại!!!");
        }
    }

    public void RemoveItem()
    {
        UpdateSlot(new Item(), 0);
    }

    public void AddQuantity(int _quantity)
    {
        UpdateSlot(item, quantity += _quantity);
    }

    public bool CanStoreInSlot(ItemScriptableObject _itemSO)
    {
        if (_itemSO == null || _itemSO.data.ID < 0) return true;
        if (slotType == ItemType.None) return true;

        return _itemSO.Type == slotType;
    }
}

[System.Serializable]
public class InventorySaveData
{
    public int[] itemIDs;
    public int[] quantities;
    public float currentWeight;
}

public enum AddItemReturnCode
{
    None,
    TooHeavy,
    NoEmptySlot,
    Allow,
}
