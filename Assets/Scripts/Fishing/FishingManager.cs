using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class FishingManager : MonoBehaviour
{
    [Header("References")]
    private Player player;
    private FishBoolManager fishBoolManager;
    private FishingMiniGame miniGame;
    [SerializeField]
    private InventoryScriptableObject inventory;

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
                Debug.Log("Không thể thực hiện hành động này hiện tại!!");
                return;
            }

            if (isFishing == false)
            {
                isFishing = true;
                player.SetPlayerMoveSpeed(playerData.currentHoldingItemWalkSpeed);
                fishingRod.SetActive(true);
            }
            else
            {
                isFishing = false;
                isCast = false;
                if(player.IsWalking())
                {
                    player.SetPlayerMoveSpeed(playerData.currentWalkSpeed);
                }
                else
                {
                    player.SetPlayerMoveSpeed(playerData.currentRunSpeed);
                }
                fishingRod.SetActive(false);
                isWaitingToCatch = false;
                Destroy(spawnedBait.gameObject);
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
        
        if(hitWater)
        {
            fishBoolManager = hit.collider.GetComponent<FishBoolManager>();
            if (fishBoolManager == null)
            {
                Debug.Log("Không tìm thấy cá!!");
            }
        }

        spawnedBait = Instantiate(fishingBait, hit.point, Quaternion.identity);
        FindAnyObjectByType<FishingLineRenderer>().SetBait(spawnedBait.transform);
        StartCoroutine(WaitToCatch());

        if (!hitWater)
        {
            Debug.Log("Cannot fish on terrain!!");
            CancelCastOnTerrain();
            CancelCastAnimation();
            return;
        }
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
        player.GainXP(player.runtimePlayerData.currentFishingXP);
    }

    public void CancelCastOnTerrain()
    {
        isCast = false;
        isWaitingToCatch = false;

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

            isWaitingToCatch = false;
            isCast = false;
            StopAllCoroutines();
            Destroy(spawnedBait.gameObject);
            CancelCastAnimation();
        }
    }

    public void CancelCastAnimation()
    {
        var playerAnimator = player.GetComponent<PlayerAnimator>();
        playerAnimator.CancelCastAnimation();
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
