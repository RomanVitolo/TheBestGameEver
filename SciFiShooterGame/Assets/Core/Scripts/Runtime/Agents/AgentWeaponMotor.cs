using Core.Scripts.Runtime.Weapons;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Scripts.Runtime.CameraSystem;
using Unity.Netcode;
using UnityEngine;

namespace Core.Scripts.Runtime.Agents
{
    /// <summary>
    /// Weapon handling for one agent. The equipped weapon is a NetworkVariable, so switching replicates to
    /// every peer; owned weapons are a server-written bitmask over <see cref="TotalWeaponsHolder"/>. Input,
    /// aiming and firing run only on the owning client; the equip state is applied on all peers.
    /// </summary>
    public class AgentWeaponMotor : NetworkBehaviour
    {
        private Agent _agent;
        private WeaponAnimations _weaponAnimations;
        private WeaponBulletMovement _weaponBulletMovement;
        private CameraSystemBehaviour _cameraSystem;

        [Header("Actual Weapon Type")]
        [SerializeField] private WeaponEnums.WeaponType _actualWeaponType;

        [Header("Weapon Settings")]
        [SerializeField] private AgentWeaponDrop _agentWeaponDrop;
        public List<Weapon> AgentWeaponsSlot = new List<Weapon>();
        public Weapon[] TotalWeaponsHolder;
        private Weapon _currentWeapon;
        private int _currentIndex = -1;
        private bool _weaponReady;
        private int weaponIndex = 1;
        private bool _inputSubscribed;
        public Weapon CurrentWeapon() => _currentWeapon;

        // Bit i set = TotalWeaponsHolder[i] is owned. Server-authoritative: pickups set bits (later slice).
        private readonly NetworkVariable<int> _ownedMask = new();
        // Index into TotalWeaponsHolder of the equipped weapon, or -1. Owner-authoritative so switching is
        // instant on the acting client; it replicates to the server and everyone else.
        private readonly NetworkVariable<int> _equippedIndex =
            new(-1, writePerm: NetworkVariableWritePermission.Owner);

        private bool IsAliveOwner => IsSpawned && IsOwner && _agent.Health.IsAlive;

        private void Awake()
        {
            _agent = GetComponentInParent<Agent>();
            _weaponBulletMovement = GetComponent<WeaponBulletMovement>();
            _weaponAnimations = GetComponent<WeaponAnimations>();
            if (_agentWeaponDrop != null)
                _agentWeaponDrop = GetComponent<AgentWeaponDrop>();

            // The roster is the fixed set of weapon children — identical on every peer because it comes from
            // the same prefab hierarchy, so an index into it means the same weapon everywhere.
            AgentWeaponsSlot = GetComponentsInChildren<Weapon>(true).ToList();
            TotalWeaponsHolder = AgentWeaponsSlot.ToArray();
        }

        public override void OnNetworkSpawn()
        {
            // Set the initial values before subscribing, so seeding them here does not fire a spurious
            // OnValueChanged (which would play an equip animation on spawn).
            if (IsServer)
                _ownedMask.Value = DefaultOwnedMask();
            if (IsOwner)
                _equippedIndex.Value = DefaultEquippedIndex();

            _ownedMask.OnValueChanged += OnOwnedChanged;
            _equippedIndex.OnValueChanged += OnEquippedChanged;

            if (IsOwner)
            {
                _cameraSystem = FindFirstObjectByType<CameraSystemBehaviour>();
                SubscribeAgentInput();
            }

            // Show the starting weapon (active object, IK, layer) but do NOT play the equip animation here:
            // NetworkAnimator.SetTrigger is unsafe during OnNetworkSpawn because the NetworkAnimator on the
            // model child may not have run its own OnNetworkSpawn yet (its state handler would still be null).
            // The equip animation — whose end-event makes the weapon ready to fire — is played from Start().
            ApplyWeaponState(_equippedIndex.Value, playEquipAnim: false);
        }

        private void Start()
        {
            // By Start, every behaviour's OnNetworkSpawn (including the NetworkAnimator's) has run, so the
            // equip trigger is safe. This plays the starting-weapon equip whose end-event readies the weapon.
            if (IsSpawned && IsOwner && _currentWeapon != null)
                _weaponAnimations.PlayWeaponEquipAnimation(
                    _currentWeapon.WeaponDataConfiguration.EquipType,
                    _currentWeapon.WeaponDataConfiguration.WeaponEquipmentSpeed);
        }

        public override void OnNetworkDespawn()
        {
            _ownedMask.OnValueChanged -= OnOwnedChanged;
            _equippedIndex.OnValueChanged -= OnEquippedChanged;
            UnsubscribeAgentInput();
        }

        public override void OnDestroy()
        {
            UnsubscribeAgentInput();
            base.OnDestroy();
        }

