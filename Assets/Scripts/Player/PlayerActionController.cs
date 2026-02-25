using UnityEngine;

public class PlayerActionController : MonoBehaviour
{
    private PlayerBaseAction currentAction;

    public void StartAction(PlayerBaseAction action)
    {
        Player player = GetComponent<Player>();
        if (currentAction != null)
        {
            currentAction.ActionStop(player);
        }
        currentAction = action;
        currentAction.ActionStart(player);
    }

    private void Update()
    {
        Player player = GetComponent<Player>();
        if (currentAction == null) return;

        currentAction.ActionTick(player);

        if (currentAction.IsActionFinished)
        {
            currentAction.ActionStop(player);
            if (currentAction.itemTaken)
            {
                currentAction = null;
            }
        }
    }
}
