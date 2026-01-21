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

    [Header("Coroutines")]
    private Coroutine miningCoroutine;

    private void Awake()
    {
        interactBillboard = transform.Find("InteractMenuCanvas")?.gameObject;

        if (interactBillboard != null)
        {
            interactBillboard.SetActive(false);
        }
    }

    private void Update()
    {
     
    }

    public void Interact(Player player)
    {
        if (isMining) return;

        if (player.IsInAction()) 
        {
            Debug.Log("Đang trong một hành động khác!");
            return; 
        }

        if (player.GetCurrentMoveSpeed() > 0)
        {
            Debug.Log("Không thể thực hiện hành động này hiện tại!");
            return;
        }

        GameUI.Instance.miningScreen.SetActive(true);
        player.MovementLock();
        thisPlayer = player;
        isMining = true;
        player.SetIsInAction(true);
        Vector3 direction = transform.position - player.transform.position;
        direction.y = 0;
        player.transform.rotation = Quaternion.LookRotation(direction);
        OnMining?.Invoke(this, EventArgs.Empty);
        miningCoroutine = StartCoroutine(MiningCoroutine());
    }

    private IEnumerator Mining()
    {
        yield return new WaitForSeconds(thisPlayer.runtimePlayerData.currentMiningTime);
    }

    private IEnumerator MiningCoroutine()
    {
        float miningTime = thisPlayer.runtimePlayerData.currentMiningTime;
        float currentMiningTime = miningTime;

        while (currentMiningTime > 0) {
            currentMiningTime -= Time.deltaTime;
            currentMiningTime = Mathf.Max(currentMiningTime, 0f);
            GameUI.Instance.miniGameTimeBar.fillAmount = currentMiningTime / miningTime;
            GameUI.Instance.miningTimeTxt.text = $"{Mathf.CeilToInt(currentMiningTime)} giây";
            yield return null;
        }
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

    private void GetOutMiningState()
    {
        if (!isMining) return;

        GameUI.Instance.miningScreen.SetActive(false);
        if (thisPlayer != null)
        {
            thisPlayer.SetIsInAction(false);
            thisPlayer.MovementUnlock();
            var playerAnimator = thisPlayer.GetComponent<PlayerAnimator>();
            if (playerAnimator != null)
            {
                playerAnimator.CancelMiningAnimation();
            }
        }
        thisPlayer = null;
        isMining = false;
    }
}
