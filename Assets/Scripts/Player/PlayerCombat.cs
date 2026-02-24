using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [Header("Flags")]
    private bool isInCombat = false;

    [Header("References")]
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (GameInput.Instance.isCombatAction())
        {
            if (!isInCombat)
            {
                EnterCombatMode();
            }
            else
            {
                ExitCombatMode();
            }
        }
    }

    private void EnterCombatMode()
    {
        isInCombat = true;
        player.events.TriggerOnCombatStarted();
        player.SetIsInAction(true);
    }

    private void ExitCombatMode()
    {
        isInCombat = false;
        player.events.TriggerOnCombatEnded();
        player.SetIsInAction(false);
    }

    public bool IsInCombat()
    {
        return isInCombat;
    }
}
