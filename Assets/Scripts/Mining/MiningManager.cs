using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MiningManager : MonoBehaviour, IInteractable
{
    public event EventHandler OnMining;

    [Header("References")]
    private GameObject interactBillboard;
    private Player thisPlayer;

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
        thisPlayer = player;
        isMining = true;
        Vector3 direction = transform.position - player.transform.position;
        direction.y = 0;
        player.transform.rotation = Quaternion.LookRotation(direction);
        OnMining?.Invoke(this, EventArgs.Empty);
    }

    private IEnumerator Mining()
    {
        yield return new WaitForSeconds(thisPlayer.runtimePlayerData.currentMiningTime);
    }

    private void MiningDuration()
    {

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
    }

    private void CancelMining()
    {
        if (!isMining) return;

        isMining = false;
        thisPlayer.MovementUnlock();
        thisPlayer = null;

        var playerAnimator = FindAnyObjectByType<PlayerAnimator>();
        if (playerAnimator != null)
        {
            playerAnimator.CancelMiningAnimation();
        }
    }
}
