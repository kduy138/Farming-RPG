public interface IInteractable
{
    void Interact(Player player);
    void OnFocus();
    void OnLostFocus();
    bool IsOnCooldown { get; }
}
