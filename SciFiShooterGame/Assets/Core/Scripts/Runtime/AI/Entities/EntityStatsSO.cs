using UnityEngine;

namespace Core.Scripts.Runtime.AI.Entities
{
    [CreateAssetMenu(fileName = "New Entity Stats", menuName = "Core/Entity/BaseEntityStats", order = 0)]
    public class EntityStatsSO : ScriptableObject
    {
        [field: SerializeField] public AttackData AttackData { get; set; }
        [field: SerializeField] public float IdleTime { get; set; }
        [field: SerializeField] public float TurnSpeed { get; set; }
        [field: SerializeField] public float ChaseSpeed { get; set; }
        [field: SerializeField] public float AggressionRange { get; set; }
        [field: SerializeField] public int HealthPoints { get; set; }
        [field: SerializeField] public Transform[] PatrolPoints { get; set; }
    }
}