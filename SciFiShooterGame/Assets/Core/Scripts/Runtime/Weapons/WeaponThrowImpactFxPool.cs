using Core.Scripts.Runtime.Utilities;

namespace Core.Scripts.Runtime.Weapons
{
    public class WeaponThrowImpactFxPool : BaseObjectPool<WeaponThrowImpactFx>
    {
        protected override void Start() => objectPool = 
            new ObjectPool<WeaponThrowImpactFx>(_prefabType, _initialPoolSize, _objectParent);

        public override void ReturnObject(WeaponThrowImpactFx obj) => base.ReturnObject(obj);
    }
}
    
