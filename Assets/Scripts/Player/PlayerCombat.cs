using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public int normalAtkCount = 1;

    [Header("Settings")]
    [SerializeField]
    private float normalAtkCooldown = 0.1f;
    private float comboResetTime = 1f;

    [Header("Flags")]
    private bool isInCombat = false;
    [SerializeField]
    private bool isAttacking = false;
    private bool canQueueNextAttack;
    private bool attackBuffered;

    [Header("References")]
    private Player player;
    private PlayerAnimator playerAnimator;
    private AnimatorOverrideController itemAOC;
    [SerializeField]
    private AnimatorOverrideController defaultAOC;
    [SerializeField]
    private GameObject weaponHolder;
    private GameObject currentWeaponInstance;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerAnimator = GetComponent<PlayerAnimator>();
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
            HandleAttackInput();
        }
    }

    private void EnterCombatMode()
    {
        isInCombat = true;

        var combatEquipmentInv = player.GetPlayerCombatEquipmentInventory();
        foreach(var slot in combatEquipmentInv.GetSlots)
        {
            if (slot.slotType == ItemType.MainWeapon)
            {
                if (slot.item.ID <= -1)
                {
                    FloatingMessageManager.Instance.ShowMessage("Bạn chưa trang bị vũ khí chính!", FloatingMessageType.Warning);
                    return;
                }

                if (currentWeaponInstance != null)
                {
                    Destroy(currentWeaponInstance.gameObject);
                }

                currentWeaponInstance = Instantiate(slot.itemSO.ItemPrefab, weaponHolder.transform);
                //currentWeaponInstance.transform.localPosition = Vector3.zero;
                //currentWeaponInstance.transform.localRotation = Quaternion.identity;

                itemAOC = slot.itemSO.ItemAnimatorOverrideController;
                playerAnimator.EquipWeapon(itemAOC);

                player.events.TriggerOnCombatStarted();
                player.SetIsInAction(true);
                return;
            }
        }
    }

    private void ExitCombatMode()
    {
        isInCombat = false;
        player.events.TriggerOnCombatEnded();
        player.SetIsInAction(false);
        playerAnimator.EquipWeapon(defaultAOC);
        currentWeaponInstance.gameObject.SetActive(false);
    }

    private void HandleAttackInput()
    {
        if (!isAttacking)
        {
            StartAttack();
            return;
        }

        if (canQueueNextAttack)
        {
            attackBuffered = true;
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        attackBuffered = false;
        canQueueNextAttack = false;
        player.events.TriggerOnNormalAttack();
    }

    public void OpenComboWindow()
    {
        Debug.Log("OPEN WINDOW!");
        canQueueNextAttack = true;
    }

    public void CloseComboWindow()
    {
        Debug.Log("CLOSE WINDOW!");
        canQueueNextAttack = false;
    }

    public void EndAttack()
    {
        isAttacking = false;

        if (attackBuffered)
        {
            attackBuffered = false;

            normalAtkCount++;
            if (normalAtkCount > 3)
            {
                normalAtkCount = 1;
            }
            StartAttack();
        }
        else
        {
            ResetNormalAtkCount();
        }
        Debug.Log("END ATTACK!");
    }

    private void DisableWeaponCollider()
    {

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
