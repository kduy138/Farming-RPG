using System;
using UnityEngine;

public class MiningManager : MonoBehaviour
{
    public event EventHandler OnMining;

    [Header("References")]
    private Player player;
    private GameObject interactBillboard;

    [Header("Flags")]
    private bool isPlayerInRange = false;
    private bool isMining = false;

    private void Awake()
    {
        player = FindAnyObjectByType<Player>();
        interactBillboard = transform.Find("InteractMenuCanvas").gameObject;
    }

    private void Update()
    {
        ToggleInteractBillboard();
        ChangeMiningState();
        CancelMiningAnimation();
    }

    private void ChangeMiningState()
    {
        if (!isPlayerInRange) {
            return;
        }

        if (GameInput.Instance.isMiningAction())
        {
            if (player.GetCurrentMoveSpeed() > 0)
            {
                Debug.Log("Không thể thực hiện hành động này hiện tại!");
                return;
            }
            isMining = true;
            Vector3 direction = transform.position - player.transform.position ;
            player.transform.rotation = Quaternion.LookRotation(direction);
            OnMining?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }

    private void ToggleInteractBillboard()
    {
        if (interactBillboard == null)
        {
            Debug.Log("Không tìm thấy Interact Billboard trên Scene hiện tại!");
            return;
        }

        if (isPlayerInRange)
        {
            interactBillboard.SetActive(true);
        }
        else
        {
            interactBillboard.SetActive(false);
        }
    }

    private void CancelMiningAnimation()
    {
        if (!isPlayerInRange)
        {
            var playerAnimator = player.GetComponent<PlayerAnimator>();
            playerAnimator.CancelMiningAnimation();
        }
    }
}
