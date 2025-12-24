using System.IO;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody playerRigidbody;
    private Vector3 moveDirection;
    private Vector3 lastMoveDirection;
    [SerializeField]
    private float currentMoveSpeed = 0f;
    private float moveSpeed;
    private bool isWalking = false;

    [SerializeField]
    private PlayerScriptableObject baseData;
    public PlayerStats runtimePlayerData { get; private set; }

    [SerializeField]
    private string savePath;

    [SerializeField]
    private float acceleration;
    [SerializeField]
    private float deceleration;
    [SerializeField]
    private float rotationSpeed;
    [SerializeField]
    private Transform cinemachineCamera;

    [SerializeField]
    private FishingManager fm;


    private void Awake()
    {
        runtimePlayerData = new PlayerStats();
        runtimePlayerData.InitFrom(baseData);

        playerRigidbody = GetComponent<Rigidbody>();
        playerRigidbody.freezeRotation = true;

        moveSpeed = runtimePlayerData.currentRunSpeed;
        currentMoveSpeed = 0f;
    }

    private void Update()
    {
        ToggleWalking();
        HandleSilverCoin();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void ToggleWalking()
    {
        if (isWalking == false && GameInput.Instance.isWalkAction())
        {
            isWalking = true;
            moveSpeed = runtimePlayerData.currentWalkSpeed;
        }
        else if (isWalking == true && GameInput.Instance.isWalkAction())
        {
            isWalking = false;
            moveSpeed = runtimePlayerData.currentRunSpeed;
        }
    }

    public bool IsWalking()
    {
        return isWalking;
    }

    public void SetPlayerMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    private void HandleMovement()
    {
        if (fm.IsWaitingToCatch() == true)
        {
            return;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (GameInput.Instance.isForwardAction()) vertical += 1f;
        if (GameInput.Instance.isBackwardAction()) vertical -= 1f;
        if (GameInput.Instance.isRightAction()) horizontal += 1f;
        if (GameInput.Instance.isLeftAction()) horizontal -= 1f;

        Vector3 camForward = cinemachineCamera.forward;
        Vector3 camRight = cinemachineCamera.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        moveDirection = (camForward * vertical + camRight * horizontal).normalized;
        bool hasInput = moveDirection != Vector3.zero;

        if (hasInput)
        {
            lastMoveDirection = moveDirection;
            currentMoveSpeed = Mathf.MoveTowards(currentMoveSpeed, moveSpeed, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            moveDirection = lastMoveDirection;
            currentMoveSpeed = Mathf.MoveTowards(currentMoveSpeed, 0f, deceleration * Time.fixedDeltaTime);
        }

        Vector3 targetVelocity = moveDirection * currentMoveSpeed;
        targetVelocity.y = playerRigidbody.linearVelocity.y;
        playerRigidbody.linearVelocity = targetVelocity;

        if (hasInput)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    private void HandleSilverCoin()
    {
        GameUI.Instance.silverCoinText.text = runtimePlayerData.currentSilverCoin.ToString("n0");
    }

    public float GetNormalizedSpeed()
    {
        float normalized = currentMoveSpeed / runtimePlayerData.currentRunSpeed;
        return Mathf.Clamp01(normalized);
    }

    public float GetBlendSpeed()
    {
        bool isMoving = currentMoveSpeed > 0.1f;
        float blend = isMoving ? 1f : 0f;
        return blend;
    }

    public float GetCurrentMoveSpeed()
    {
        return currentMoveSpeed;
    }

    [ContextMenu("Save")]
    public void Save()
    {
        string fullSavePath = Application.persistentDataPath + savePath;
        PlayerSaveData saveData = new PlayerSaveData();

        saveData.MaxHealth = runtimePlayerData.currentMaxHealth;
        saveData.Health = runtimePlayerData.currentHealth;
        saveData.Stamina = runtimePlayerData.currentStamina;
        saveData.Mana = runtimePlayerData.currentMana;

        saveData.ATK = runtimePlayerData.currentATK;
        saveData.DEF = runtimePlayerData.currentDEF;
        saveData.Evasion = runtimePlayerData.currentEvasion;
        saveData.DamageReduction = runtimePlayerData.currentDamageReduction;

        saveData.WalkSpeed = runtimePlayerData.currentWalkSpeed;
        saveData.RunSpeed = runtimePlayerData.currentRunSpeed;
        saveData.HoldingItemWalkSpeed = runtimePlayerData.currentHoldingItemWalkSpeed;

        saveData.Level = runtimePlayerData.currentLevel;
        saveData.XP = runtimePlayerData.currentXP;

        saveData.ItemDropRate = runtimePlayerData.currentItemDropRate;

        saveData.SilverCoin = runtimePlayerData.currentSilverCoin;

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(fullSavePath, json);

        Debug.Log("Đã lưu dữ liệu nhân vật tại: " + fullSavePath);
    }

    [ContextMenu("Load")]
    public void Load()
    {
        string fullSavePath = string.Concat(Application.persistentDataPath, savePath);

        if (!File.Exists(fullSavePath)) return;

        string json = File.ReadAllText(fullSavePath);
        PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(json);

        runtimePlayerData.currentMaxHealth = saveData.MaxHealth;
        runtimePlayerData.currentHealth = saveData.Health;
        runtimePlayerData.currentMana = saveData.Mana;
        runtimePlayerData.currentStamina = saveData.Stamina;

        runtimePlayerData.currentATK = saveData.ATK;
        runtimePlayerData.currentDEF = saveData.DEF;
        runtimePlayerData.currentEvasion = saveData.Evasion;
        runtimePlayerData.currentDamageReduction = saveData.DamageReduction;

        runtimePlayerData.currentWalkSpeed = saveData.WalkSpeed;
        runtimePlayerData.currentRunSpeed = saveData.RunSpeed;
        runtimePlayerData.currentHoldingItemWalkSpeed = saveData.HoldingItemWalkSpeed;

        runtimePlayerData.currentLevel = saveData.Level;
        runtimePlayerData.currentXP = saveData.XP;

        runtimePlayerData.currentItemDropRate = saveData.ItemDropRate;

        runtimePlayerData.currentSilverCoin = saveData.SilverCoin;

        Debug.Log("Đã tải dữ liệu nhân vật được lưu tại: " + fullSavePath);
    }
}

[System.Serializable]
public class PlayerSaveData
{
    public float MaxHealth;
    public float Health;
    public float Stamina;
    public float Mana;

    public int ATK;
    public int DEF;
    public int Evasion;
    public int DamageReduction;

    public float WalkSpeed;
    public float RunSpeed;
    public float HoldingItemWalkSpeed;

    public int Level;
    public float XP;

    public float ItemDropRate;

    public double SilverCoin;
}