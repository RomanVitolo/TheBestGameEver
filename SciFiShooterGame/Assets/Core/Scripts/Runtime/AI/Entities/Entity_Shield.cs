using UnityEngine;

namespace Core.Scripts.Runtime.AI.Entities
{
    public class Entity_Shield : MonoBehaviour
    {
        private static readonly int ChaseIndex = Animator.StringToHash("ChaseIndex");
        [field: SerializeField] public int ShieldDurability { get; set; }

       private Entity_Melee _entityMelee;

       private void Awake()
       {
           _entityMelee = GetComponentInParent<Entity_Melee>();
       }

       public void ReduceDurability()
       {
           ShieldDurability--;

           if (ShieldDurability <= 0)
           {
               _entityMelee.Animator.SetFloat(ChaseIndex, 0);
               Destroy(gameObject);
           }
       }
    }
}