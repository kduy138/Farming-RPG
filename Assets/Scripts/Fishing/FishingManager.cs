using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class FishingManager : MonoBehaviour
{
    private Player player;
    public bool isFishing = false;
    public bool isCast = false;
    public bool isWaitingToCatch = false;
    public bool isPlayingMinigame = false;
    [SerializeField]
    private GameObject fishingRod;
    [SerializeField]
    private GameObject fishingBait;
    [System.NonSerialized]
    public GameObject spawnedBait;

    private FishBoolManager fishBoolManager;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
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
            Debug.Log("Fishing: " + isFishing);
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
            StartCoroutine(WaitToSpawnFishingBait());
            Debug.Log("Casted");
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
        int waitingTime = 8;
        yield return new WaitForSeconds(waitingTime);
        isWaitingToCatch = false;
        EnterFishingMinigame();
    }

    private void EnterFishingMinigame() {
        if (!isFishing) return;
        if(fishBoolManager == null) return;

        isPlayingMinigame = true;
        FindAnyObjectByType<FishingMiniGame>().BeginMinigame(fishBoolManager);
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
