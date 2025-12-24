using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryScriptableObject", menuName = "Scriptable Objects/Inventory")]
public class InventoryScriptableObject : ScriptableObject
{
    [SerializeField]
    private string savePath;
    public ItemDatabaseObject itemDatabase;
    public Inventory container = new Inventory();
    public InventorySlot[] GetSlots { get { return container.slots; } }

    [Header("Weight")]
    private float currentWeight;
    public float CurrentWeight { get => currentWeight; set => currentWeight = value; }
    [SerializeField]
    private float weightLimit;
    public float WeightLimit { get => weightLimit; private set => weightLimit = value; }

    public bool AddItem(Item _item, int _quantity)
    {
        InventorySlot slot = FindItemOnInventory(_item);

        if (GetEmptySlotCount <= 0 && (!itemDatabase.itemSO[_item.ID].Stackable || slot == null))
        {
            Debug.Log("Không thể thêm " + _item.ItemName + " - " + "Kho đồ đã đầy!!!");
            return false;
        }

        if (currentWeight >= weightLimit)
        {
            Debug.Log("Không thể thêm " + _item.ItemName + " - " + "Kho đồ quá nặng!!!");
            return false;
        }

        if (!itemDatabase.itemSO[_item.ID].Stackable || slot == null)
        {
            SetItemToEmptySlot(_item, _quantity);
            currentWeight += itemDatabase.itemSO[_item.ID].Weight;
            Debug.Log("Đã thêm Item: " + _item.ItemName + " - " + _quantity);
            return true;
        }

        slot.AddQuantity(_quantity);
        currentWeight += itemDatabase.itemSO[_item.ID].Weight;
        Debug.Log("Đã thêm Item: " + _item.ItemName + " với số lượng x" + _quantity);
        return true;
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
                if (GetSlots[i].item.ID <= -1)
                {
                    counter++;
                }
            }
            return counter;
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
        if (_itemSlot2.CanStoreInSlot(_itemSlot1.itemSO) && _itemSlot1.CanStoreInSlot(_itemSlot2.itemSO))
        {
            InventorySlot temp = new InventorySlot(_itemSlot2.item, _itemSlot2.quantity);
            _itemSlot2.UpdateSlot(_itemSlot1.item, _itemSlot1.quantity);
            _itemSlot1.UpdateSlot(temp.item, temp.quantity);
        }
        else
        {
            Debug.Log("Không thể di chuyển vật phẩm!!!");
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
    public InventorySlot[] slots = new InventorySlot[32];

    public Inventory()
    {
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
    public ItemType[] allowedItems = new ItemType[0];
    [System.NonSerialized]
    public UserInterface parent;
    [System.NonSerialized]
    public GameObject slotDisplay;
    public Item item;
    public int quantity;
    public bool isSelected = false;

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
                return parent.inventory.itemDatabase.itemSO[item.ID];
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
        if (allowedItems.Length <= 0 || _itemSO == null || _itemSO.data.ID < 0) return true;

        for (int i = 0; i < allowedItems.Length; i++)
        {
            if (_itemSO.Type == allowedItems[i])
            {
                return true;
            }
        }

        return false;
    }
}

[System.Serializable]
public class InventorySaveData
{
    public int[] itemIDs;
    public int[] quantities;
    public float currentWeight;
}
