using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private Vector3 moveDirection;
    private Vector3 lastMoveDirection;
    private float currentMoveSpeed = 0f;
    private float moveSpeed;

    [Header("Flags")]
    private bool isWalking = false;
    [SerializeField]
    private bool isDead = false;
    private bool isMovementLocked = false;
    private bool isInAction = false;

    [Header("References")]
    public PlayerEvents events { get; private set; } 
    private Rigidbody playerRigidbody;
    [SerializeField]
    private PlayerScriptableObject baseData;
    public PlayerStats runtimePlayerData { get; private set; }
    private FishingManager currentFishingManager;
    private PlayerCombat playerCombat;

    [Header("Inventories")]
    [SerializeField]
    private InventoryScriptableObject combatEquipmentInventory;
    [SerializeField]
    private InventoryScriptableObject lifeSkillEquipmentInventory;

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

    [Header("UI")]
    private Image staminaBar;
    private TextMeshProUGUI staminaTxt;
    private Image hpBar;
    private TextMeshProUGUI hpTxt;
    private Image manaBar;
    private TextMeshProUGUI manaTxt;

    private void Awake()
    {
        events = new PlayerEvents();
        runtimePlayerData = new PlayerStats();
        runtimePlayerData.InitFrom(baseData);

        playerRigidbody = GetComponent<Rigidbody>();
        playerRigidbody.freezeRotation = true;

        playerCombat = GetComponent<PlayerCombat>();

        moveSpeed = runtimePlayerData.currentRunSpeed;
        currentMoveSpeed = 0f;
    }

    private void Start()
    {
        InitializePlayerUI();
    }

    private void Update()
    {
        if (isDead) return;

        ToggleWalking();
        DisplayPlayerUI();
        HandlePlayerFillBar();
        HandleStaminaRecoverOverTime();
        HandlePlayerDead();
    }

    private void FixedUpdate()
    {
        HandlePlayerMovement();
    }

    private void InitializePlayerUI()
    {
        if (GameUI.Instance == null)
        {
            Debug.LogError("GameUI.Instance is NULL");
            return;
        }
        var GUIInstance = GameUI.Instance;
        staminaBar = GUIInstance.staminaBar;
        staminaTxt = GUIInstance.staminaTxt;
        hpBar = GUIInstance.hpBar;
        hpTxt = GUIInstance.hpTxt;
        manaBar = GUIInstance.manaBar;
        manaTxt = GUIInstance.manaTxt;
    }

    private void DisplayPlayerUI()
    {
        DisplayPlayerSilverCoin();
        DisplayPlayerLevelAndXP();
    }

    private void HandlePlayerFillBar()
    {
        staminaBar.fillAmount = runtimePlayerData.currentStamina / runtimePlayerData.currentMaxStamina;
        staminaTxt.text = $"{Mathf.CeilToInt(runtimePlayerData.currentStamina)}/{runtimePlayerData.currentMaxStamina}";
        hpBar.fillAmount = runtimePlayerData.currentHealth / runtimePlayerData.currentMaxHealth;
        hpTxt.text = $"{runtimePlayerData.currentHealth}/{runtimePlayerData.currentMaxHealth}";
        manaBar.fillAmount = runtimePlayerData.currentMana / runtimePlayerData.currentMaxMana;
        manaTxt.text = $"{runtimePlayerData.currentMana}/{runtimePlayerData.currentMaxMana}";
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

    private void OnEquipmentChanged(InventorySlot slot)
    {

    }

    private void HandlePlayerMovement()
    {
        if (isMovementLocked || GetPlayerCombat().IsAttacking())
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

    public void MovementLock()
    {
        isMovementLocked = true;
        currentMoveSpeed = 0f;
    }

    public void MovementUnlock()
    {
        isMovementLocked = false;
    }

    private void DisplayPlayerSilverCoin()
    {
        GameUI.Instance.silverCoinText.text = runtimePlayerData.currentSilverCoin.ToString("n0");
    }

    private void DisplayPlayerLevelAndXP()
    {
        GameUI.Instance.levelTxt.text = runtimePlayerData.currentLevel.ToString();
        GameUI.Instance.xpTxt.text = $"{runtimePlayerData.currentXP}/{XPToReachNextLevelCalculator(runtimePlayerData.currentLevel + 1)}";
    }

    private void HandleStaminaRecoverOverTime()
    {
        if (runtimePlayerData.currentStamina >= runtimePlayerData.currentMaxStamina)
        {
            runtimePlayerData.currentStamina = runtimePlayerData.currentMaxStamina;
            return;
        }

        if (runtimePlayerData.currentStamina < runtimePlayerData.currentMaxStamina)
        {
            runtimePlayerData.currentStamina += runtimePlayerData.currentStaminaRecoverRate * Time.deltaTime;
        }
    }

    private void HandlePlayerDead()
    {
        if (runtimePlayerData.currentHealth <= 0f && !isDead)
        {
            isDead = true;
            MovementLock();
            runtimePlayerData.currentHealth = 0f;
            events.TriggerOnDead();
            GameUI.Instance.playerDeadScreen.SetActive(true);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (runtimePlayerData.currentHealth <= 0f) {
            runtimePlayerData.currentHealth = 0f;
            return;
        }
        runtimePlayerData.currentHealth -= damageAmount;
    }

    private int XPToReachNextLevelCalculator(int nextLevel)
    {
        if (nextLevel > runtimePlayerData.MaxLevel) { 
            return 0; 
        }

        int firstPass = 0;
        int xp = 0;

        for (int l = 1; l < nextLevel; l++)
        {
            firstPass += (int)Math.Floor(l + (300.0f * Math.Pow(2.0f, l / 7.0f)));
            xp = firstPass / 4;
        }
        if (xp > runtimePlayerData.MaxXP)
        {
            return (int)runtimePlayerData.MaxXP;
        }
        return xp;
    }

    public void GainXP(float amount)
    {
        if (runtimePlayerData.currentXP + amount < 0)
        {
            return;
        }
        if (runtimePlayerData.currentXP > runtimePlayerData.MaxXP) { 
            runtimePlayerData.currentXP = runtimePlayerData.MaxXP;
            return;
        }

        runtimePlayerData.currentXP += amount;
        if (runtimePlayerData.currentXP >= XPToReachNextLevelCalculator(runtimePlayerData.currentLevel + 1))
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        if (runtimePlayerData.currentLevel >= runtimePlayerData.MaxLevel)
        {
            return;
        }

        runtimePlayerData.currentLevel += 1;
    }

    public float GetNormalizedSpeed()
    {
        float normalized = currentMoveSpeed / runtimePlayerData.currentRunSpeed;
        return Mathf.Clamp01(normalized);
    }

    public Transform GetPlayerTransform()
    {
        return transform;
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

    public bool IsDead()
    {
        return isDead;
    }

    public void ResetDeadFlag()
    {
        isDead = false;
    }

    public void SetIsInAction(bool value)
    {
        isInAction = value;
    }

    public bool IsInAction()
    {
        return isInAction;
    }

    public void SetCurrentFishingManager(FishingManager fm)
    {
        currentFishingManager = fm;
    }

    public FishingManager GetCurrentFishingManager()
    {
        return currentFishingManager;
    }

    public PlayerCombat GetPlayerCombat()
    {
        return playerCombat;
    }

    public InventoryScriptableObject GetPlayerCombatEquipmentInventory()
    {
        return combatEquipmentInventory;
    }

    public InventoryScriptableObject GetPlayerLifeSkillEquipmentInventory()
    {
        return lifeSkillEquipmentInventory;
    }

    [ContextMenu("Save")]
    public void SavePlayerData()
    {
        string fullSavePath = Application.persistentDataPath + savePath;
        PlayerSaveData saveData = new PlayerSaveData();

        saveData.CurrentHealth = runtimePlayerData.currentHealth;
        saveData.MaxHealth = runtimePlayerData.currentMaxHealth;
        saveData.CurrentStamina = runtimePlayerData.currentStamina;
        saveData.MaxStamina = runtimePlayerData.currentMaxStamina;
        saveData.CurrentMana = runtimePlayerData.currentMana;
        saveData.MaxMana = runtimePlayerData.currentMaxMana;

        saveData.CurrentATK = runtimePlayerData.currentATK;
        saveData.CurrentDEF = runtimePlayerData.currentDEF;
        saveData.CurrentEvasion = runtimePlayerData.currentEvasion;
        saveData.CurrentDamageReduction = runtimePlayerData.currentDamageReduction;

        saveData.CurrentWalkSpeed = runtimePlayerData.currentWalkSpeed;
        saveData.CurrentRunSpeed = runtimePlayerData.currentRunSpeed;
        saveData.CurrentHoldingItemWalkSpeed = runtimePlayerData.currentHoldingItemWalkSpeed;

        saveData.CurrentLevel = runtimePlayerData.currentLevel;
        saveData.CurrentXP = runtimePlayerData.currentXP;
        saveData.CurrentDeathPenalty = runtimePlayerData.currentDeathPenalty;

        saveData.CurrentFishingXPGain = runtimePlayerData.currentFishingXPGain;
        saveData.CurrentMiningXPGain = runtimePlayerData.currentMiningXPGain;

        saveData.CurrentItemDropRate = runtimePlayerData.currentItemDropRate;
        saveData.CurrentStaminaRecoverRate = runtimePlayerData.currentStaminaRecoverRate;

        saveData.CurrentFishingTime = runtimePlayerData.currentFishingTime;
        saveData.CurrentMiningTime = runtimePlayerData.currentMiningTime;

        saveData.CurrentSilverCoin = runtimePlayerData.currentSilverCoin;

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(fullSavePath, json);

        Debug.Log("Đã lưu dữ liệu nhân vật tại: " + fullSavePath);
    }

    [ContextMenu("Load")]
    public void LoadPlayerData()
    {
        string fullSavePath = string.Concat(Application.persistentDataPath, savePath);

        if (!File.Exists(fullSavePath)) return;

        string json = File.ReadAllText(fullSavePath);
        PlayerSaveData saveData = JsonUtility.FromJson<PlayerSaveData>(json);

        runtimePlayerData.currentHealth = saveData.CurrentHealth;
        runtimePlayerData.currentMaxHealth = saveData.MaxHealth;
        runtimePlayerData.currentMana = saveData.CurrentMana;
        runtimePlayerData.currentMaxMana = saveData.MaxMana;
        runtimePlayerData.currentStamina = saveData.CurrentStamina;
        runtimePlayerData.currentMaxStamina = saveData.MaxStamina;

        runtimePlayerData.currentATK = saveData.CurrentATK;
        runtimePlayerData.currentDEF = saveData.CurrentDEF;
        runtimePlayerData.currentEvasion = saveData.CurrentEvasion;
        runtimePlayerData.currentDamageReduction = saveData.CurrentDamageReduction;

        runtimePlayerData.currentWalkSpeed = saveData.CurrentWalkSpeed;
        runtimePlayerData.currentRunSpeed = saveData.CurrentRunSpeed;
        runtimePlayerData.currentHoldingItemWalkSpeed = saveData.CurrentHoldingItemWalkSpeed;

        runtimePlayerData.currentLevel = saveData.CurrentLevel;
        runtimePlayerData.currentXP = saveData.CurrentXP;
        runtimePlayerData.currentDeathPenalty = saveData.CurrentDeathPenalty;

        runtimePlayerData.currentFishingXPGain = saveData.CurrentFishingXPGain;
        runtimePlayerData.currentMiningXPGain = saveData.CurrentMiningXPGain;

        runtimePlayerData.currentItemDropRate = saveData.CurrentItemDropRate;
        runtimePlayerData.currentStaminaRecoverRate = saveData.CurrentStaminaRecoverRate;

        runtimePlayerData.currentFishingTime = saveData.CurrentFishingTime;
        runtimePlayerData.currentMiningTime = saveData.CurrentMiningTime;

        runtimePlayerData.currentSilverCoin = saveData.CurrentSilverCoin;

        Debug.Log("Đã tải dữ liệu nhân vật được lưu tại: " + fullSavePath);
    }

    private void OnEnable()
    {
        combatEquipmentInventory.OnEquipmentChanged += OnEquipmentChanged;
        lifeSkillEquipmentInventory.OnEquipmentChanged += OnEquipmentChanged;
    }

    private void OnDisable()
    {
        combatEquipmentInventory.OnEquipmentChanged -= OnEquipmentChanged;
        lifeSkillEquipmentInventory.OnEquipmentChanged -= OnEquipmentChanged;
    }
}

[System.Serializable]
public class PlayerSaveData
{
    public float CurrentHealth;
    public float MaxHealth;
    public float CurrentStamina;
    public float MaxStamina;
    public float CurrentMana;
    public float MaxMana;

    public int CurrentATK;
    public int CurrentDEF;
    public int CurrentEvasion;
    public int CurrentDamageReduction;

    public float CurrentWalkSpeed;
    public float CurrentRunSpeed;
    public float CurrentHoldingItemWalkSpeed;

    public int CurrentLevel;
    public float CurrentXP;
    public float CurrentDeathPenalty;

    public float CurrentFishingXPGain;
    public float CurrentMiningXPGain;

    public float CurrentItemDropRate;
    public float CurrentStaminaRecoverRate;

    public float CurrentFishingTime;
    public float CurrentMiningTime;

    public double CurrentSilverCoin;
}