        private void SubscribeAgentInput()
        {
            if (_inputSubscribed) return;
            _inputSubscribed = true;

            _agent.AgentInputReader.NotifyWeaponSwitch += SwitchOffWeaponsByGenericButtonPressed;
            _agent.AgentInputReader.NotifyMainWeaponSwitch += EquipWeaponBySpecificButtonPressed;
            _agent.AgentInputReader.NotifySecondaryWeaponSwitch += EquipWeaponBySpecificButtonPressed;
            _agent.AgentInputReader.NotifyMeleeWeaponSwitch += EquipWeaponBySpecificButtonPressed;
            _agent.AgentInputReader.NotifyWeaponReload += OnWeaponReload;
            _agent.AgentInputReader.NotifyWhenWeaponDropped += DropWeapon;
            _agent.AgentInputReader.NotifyWhenWeaponFireModeChanged += SwitchWeaponFireMode;
        }

        private void UnsubscribeAgentInput()
        {
            if (!_inputSubscribed) return;
            _inputSubscribed = false;

            _agent.AgentInputReader.NotifyWeaponSwitch -= SwitchOffWeaponsByGenericButtonPressed;
            _agent.AgentInputReader.NotifyMainWeaponSwitch -= EquipWeaponBySpecificButtonPressed;
            _agent.AgentInputReader.NotifySecondaryWeaponSwitch -= EquipWeaponBySpecificButtonPressed;
            _agent.AgentInputReader.NotifyMeleeWeaponSwitch -= EquipWeaponBySpecificButtonPressed;
            _agent.AgentInputReader.NotifyWeaponReload -= OnWeaponReload;
            _agent.AgentInputReader.NotifyWhenWeaponDropped -= DropWeapon;
            _agent.AgentInputReader.NotifyWhenWeaponFireModeChanged -= SwitchWeaponFireMode;
        }

        // ---------------- replicated equip state ----------------

        // A newly-owned weapon does not auto-equip, so there is nothing to re-apply visually here yet. When
        // pickups land, a bit change may re-enable a weapon the owner subsequently switches to.
        private void OnOwnedChanged(int previous, int current) { }

        private void OnEquippedChanged(int previous, int current) =>
            ApplyWeaponState(current, playEquipAnim: true);

        /// <summary>
        /// Runs on every peer. Activates the equipped weapon (deactivating the rest), re-targets the left-hand
        /// IK, and switches the animation layer. The equip animation itself is played only by the owner, whose
        /// NetworkAnimator trigger replays it on everyone else.
        /// </summary>
        private void ApplyWeaponState(int index, bool playEquipAnim)
        {
            foreach (var weapon in TotalWeaponsHolder)
                weapon.gameObject.SetActive(false);

            SetWeaponReady(false);

            if (index < 0 || index >= TotalWeaponsHolder.Length)
            {
                _currentWeapon = null;
                _currentIndex = -1;
                return;
            }

            Weapon equipped = TotalWeaponsHolder[index];
            _currentWeapon = equipped;
            _currentIndex = index;
            _actualWeaponType = equipped.WeaponDataConfiguration.WeaponType;

            equipped.gameObject.SetActive(true);
            _weaponAnimations.AttachLeftHand(equipped.transform);
            _weaponAnimations.SwitchAnimationLayer((int)equipped.WeaponDataConfiguration.AnimationLayer);

            if (!IsOwner) return;

            if (playEquipAnim)
                _weaponAnimations.PlayWeaponEquipAnimation(
                    equipped.WeaponDataConfiguration.EquipType,
                    equipped.WeaponDataConfiguration.WeaponEquipmentSpeed);

            if (_cameraSystem != null)
                _cameraSystem.ChangeCameraDistance(equipped.WeaponDataConfiguration.CameraDistance);
        }

        // ---------------- owner input ----------------

        private void EquipWeaponBySpecificButtonPressed() // 1 / 2 / 3
        {
            int index = OwnedIndexForSlot(_agent.AgentInputReader.WeaponSlotLocation);
            if (index < 0 || index == _currentIndex) return;

            weaponIndex = 0;
            _equippedIndex.Value = index; // owner write -> replicates -> ApplyWeaponState on every peer
        }

        private void SwitchOffWeaponsByGenericButtonPressed() // cycle (Mouse3)
        {
            int next = NextOwnedIndex(_currentIndex);
            if (next < 0 || next == _currentIndex) return;

            weaponIndex = 0;
            _equippedIndex.Value = next;
        }

        private void OnWeaponReload()
        {
            if (_currentWeapon == null) return;

            SetWeaponReady(false);
            if (!_currentWeapon.Runtime.CanReload() && !_weaponReady) return;

            _weaponAnimations.WeaponReloadAnimation(_currentWeapon.WeaponDataConfiguration.WeaponReloadSpeed);
        }

