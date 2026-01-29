public interface IAction
{
    void ActionStart(Player player);
    void ActionTick(Player player);
    void ActionStop(Player player);
    void ActionSuccess(Player player);
    bool IsActionFinished { get; }
}
