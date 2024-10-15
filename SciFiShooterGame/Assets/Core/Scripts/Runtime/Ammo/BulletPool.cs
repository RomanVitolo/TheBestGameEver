using Core.Scripts.Runtime.Utilities;

namespace Core.Scripts.Runtime.Ammo
{
    public class BulletPool : BaseObjectPool<Bullet>
    {
        protected override void Start() => objectPool = 
            new ObjectPool<Bullet>(_prefabType, _initialPoolSize, _objectParent);

        public override void ReturnObject(Bullet obj) => base.ReturnObject(obj);
    }
}