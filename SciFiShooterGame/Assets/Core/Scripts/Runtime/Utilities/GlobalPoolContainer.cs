using Core.Scripts.Runtime.Ammo;
using UnityEngine;

namespace Core.Scripts.Runtime.Utilities
{
    public class GlobalPoolContainer : GenericSingleton<GlobalPoolContainer>
    {
        [field: SerializeField] public BulletPool BulletPool { get; set; }
        [field: SerializeField] public BulletPoolImpact BulletPoolImpact{ get; set; }
        
    }
}