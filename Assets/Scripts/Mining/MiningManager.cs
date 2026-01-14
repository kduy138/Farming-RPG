using System;
using Unity.VisualScripting;
using UnityEngine;

public class MiningManager : MonoBehaviour, IInteractable
{
    public event EventHandler OnMining;

    [Header("References")]
    private GameObject interactBillboard;

    [Header("Flags")]
    public bool isMining = false;

    private void Awake()
    {
        interactBillboard = transform.Find("InteractMenuCanvas")?.gameObject;

        if (interactBillboard != null)
        {
            interactBillboard.SetActive(false);
        }
    }

    public void Interact(Player player)
    {
        if (isMining) return;

        if (player.GetCurrentMoveSpeed() > 0)
        {
            Debug.Log("Không thể thực hiện hành động này hiện tại!");
            return;
        }

        player.MovementLock();
        isMining = true;
        Vector3 direction = transform.position - player.transform.position;
        direction.y = 0;
        player.transform.rotation = Quaternion.LookRotation(direction);
        OnMining?.Invoke(this, EventArgs.Empty);
    }

    public void OnFocus()
    {
        if (interactBillboard != null)
        {
            interactBillboard.SetActive(true);
        }
    }

    public void OnLostFocus()
    {
        if (interactBillboard != null)
        {
            interactBillboard.SetActive(false);
        }
        CancelMining();
    }

    private void CancelMining()
    {
        if (!isMining) return;

        isMining = false;

        var playerAnimator = FindAnyObjectByType<PlayerAnimator>();
        if (playerAnimator != null)
        {
            playerAnimator.CancelMiningAnimation();
        }
    }
}
