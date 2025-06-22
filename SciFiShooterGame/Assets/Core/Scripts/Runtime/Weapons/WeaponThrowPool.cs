using Core.Scripts.Runtime.AI.Entities.StateMachine;
using Core.Scripts.Runtime.Utilities;

namespace Core.Scripts.Runtime.Weapons
{
    public class WeaponThrowPool : BaseObjectPool<Entity_WeaponThrow>
    {
        protected override void Start() => objectPool = 
            new ObjectPool<Entity_WeaponThrow>(_prefabType, _initialPoolSize, _objectParent);

        public override void ReturnObject(Entity_WeaponThrow obj) => base.ReturnObject(obj);
    }
}