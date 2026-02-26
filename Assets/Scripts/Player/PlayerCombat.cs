using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public int normalAtkCount = 1;

    [Header("Settings")]
    [SerializeField]
    private float normalAtkCooldown = 0.1f;
    private float lastNormalAtkTime;

    [Header("Flags")]
    private bool isInCombat = false;
    [SerializeField]
    private bool isAttacking = false;

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

        if (!isInCombat) return;

        if (GameInput.Instance.isNormalAttackAction())
        {
            if (normalAtkCount > 3)
            {
                ResetNormalAtkCount();
            }
            NormalAttack("Sword_Normal_Attack_");
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

    private void NormalAttack(string animation)
    {
        if (Time.time - lastNormalAtkTime < normalAtkCooldown) return;
        if (isAttacking) return;

        lastNormalAtkTime = Time.time;
        isAttacking = true;
        player.events.TriggerOnNormalAttack(animation);
        normalAtkCount++;
    }

    public void EndAttack()
    {
        isAttacking = false;
    }

    private void ResetNormalAtkCount()
    {
        normalAtkCount = 1;
    }

    public bool IsInCombat()
    {
        return isInCombat;
    }

    public bool IsAttacking()
    {
        return isAttacking;
    }
}
