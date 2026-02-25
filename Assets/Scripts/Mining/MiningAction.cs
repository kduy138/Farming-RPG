using UnityEngine;
using UnityEngine.UI;

public class MiningAction : PlayerBaseAction
{
    private Transform sourceTransform;
    private IItemDrop itemDrop;

    private Item pendingItem;
    private int pendingItemAmount;

    [Header("References")]
    private InventoryScriptableObject inventory;
    private ResourceRespawnManager rrm;
    private GameObject interactBillboard;

    [Header("Flags")]
    private bool itemDropped;
    private bool playerReset;

    public MiningAction(
            Transform sourceTransform, 
            float miningTime, 
            IItemDrop itemDrop, 
            InventoryScriptableObject inventory, 
            ResourceRespawnManager rrm,
            GameObject interactBillboard
        )
    {
        this.sourceTransform = sourceTransform;
        this.duration = miningTime;
        this.itemDrop = itemDrop;
        this.inventory = inventory;
        this.rrm = rrm;
        this.interactBillboard = interactBillboard;
    }

    public override void ActionStart(Player player)
    {
        base.ActionStart(player);

        interactBillboard.SetActive(false);

        itemDropped = false;
        playerReset = false;

        player.MovementLock();
        player.SetIsInAction(true);

        Vector3 direction = sourceTransform.position - player.transform.position;
        direction.y = 0;
        player.transform.rotation = Quaternion.LookRotation(direction);

        GameUI.Instance.miningScreen.SetActive(true);
        player.events.TriggerOnMiningStarted();
    }

    public override void ActionTick(Player player)
    {
        base.ActionTick(player);
        currentTime = Mathf.Max(currentTime, 0f);
        GameUI.Instance.miningTimebar.fillAmount = currentTime / duration;
        GameUI.Instance.miningTimeTxt.text = $"{Mathf.CeilToInt(currentTime)} giây";
        if (GameInput.Instance.isTakeItemAction())
        {
            ActionSuccess(player);
        }
        GameUI.Instance.getItemBtn.onClick.RemoveAllListeners();
        GameUI.Instance.getItemBtn.onClick.AddListener(() => ActionSuccess(player));
    }

    public override void ActionStop(Player player)
    {
        base.ActionStop(player);

        rrm.DepleteResource();
        interactBillboard.SetActive(false);

        if (!playerReset)
        {
            GameUI.Instance.miningScreen.SetActive(false);
            player.MovementUnlock();
            player.SetIsInAction(false);
            player.events.TriggerOnMiningEnded();
        }
        playerReset = true;

        if (itemDrop != null && !itemDropped)
        {
            var item = itemDrop.GetRandomItem();
            var itemAmount = itemDrop.GetRandomItemAmount();
            pendingItem = new Item(item);
            pendingItemAmount = itemAmount;

            if (item != null)
            {
                player.GainXP(player.runtimePlayerData.currentMiningXPGain);
                GameUI.Instance.ToggleGetItemPopUp();
                GameUI.Instance.itemPopUpIcon.transform.Find("Icon").GetComponent<Image>().sprite = item.Icon;
                GameUI.Instance.itemPopUpIcon.transform.Find("Icon").GetComponent<Image>().color = new Color(1, 1, 1, 1);
                GameUI.Instance.getItemPopUpTxt.text = $"Đã đào được {item.ItemName} x{itemAmount.ToString()}";
            }
        }
        itemDropped = true;
    }

    public override void ActionSuccess(Player player)
    {
        base.ActionSuccess(player);
        if (pendingItem == null || pendingItemAmount == 0) return;
       
        AddItemReturnCode addItemPermission = inventory.CheckAddItem(pendingItem, pendingItemAmount);

        switch (addItemPermission)
        {
            case AddItemReturnCode.NoEmptySlot:
                GameUI.Instance.getItemWarningTxt.text = "Kho đồ đã đầy!";
                break;
            case AddItemReturnCode.TooHeavy:
                GameUI.Instance.getItemWarningTxt.text = "Kho đồ quá nặng!";
                break;
            case AddItemReturnCode.Allow:
                inventory.AddItem(pendingItem, pendingItemAmount);
                inventory.Save();
                pendingItem = null;
                pendingItemAmount = 0;
                GameUI.Instance.getItemPopUp.SetActive(false);
                itemTaken = true;
                break;
        }
    }
}
