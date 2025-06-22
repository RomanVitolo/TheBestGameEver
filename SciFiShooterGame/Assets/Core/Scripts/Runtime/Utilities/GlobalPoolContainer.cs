using Core.Scripts.Runtime.Ammo;
using Core.Scripts.Runtime.Weapons;
using UnityEngine;

namespace Core.Scripts.Runtime.Utilities
{
    public class GlobalPoolContainer : GenericSingleton<GlobalPoolContainer>
    {
        [field: SerializeField] public BulletPool BulletPool { get; set; }
        [field: SerializeField] public BulletPoolImpact BulletPoolImpact{ get; set; }
        [field: SerializeField] public WeaponThrowPool WeaponThrow{ get; set; }
        [field: SerializeField] public WeaponThrowImpactFxPool WeaponThrowImpactFx{ get; set; }
    }
}