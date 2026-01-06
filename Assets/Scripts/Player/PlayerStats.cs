
[System.Serializable]
public class PlayerStats
{
    public float currentHealth;
    public float currentMaxHealth;
    public float currentStamina;
    public float currentMaxStamina;
    public float currentMana;
    public float currentMaxMana;

    public int currentATK;
    public int currentDEF;
    public int currentEvasion;
    public int currentDamageReduction;

    public float currentWalkSpeed;
    public float currentRunSpeed;
    public float currentHoldingItemWalkSpeed;

    public int currentLevel;
    public int MaxLevel;
    public float currentXP;
    public float MaxXP;
    public float currentDeathPenalty;

    public float currentFishingXP;

    public float currentItemDropRate;
    public float currentStaminaRecoverRate;

    public float currentFishingTime;

    public double currentSilverCoin;

    public void InitFrom(PlayerScriptableObject baseData)
    {
        currentHealth = baseData.Health;
        currentMaxHealth = baseData.Health;
        currentStamina = baseData.Stamina;
        currentMaxStamina = baseData.Stamina;
        currentMana = baseData.Mana;
        currentMaxMana = baseData.Mana;

        currentATK = baseData.ATK;
        currentDEF = baseData.DEF;
        currentEvasion = baseData.Evasion;
        currentDamageReduction = baseData.DamageReduction;

        currentWalkSpeed = baseData.WalkSpeed;
        currentRunSpeed = baseData.RunSpeed;
        currentHoldingItemWalkSpeed = baseData.HoldingItemWalkSpeed;

        currentLevel = baseData.Level;
        MaxLevel = 99;
        currentXP = 0f;
        MaxXP = 13034431f;
        currentDeathPenalty = baseData.DeathPenalty;

        currentFishingXP = baseData.FishingXP;

        currentItemDropRate = baseData.ItemDropRate;
        currentStaminaRecoverRate = baseData.StaminaRecoverRate;

        currentFishingTime = baseData.FishingTime;

        currentSilverCoin = 0;
    }
}
