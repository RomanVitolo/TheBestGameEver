using Core.Scripts.Runtime.Utilities;

namespace Core.Scripts.Runtime.Ammo
{
    public class BulletPoolImpact : BaseObjectPool<BulletImpact>
    {
        protected override void Start() => objectPool = 
            new ObjectPool<BulletImpact>(_prefabType, _initialPoolSize, _objectParent);

        public override void ReturnObject(BulletImpact obj) => base.ReturnObject(obj);
    }
}