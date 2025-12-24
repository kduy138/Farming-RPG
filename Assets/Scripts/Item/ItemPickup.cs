using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField]
    private InventoryScriptableObject inventory;
    [SerializeField]
    private InventoryScriptableObject equipment;

    private bool isPlayerInRange = false;

    private void Update()
    {
        if (isPlayerInRange && GameInput.Instance.isCollectAction())
        {
            Pickup();
            inventory.Save();
        }
    }

    private void Pickup()
    {
        var item = GetComponent<ItemController>();

        if (item != null)
        {
            Item _item = new Item(item.itemData);
            if (inventory.AddItem(_item, item.quantity))
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
