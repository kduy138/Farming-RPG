public interface IAction
{
    void ActionStart(Player player);
    void ActionTick(Player player);
    void ActionStop(Player player);
    bool IsActionFinished { get; }
}
