using System.Collections;
using UnityEngine;

public class FishingManager : MonoBehaviour
{
    public enum State { 
        Idle,
        Cast,
        WaitingToCatch,
        Minigame,
    }

    private State state;

    [Header("References")]
    private Player player;
    private FishBoolManager fishBoolManager;
    private FishingMiniGame miniGame;
    public InventoryScriptableObject inventory;

    [Header("Flags")]
    public bool isFishing = false;

    [Header("Game Objects")]
    [SerializeField]
    private Transform fishingRod;
    [SerializeField]
    private GameObject fishingBait;
    [System.NonSerialized]
    public GameObject spawnedBait;

    [Header("Settings")]
    private float baitSpawnTimer;
    private float baitSpawnTimerMax = 2f;
    private float fishingTimer;
    private float fishingTimerMax;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        miniGame = FindAnyObjectByType<FishingMiniGame>();
        fishingRod.gameObject.SetActive(false);
        state = State.Idle;
    }

    private void Update()
    {
        ToggleFishingMode();

        if (!isFishing) return;

        switch(state)
        {
            default:
            case State.Idle:
                if (GameInput.Instance.isTriggerCast())
                {
                    state = State.Cast;
                }
                break;
            case State.Cast:
                miniGame.HandleReleaseFish();
                player.MovementLock();
                player.events.TriggerOnCastStarted();

                state = State.WaitingToCatch;
                break;
            case State.WaitingToCatch:
                baitSpawnTimer += Time.deltaTime;
                if (baitSpawnTimer > baitSpawnTimerMax)
                {
                    baitSpawnTimer = 0f;
                    SpawnFishingBait();
                }

                fishingTimerMax = player.runtimePlayerData.currentFishingTime;
                fishingTimer += Time.deltaTime;
                
                if (fishingTimer > fishingTimerMax)
                {
                    fishingTimer = 0f;
                    state = State.Minigame;

                    if (fishBoolManager == null) return;
                    if (miniGame == null) return;

                    miniGame.BeginMinigame(fishBoolManager);
                }
                break;
            case State.Minigame:
                break;
        }
        CancelWaitingToCatch();
    }

    private void ToggleFishingMode()
    {
        if (GameInput.Instance.isToggleFishing())
        {
            var playerData = player.runtimePlayerData;
            if (player.GetCurrentMoveSpeed() > 0)
            {
                FloatingMessageManager.Instance.ShowMessage("Hiện tại không thể thực hiện hành động này!", FloatingMessageType.Warning);
                return;
            }

            if (!isFishing)
            {
                isFishing = true;
                player.SetIsInAction(true);
                player.SetCurrentFishingManager(this);
                player.events.TriggerOnFishingStarted();
                player.SetPlayerMoveSpeed(playerData.currentHoldingItemWalkSpeed);
                fishingRod.gameObject.SetActive(true);
            }
            else
            {
                isFishing = false;
                player.SetIsInAction(false);
                player.events.TriggerOnFishingEnded();
                player.SetCurrentFishingManager(null);
                player.MovementUnlock();
                if (state == State.WaitingToCatch)
                {
                    player.events.TriggerOnCastEnded();
                }
                state = State.Idle;
                fishingRod.gameObject.SetActive(false);
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

    private void SpawnFishingBait()
    {
        if (spawnedBait != null) return;

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
    }

    public void GiveFishingXPToPlayer()
    {
        player.GainXP(player.runtimePlayerData.currentFishingXPGain);
    }

    public void CancelCastOnTerrain()
    {
        state = State.Idle;
        player.MovementUnlock();

        if (spawnedBait != null)
        {
            Destroy(spawnedBait.gameObject);
        }
    }

    public void CancelWaitingToCatch()
    {
        if (spawnedBait == null) return;

        if(GameInput.Instance.isMovement())
        {
            if (state != State.WaitingToCatch) return;

            player.MovementUnlock();
            state = State.Idle;
            Destroy(spawnedBait.gameObject);
            player.events.TriggerOnCastEnded();
        }
    }

    public void AddFishToInventory(Item item, int quantity)
    {
        inventory.AddItem(item, quantity);
    }

    public Player GetPlayer()
    {
        return player;
    }

    public bool IsFishing()
    {
        return isFishing;
    }

    public State GetState()
    {
        return state;
    }

    public void SetState(State state)
    {
        this.state = state;
    }
}
