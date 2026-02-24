using System.Collections;
using UnityEngine;

public class FishingManager : MonoBehaviour
{
    [Header("References")]
    private Player player;
    private FishBoolManager fishBoolManager;
    private FishingMiniGame miniGame;
    public InventoryScriptableObject inventory;

    [Header("Flags")]
    public bool isFishing = false;
    public bool isCast = false;
    public bool isWaitingToCatch = false;
    public bool isPlayingMinigame = false;

    [Header("Game Objects")]
    [SerializeField]
    private GameObject fishingRod;
    [SerializeField]
    private GameObject fishingBait;
    [System.NonSerialized]
    public GameObject spawnedBait;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        miniGame = FindAnyObjectByType<FishingMiniGame>();
        fishingRod.SetActive(false);
    }

    private void Update()
    {
        ChangeFishingState();
        TriggerCast();
        CancelWaitingToCatch();
    }

    private void ChangeFishingState()
    {
        if (GameInput.Instance.isToggleFishing())
        {
            var playerData = player.runtimePlayerData;
            if (player.GetCurrentMoveSpeed() > 0)
            {
                Debug.Log("Không thể thực hiện hành động này hiện tại!");
                return;
            }

            if (!isFishing)
            {
                isFishing = true;
                player.SetIsInAction(true);
                player.SetCurrentFishingManager(this);
                player.events.TriggerOnFishingStarted();
                player.SetPlayerMoveSpeed(playerData.currentHoldingItemWalkSpeed);
                fishingRod.SetActive(true);
            }
            else
            {
                isFishing = false;
                player.SetIsInAction(false);
                player.events.TriggerOnFishingEnded();
                player.SetCurrentFishingManager(null);
                player.MovementUnlock();
                if (isWaitingToCatch)
                {
                    player.events.TriggerOnCastEnded();
                }
                isCast = false;
                fishingRod.SetActive(false);
                isWaitingToCatch = false;
                if (player.IsWalking())
                {
                    player.SetPlayerMoveSpeed(playerData.currentWalkSpeed);
                }
                else
                {
                    player.SetPlayerMoveSpeed(playerData.currentRunSpeed);
                }
               
                if (spawnedBait != null)
                {
                    Destroy(spawnedBait.gameObject);
                }
            }
        }
    }

    private void TriggerCast()
    {
        if(GameInput.Instance.isTriggerCast())
        {
            if (!isFishing) return;

            if (isWaitingToCatch || isCast || isPlayingMinigame) return;

            isCast = true;
            isWaitingToCatch = true;
            player.MovementLock();
            player.events.TriggerOnCastStarted();
            miniGame.HandleReleaseFish();
            StartCoroutine(WaitToSpawnFishingBait());
        }
    }

    private IEnumerator WaitToSpawnFishingBait()
    {
        float waitingTime = 2f;
        yield return new WaitForSeconds(waitingTime);
        SpawnFishingBait();
    }

    private void SpawnFishingBait()
    {
        Vector3 playerPos = player.transform.position;
        float spawnDistance = 10f;
        float heightOffset = 1f;
        Vector3 spawnPos = playerPos + player.transform.forward * spawnDistance;
        spawnPos.y -= heightOffset;

        RaycastHit hit;
        bool hitWater = Physics.Raycast(
            spawnPos + Vector3.up * 5f,
            Vector3.down,
            out hit,
            10f,
            LayerMask.GetMask("Water")
        );

        if (!hitWater)
        {
            FloatingMessageManager.Instance.ShowMessage("Không thể câu cá trên mặt đất!", FloatingMessageType.Warning);
            CancelCastOnTerrain();
            player.events.TriggerOnCastEnded();
            return;
        }

        if (hitWater)
        {
            fishBoolManager = hit.collider.GetComponent<FishBoolManager>();
            if (fishBoolManager == null)
            {
                FloatingMessageManager.Instance.ShowMessage("Không tìm thấy cá tại đây!", FloatingMessageType.Info);
            }
        }

        player.SetCurrentFishingManager(this);
        spawnedBait = Instantiate(fishingBait, hit.point, Quaternion.identity);
        FindAnyObjectByType<FishingLineRenderer>().SetBait(spawnedBait.transform);
        StartCoroutine(WaitToCatch());
    }

    private IEnumerator WaitToCatch()
    {
        yield return new WaitForSeconds(player.runtimePlayerData.currentFishingTime);
        isWaitingToCatch = false;
        EnterFishingMinigame();
    }

    private void EnterFishingMinigame() {
        if (!isFishing) return;
        if(fishBoolManager == null) return;
        if (miniGame == null) return;

        isPlayingMinigame = true;

        miniGame.BeginMinigame(fishBoolManager);
    }

    public void GiveFishingXPToPlayer()
    {
        player.GainXP(player.runtimePlayerData.currentFishingXPGain);
    }

    public void CancelCastOnTerrain()
    {
        isCast = false;
        isWaitingToCatch = false;
        player.MovementUnlock();

        if (spawnedBait != null)
        {
            Destroy(spawnedBait.gameObject);
        }

        StopAllCoroutines();
    }

    public void CancelWaitingToCatch()
    {
        if (spawnedBait == null) return;

        if (!isFishing) return;

        if(GameInput.Instance.isMovement())
        {
            if (!isWaitingToCatch) return;

            player.MovementUnlock();
            isWaitingToCatch = false;
            isCast = false;
            StopAllCoroutines();
            Destroy(spawnedBait.gameObject);
            player.events.TriggerOnCastEnded();
        }
    }

    public void AddFishToInventory(Item item, int quantity)
    {
        inventory.AddItem(item, quantity);
    }

    public void ResetCast()
    {
        isCast = false;
    }

    public void ResetIsPlayingMinigame()
    {
        isPlayingMinigame = false;
    }

    public void ResetWaiting()
    {
        isWaitingToCatch = false;
    }

    public Player GetPlayer()
    {
        return player;
    }

    public bool IsFishing()
    {
        return isFishing;
    }

    public bool IsCast() {
        return isCast;
    }

    public bool IsWaitingToCatch()
    {
        return isWaitingToCatch;
    }

    public bool IsPlayingMinigame()
    {
        return isPlayingMinigame;
    }
}
