using UnityEngine;
using UnityEngine.UI;

public class MiningAction : PlayerBaseAction
{
    private Transform sourceTransform;
    private IItemDrop itemDrop;

    public MiningAction(Transform sourceTransform, float miningTime, IItemDrop itemDrop)
    {
        this.sourceTransform = sourceTransform;
        this.duration = miningTime;
        this.itemDrop = itemDrop;
    }

    public override void ActionStart(Player player)
    {
        base.ActionStart(player);

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
    }

    public override void ActionStop(Player player)
    {
        base.ActionStop(player);
        GameUI.Instance.miningScreen.SetActive(false);
        player.MovementUnlock();
        player.SetIsInAction(false);
        player.events.TriggerOnMiningEnded();

        if (itemDrop != null)
        {
            var item = itemDrop.GetRandomItem();
            var itemAmount = itemDrop.GetRandomItemAmount();

            if (item != null)
            {
                GameUI.Instance.ToggleGetItemPopUp();
                GameUI.Instance.itemPopUpIcon.transform.Find("Icon").GetComponent<Image>().sprite = item.Icon;
                GameUI.Instance.itemPopUpIcon.transform.Find("Icon").GetComponent<Image>().color = new Color(1, 1, 1, 1);
                GameUI.Instance.getItemPopUpTxt.text = $"Đã đào được {item.ItemName} x{itemAmount.ToString()}";
            }
        }
    }
}
