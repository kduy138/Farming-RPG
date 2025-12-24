
[System.Serializable]
public class PlayerStats
{
    public float currentHealth;
    public float currentMaxHealth;
    public float currentStamina;
    public float currentMana;

    public int currentATK;
    public int currentDEF;
    public int currentEvasion;
    public int currentDamageReduction;

    public float currentWalkSpeed;
    public float currentRunSpeed;
    public float currentHoldingItemWalkSpeed;

    public int currentLevel;
    public float currentXP;

    public float currentItemDropRate;

    public double currentSilverCoin;

    public void InitFrom(PlayerScriptableObject baseData)
    {
        currentHealth = baseData.Health;
        currentMaxHealth = baseData.Health;
        currentStamina = baseData.Stamina;
        currentMana = baseData.Mana;

        currentATK = baseData.ATK;
        currentDEF = baseData.DEF;
        currentEvasion = baseData.Evasion;
        currentDamageReduction = baseData.DamageReduction;

        currentWalkSpeed = baseData.WalkSpeed;
        currentRunSpeed = baseData.RunSpeed;
        currentHoldingItemWalkSpeed = baseData.HoldingItemWalkSpeed;

        currentLevel = baseData.Level;
        currentXP = 0f;

        currentItemDropRate = 0f;

        currentSilverCoin = 0;
    }
}