        private void SwitchWeaponFireMode()
        {
            if (_currentWeapon == null) return;

            _currentWeapon.Runtime.CycleFireMode(weaponIndex);
            weaponIndex = (weaponIndex + 1) %
                          _currentWeapon.WeaponDataConfiguration.WeaponFireMode.FireModeTypesList.Count;
        }

        private void Update()
        {
            if (!IsAliveOwner || _currentWeapon == null) return;

            Transform gunPoint = _currentWeapon.Runtime.GunPoint;

            _agent.AgentAim.UpdateAimVisuals(gunPoint,
                _weaponBulletMovement.BulletDirection(gunPoint.position),
                _weaponReady, _currentWeapon.WeaponDataConfiguration.WeaponDistance);

            if (_agent.AgentInputReader.CanShoot)
                WeaponShoot();
        }

        private void WeaponShoot()
        {
            if (!_weaponReady) return;

            if (_currentWeapon.Runtime.FireMode == WeaponEnums.FireModeType.Single)
                _agent.AgentInputReader.CanShoot = false;

            if (_currentWeapon != null && _currentWeapon.Runtime.ReadyToShoot())
            {
                _weaponAnimations.TriggerShootAnimation();

                var fireModeSystem = new FireModeSystem();
                fireModeSystem.HandleFireMode(this);
            }
            else if (!_currentWeapon.Runtime.HaveEnoughBullets())
                EmptyMagazine();
        }

        public void FireSingleBullet()
        {
            WeaponRuntime runtime = _currentWeapon.Runtime;
            Transform gunPoint = runtime.GunPoint;

            runtime.ConsumeBullet();

            // Recoil is drawn from Random, so the direction is settled here, once, and replicated verbatim.
            var fireParams = new BulletFireParams
            {
                Origin = gunPoint.position,
                Direction = runtime.ApplyRecoil(_weaponBulletMovement.BulletDirection(gunPoint.position)),
                FlyDistance = _currentWeapon.WeaponDataConfiguration.WeaponDistance,
                ImpactForce = _currentWeapon.WeaponDataConfiguration.BulletImpactForce,
                BulletSpeed = _weaponBulletMovement.BulletSpeed
            };

            _agent.WeaponFire.Fire(fireParams);
        }

        public IEnumerator BurstFireMode()
        {
            SetWeaponReady(false);

            foreach (var type in _currentWeapon.WeaponDataConfiguration.WeaponFireMode.FireModeTypesList
                         .Where(type => type.FireModeType == _currentWeapon.Runtime.FireMode))
            {
                for (int i = 1; i <= type.BulletsPerShotInBurstMode(); i++)
                {
                    FireSingleBullet();
                    yield return new WaitForSeconds(type.BurstModeDelay());

                    if(i >= type.BulletsPerShotInBurstMode())
                        SetWeaponReady(true);
                }
            }
        }

        private void EmptyMagazine() => Debug.Log("NEED MORE AMMO");

        private void DropWeapon()
        {
            _agentWeaponDrop.DropWeapon(AgentWeaponsSlot, _currentWeapon, _actualWeaponType, _currentIndex);
            SetWeaponReady(true);
        }

        public void SetWeaponReady(bool ready) => _weaponReady = ready;

        public Rigidbody GetRigidbody;

        // ---------------- roster helpers ----------------

        private bool IsOwned(int index) =>
            index >= 0 && index < TotalWeaponsHolder.Length && (_ownedMask.Value & (1 << index)) != 0;

        // The starting loadout is every weapon present on the prefab; pickups will start some unowned later.
        private int DefaultOwnedMask()
        {
            int mask = 0;
            for (int i = 0; i < TotalWeaponsHolder.Length; i++)
                mask |= 1 << i;
            return mask;
        }

        // Computed from the local roster only (no owned-mask dependency), so it is safe before the mask syncs.
        private int DefaultEquippedIndex()
        {
            for (int i = 0; i < TotalWeaponsHolder.Length; i++)
                if (TotalWeaponsHolder[i].WeaponDataConfiguration.WeaponType == _actualWeaponType)
                    return i;

            return TotalWeaponsHolder.Length > 0 ? 0 : -1;
        }

        private int OwnedIndexForSlot(int inputSlot)
        {
            for (int i = 0; i < TotalWeaponsHolder.Length; i++)
                if (IsOwned(i) && TotalWeaponsHolder[i].WeaponDataConfiguration.WeaponInputSlot == inputSlot)
                    return i;
            return -1;
        }

        private int NextOwnedIndex(int from)
        {
            int n = TotalWeaponsHolder.Length;
            if (n == 0) return -1;

            for (int step = 1; step <= n; step++)
            {
                int i = (((from < 0 ? -1 : from) + step) % n + n) % n;
                if (IsOwned(i)) return i;
            }
            return -1;
        }
    }
}
