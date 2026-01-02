using UnityEngine;

[CreateAssetMenu(fileName = "EnemyScriptableObject", menuName = "Scriptable Objects/Enemy")]
public class EnemyScriptableObject : ScriptableObject
{
    [Header("Basic stats")]
    [SerializeField]
    private float health;
    public float Health { get => health; private set => health = value; }
    [SerializeField]
    private float atkDamage;
    public float ATKDamage { get => atkDamage; private set => atkDamage = value; }
}
