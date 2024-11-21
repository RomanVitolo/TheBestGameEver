using UnityEngine;

namespace Core.Scripts.Runtime.Weapons
{
    public abstract class WeaponFireModeType : ScriptableObject
    {
        [SerializeField] protected int bulletsPerShot;
        [SerializeField] protected float fireRate;
        [SerializeField] protected float fireDelay;
    }
}