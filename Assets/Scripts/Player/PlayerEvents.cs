using System;
using Unity.VisualScripting;

public class PlayerEvents
{
    public event EventHandler OnDead;
    public event EventHandler OnMiningStarted;
    public event EventHandler OnMiningEnded;
    public event EventHandler OnFishingStarted;
    public event EventHandler OnFishingEnded;
    public event EventHandler OnCastStarted;
    public event EventHandler OnCastEnded;
    public event EventHandler OnCombatStarted;
    public event EventHandler OnCombatEnded;
    public event EventHandler OnNormalAttack;

    public void TriggerOnDead() => OnDead?.Invoke(this, EventArgs.Empty);
    public void TriggerOnMiningStarted() => OnMiningStarted?.Invoke(this, EventArgs.Empty);
    public void TriggerOnMiningEnded() => OnMiningEnded?.Invoke(this, EventArgs.Empty);
    public void TriggerOnFishingStarted() => OnFishingStarted?.Invoke(this, EventArgs.Empty);
    public void TriggerOnFishingEnded() => OnFishingEnded?.Invoke(this, EventArgs.Empty);
    public void TriggerOnCastStarted() => OnCastStarted?.Invoke(this, EventArgs.Empty);
    public void TriggerOnCastEnded() => OnCastEnded?.Invoke(this, EventArgs.Empty);
    public void TriggerOnCombatStarted() => OnCombatStarted?.Invoke(this, EventArgs.Empty);
    public void TriggerOnCombatEnded() => OnCombatEnded?.Invoke(this, EventArgs.Empty);
    public void TriggerOnNormalAttack() => OnNormalAttack?.Invoke(this, EventArgs.Empty);
}
