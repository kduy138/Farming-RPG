using UnityEngine;

public class MiningManager : MonoBehaviour, IInteractable
{
    protected float cooldownTime = 10f;
    protected bool isOnCooldown;
    private IItemDrop itemDrop;

    public bool IsOnCooldown => isOnCooldown;

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
    }

    public void Interact(Player player)
    {
        if (player.IsInAction()) 
        {
            Debug.Log("Đang trong một hành động khác!");
            return; 
        }

        if (player.GetCurrentMoveSpeed() > 0)
        {
            Debug.Log("Không thể thực hiện hành động này hiện tại!");
            return;
        }

        MiningAction action = new MiningAction(
            this.transform, 
            player.runtimePlayerData.currentMiningTime,
            itemDrop
        );
        player.GetComponent<PlayerActionController>().StartAction(action);
    }

    public void OnFocus()
    {
        if (interactBillboard != null)
        {
            interactBillboard.SetActive(true);
        }
    }

    public void OnLostFocus()
    {
        if (interactBillboard != null)
        {
            interactBillboard.SetActive(false);
        }
    }
}
