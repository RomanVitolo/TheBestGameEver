using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Scripts.Runtime.Weapons
{
    /// <summary>
    /// Authored weapon configuration, read-only at runtime. Every mutable field (ammo, recoil, current fire
    /// mode, gun point) now lives on <see cref="WeaponRuntime"/>, one instance per <see cref="Weapon"/>,
    /// so two agents carrying the same weapon type no longer share a magazine.
    /// </summary>
    [CreateAssetMenu(menuName = "Core/Weapon Settings/Create AgentWeapon", fileName = "AgentWeapon")]
    [InlineEditor]
    public class WeaponDataSO : SerializedScriptableObject
    {
        [field: SerializeField] public string WeaponName { get; private set; }
        [field: SerializeField] public float CameraDistance { get; private set; }
        [field: SerializeField, BoxGroup("Weapon Settings")] public WeaponEnums.WeaponType WeaponType { get; private set; }
        [field: SerializeField, BoxGroup("Weapon Settings")] public WeaponEnums.FireModeType FireMode { get; private set; }
        [field: SerializeField, BoxGroup("Weapon Settings")] public int WeaponInputSlot { get; private set; }
        [field: SerializeField, Range(2,12), BoxGroup("Weapon Settings")] public float WeaponDistance { get; private set; }
        [field: SerializeField, BoxGroup("Weapon Settings"), Range(0,100)] public int WeaponDurability { get; private set; }
        [field: SerializeField, Range(1,5), BoxGroup("Weapon Settings")] public float WeaponReloadSpeed { get; private set; }
        [field: SerializeField, Range(1,5), BoxGroup("Weapon Settings")] public float WeaponEquipmentSpeed { get; private set; }
        [field: SerializeField, BoxGroup("Animation Layer Settings")] public WeaponEnums.WeaponAnimationLayerType AnimationLayer { get; private set; }
        [field: SerializeField, BoxGroup("Animation Layer Settings")] public WeaponEnums.EquipType EquipType { get; private set; }
        [field: SerializeField, BoxGroup("Weapon Fire Mode Data"), InlineEditor]
        public WeaponFireModeHolderSO WeaponFireMode { get; private set; }

        [field: SerializeField, BoxGroup("Ammo Settings"), PreviewField(100), HideLabel]
        public GameObject BulletPrefab { get; private set; }
        [field: SerializeField, VerticalGroup("Ammo Settings/Stats"), LabelWidth(100), GUIColor(0.3f,0.5f,1f)]
        public WeaponEnums.WeaponAmmoType AmmoType { get; private set; }
        [field: SerializeField, VerticalGroup("Ammo Settings/Stats"), LabelWidth(100), GUIColor(0.8f,0.4f,0.4f)]
        public float BulletMass { get; private set; }
        [field: SerializeField, VerticalGroup("Ammo Settings/Stats"), LabelWidth(100), GUIColor(1f,1f,0f)]
        public float BulletVelocity { get; private set; }
        [field: SerializeField, VerticalGroup("Ammo Settings/Stats"), LabelWidth(100), GUIColor(1f,1f,0f)]
        public float BulletImpactForce { get; private set; }

        // AmmoInMagazine and TotalReserveAmmo are authored starting values only. WeaponRuntime copies them
        // on construction and owns them from then on.
        [field: SerializeField, BoxGroup("Weapon Magazine")] public int AmmoInMagazine { get; private set; }
        [field: SerializeField, BoxGroup("Weapon Magazine")] public int MagazineCapacity { get; private set; }
        [field: SerializeField, BoxGroup("Weapon Magazine")] public int TotalReserveAmmo { get; private set; }
        [field: SerializeField, BoxGroup("Weapon Magazine")] public int InitialWeaponAmmo { get; private set; }
        [field: SerializeField, BoxGroup("Weapon Recoil")] public float BaseRecoil { get; private set; }
        [field: SerializeField, BoxGroup("Weapon Recoil")] public float MaximumRecoil { get; private set; }
        [field: SerializeField, BoxGroup("Weapon Recoil"), Range(0, 1)] public float RecoilIncreaseRate { get; private set; }
    }
}
