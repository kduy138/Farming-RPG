using TMPro;
using UnityEngine;

public class MiningManager : MonoBehaviour, IInteractable
{
    private IItemDrop itemDrop;

    [Header("References")]
    [SerializeField]
    private InventoryScriptableObject inventory;
    private ResourceRespawnManager rrm;

    [Header("References")]
    private GameObject interactBillboard;

    private void Awake()
    {
        interactBillboard = transform.Find("InteractMenuCanvas")?.gameObject;
        itemDrop = GetComponent<IItemDrop>();

        if (interactBillboard != null)
        {
            interactBillboard.SetActive(false);
        }

        if (rrm == null)
        {
            rrm = GetComponent<ResourceRespawnManager>();
        }
    }

    public void Interact(Player player)
    {
        if (rrm.IsDepleted()) return;

        if (player.IsInAction()) 
        {
            FloatingMessageManager.Instance.ShowMessage("Bạn đang thực hiện 1 hành động khác!", FloatingMessageType.Warning);
            return; 
        }

        if (player.GetCurrentMoveSpeed() > 0)
        {
            FloatingMessageManager.Instance.ShowMessage("Không thể thực hiện hành động này hiện tại!", FloatingMessageType.Warning);
            return;
        }

        MiningAction action = new MiningAction(
            this.transform, 
            player.runtimePlayerData.currentMiningTime,
            itemDrop,
            inventory,
            rrm,
            interactBillboard
        );
        player.GetComponent<PlayerActionController>().StartAction(action);
    }

    public void OnFocus()
    {
        if (rrm.IsDepleted()) return;

        if (interactBillboard != null)
        {
            interactBillboard.SetActive(true);
            interactBillboard.GetComponentInChildren<TextMeshProUGUI>().text = "E (Khai thác)";
        }
    }

    public void OnLostFocus()
    {
        if (interactBillboard != null)
        {
            interactBillboard.SetActive(false);
            interactBillboard.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
    }
}
