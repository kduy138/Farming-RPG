using UnityEngine;

public abstract class PlayerBaseAction : IAction
{
    protected float duration;
    protected float currentTime;
    protected bool isFinished;
    public bool itemTaken = false;

    public bool IsActionFinished => isFinished;

    public virtual void ActionStart(Player player)
    {
        currentTime = duration;
        isFinished = false;
        itemTaken = false;
    }

    public virtual void ActionTick(Player player)
    {
        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            Finish(player);
        }
    }

    public virtual void ActionStop(Player player)
    {
        isFinished = true;
        if (!isFinished) return;
    }

    public virtual void ActionSuccess(Player player)
    {
        if (!isFinished) return;
    }

    protected virtual void Finish(Player player) {
        isFinished = true;
    }
}
