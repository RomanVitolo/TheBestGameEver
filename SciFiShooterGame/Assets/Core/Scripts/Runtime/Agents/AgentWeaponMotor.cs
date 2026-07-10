using Core.Scripts.Runtime.Weapons;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Scripts.Runtime.CameraSystem;
using UnityEngine;

namespace Core.Scripts.Runtime.Agents
{
    /// <summary>
    /// Weapon handling for one agent. Equipping runs on every peer so remote players visibly hold the right
    /// weapon, but input, aiming and firing run only on the owning client. Bullets are still spawned into a
    /// local pool, so they are not yet replicated: see the networked-shooting slice.
    /// </summary>
    public class AgentWeaponMotor : MonoBehaviour
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
        private int _currentIndex;
        private bool _weaponReady;
        private int weaponIndex = 1;
        private bool _inputSubscribed;
        public Weapon CurrentWeapon() => _currentWeapon;

        private bool IsOwner => _agent.IsSpawned && _agent.IsOwner && _agent.Health.IsAlive;

        private void Awake()
        {
            var getWeapons = GetComponentsInChildren<Weapon>(true);
            foreach (var weapon in getWeapons)
            {
                AgentWeaponsSlot.Add(weapon);
            }

            _agent = GetComponentInParent<Agent>();
            _weaponBulletMovement = GetComponent<WeaponBulletMovement>();
            _weaponAnimations = GetComponent<WeaponAnimations>();
            if (_agentWeaponDrop != null)
                _agentWeaponDrop = GetComponent<AgentWeaponDrop>();
        }

        // Runs after OnNetworkSpawn for dynamically spawned NetworkObjects, so ownership is known here.
        private void Start()
        {
            TotalWeaponsHolder = AgentWeaponsSlot.ToArray();
            AssignDefaultWeapon();

            if (!IsOwner) return;

            _cameraSystem = FindFirstObjectByType<CameraSystemBehaviour>();
            SubscribeAgentInput();
        }

        private void OnDestroy() => UnsubscribeAgentInput();

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

        private void OnWeaponReload()
        {
            SetWeaponReady(false);
            if (!_currentWeapon.Runtime.CanReload() && !_weaponReady) return;

            _weaponAnimations.WeaponReloadAnimation(_currentWeapon.WeaponDataConfiguration.WeaponReloadSpeed);
        }

        private void Update()
        {
            if (!IsOwner || _currentWeapon == null) return;

            Transform gunPoint = _currentWeapon.Runtime.GunPoint;

            _agent.AgentAim.UpdateAimVisuals(gunPoint,
                _weaponBulletMovement.BulletDirection(gunPoint.position),
                _weaponReady, _currentWeapon.WeaponDataConfiguration.WeaponDistance);

            if (_agent.AgentInputReader.CanShoot)
                WeaponShoot();
        }

        private void AssignDefaultWeapon()
        {
            foreach (var weapon in AgentWeaponsSlot)
            {
                if (weapon.WeaponDataConfiguration.WeaponType == _actualWeaponType)
                    WeaponConfig(weapon);
                else
                    weapon.gameObject.SetActive(false);
            }
        }

        private void WeaponConfig(Weapon weapon)
        {
            SetWeaponReady(false);
            _weaponAnimations.AttachLeftHand(weapon.transform);
            _weaponAnimations.SwitchAnimationLayer((int)weapon.WeaponDataConfiguration.AnimationLayer);
            _weaponAnimations.PlayWeaponEquipAnimation(weapon.WeaponDataConfiguration.EquipType,
                weapon.WeaponDataConfiguration.WeaponEquipmentSpeed);
            _currentWeapon = weapon;
            weapon.gameObject.SetActive(true);
        }

        private void SwitchOffWeaponsByGenericButtonPressed() //Actually Mouse3
        {
            SetWeaponReady(false);
            AgentWeaponsSlot[_currentIndex].gameObject.SetActive(false);
            _currentIndex = (_currentIndex + 1) % AgentWeaponsSlot.Count;
            AgentWeaponsSlot[_currentIndex].gameObject.SetActive(true);
            _actualWeaponType = AgentWeaponsSlot[_currentIndex].WeaponDataConfiguration.WeaponType;
            _weaponAnimations.AttachLeftHand(AgentWeaponsSlot[_currentIndex].gameObject.transform);
            _weaponAnimations.SwitchAnimationLayer((int)AgentWeaponsSlot[_currentIndex].WeaponDataConfiguration.AnimationLayer);
            _weaponAnimations.PlayWeaponEquipAnimation(AgentWeaponsSlot[_currentIndex].WeaponDataConfiguration.EquipType,
                AgentWeaponsSlot[_currentIndex].WeaponDataConfiguration.WeaponEquipmentSpeed);
            _currentWeapon = AgentWeaponsSlot[_currentIndex];
            weaponIndex = 0;
        }

        private void SwitchWeaponFireMode()
        {
            _currentWeapon.Runtime.CycleFireMode(weaponIndex);
            weaponIndex = (weaponIndex + 1) %
                          _currentWeapon.WeaponDataConfiguration.WeaponFireMode.FireModeTypesList.Count;
        }

        private void EquipWeaponBySpecificButtonPressed() // Actually 1,2,3 (buttons)
        {
            bool weaponFound = false;
            weaponIndex = 0;
            foreach (var weapon in AgentWeaponsSlot)
            {
                if (weapon.WeaponDataConfiguration.WeaponInputSlot == _agent.AgentInputReader.WeaponSlotLocation)
                {
                    weaponFound = true;
                    SetWeaponReady(false);
                    _actualWeaponType = weapon.WeaponDataConfiguration.WeaponType;
                    WeaponConfig(weapon);
                    _currentIndex = weapon.WeaponDataConfiguration.WeaponInputSlot;
                }
                else
                {
                    if (_currentWeapon != null || weapon != _currentWeapon)
                    {
                        weapon.gameObject.SetActive(false);
                    }
                }
            }
            if (weaponFound) return;
            if (_currentWeapon != null)
            {
                _currentWeapon.gameObject.SetActive(true);

                if (_cameraSystem != null)
                    _cameraSystem.ChangeCameraDistance(_currentWeapon.WeaponDataConfiguration.CameraDistance);

                _currentIndex = _currentWeapon.WeaponDataConfiguration.WeaponInputSlot;
            }
            Debug.Log("Selected weapon not found in the list, keeping current weapon.");
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
    }
}
