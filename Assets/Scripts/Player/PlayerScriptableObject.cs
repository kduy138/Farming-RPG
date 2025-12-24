using UnityEngine;

[CreateAssetMenu(fileName = "PlayerScriptableObject", menuName = "Scriptable Objects/Player")]
public class PlayerScriptableObject : ScriptableObject
{
    [Header("Basic Stats")]
    [SerializeField]
    private float health;
    public float Health { get => health; private set => health = value; }
    [SerializeField]
    private float stamina;
    public float Stamina { get => stamina; private set => stamina = value; }
    [SerializeField]
    private float mana;
    public float Mana { get => mana; private set => mana = value; }

    [Header("Combat Stats")]
    [SerializeField]
    private int atk;
    public int ATK { get => atk; private set => atk = value; }
    [SerializeField]
    private int def;
    public int DEF { get => def; private set => def = value; }
    [SerializeField]
    private int evasion;
    public int Evasion { get => evasion; private set => evasion = value; }
    [SerializeField]
    private int damageReduction;
    public int DamageReduction { get => damageReduction; private set => damageReduction = value; }

    [Header("Move Speed")]
    [SerializeField]
    private float walkSpeed;
    public float WalkSpeed { get => walkSpeed; private set => walkSpeed = value; }
    [SerializeField]
    private float runSpeed;
    public float RunSpeed { get => runSpeed; private set => runSpeed = value; }
    [SerializeField]
    private float holdingItemWalkSpeed;
    public float HoldingItemWalkSpeed { get => holdingItemWalkSpeed; private set =>  holdingItemWalkSpeed = value; }

    [Header("Level")]
    [SerializeField]
    private int level;
    public int Level { get => level; private set => level = value; }
}
