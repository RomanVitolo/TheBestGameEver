using Core.Scripts.Runtime.Ammo;
using Core.Scripts.Runtime.Items;
using Core.Scripts.Runtime.Utilities;
using Core.Scripts.Runtime.Weapons;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Scripts.Runtime.CameraSystem;
using UnityEngine;

namespace Core.Scripts.Runtime.Agents
{
    public class AgentWeaponMotor : MonoBehaviour, IItemPickUP<WeaponEnums.WeaponType>, IUtilityEvent
    {
        public event Action NotifyAction;
        
        private Agent _agent;
        private WeaponAnimations _weaponAnimations;

        [Header("Actual Weapon Type")]
        [SerializeField] private WeaponEnums.WeaponType _actualWeaponType;

        [Header("Weapon Settings")]
        private Weapon[] _totalWeaponsHolder;
        [SerializeField] private List<Weapon> _agentWeaponsSlots = new List<Weapon>();
        private Weapon _currentWeapon;
        private int _currentIndex;
        private const int _maxWeaponsSlotsAllowed = 3;
        private bool _weaponReady;
        private int weaponIndex = 1;
        
        public Weapon CurrentWeapon() => _currentWeapon;

        [Header("Ammo Settings")]
        [SerializeField] private float _bulletSpeed;
        [SerializeField] private BulletPool _bulletPool;
        
        private void Awake()
        {
            var getWeapons = GetComponentsInChildren<Weapon>(true);
            foreach (var weapon in getWeapons)
            {
                _agentWeaponsSlots.Add(weapon);
            }
            _agent = GetComponentInParent<Agent>();
            _weaponAnimations = GetComponent<WeaponAnimations>();
            _bulletPool ??= FindAnyObjectByType<BulletPool>();
        }

        private void Start()
        {
            _totalWeaponsHolder = _agentWeaponsSlots.ToArray();
            SubscribeAgentInput();
            AssignDefaultWeapon();
        }

        private void OnDestroy() => UnsubscribeAgentInput();
        
        private void SubscribeAgentInput() 
        {
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
            if (!_currentWeapon.WeaponDataConfiguration.CanReload() && !_weaponReady) return;

            _weaponAnimations.WeaponReloadAnimation(_currentWeapon.WeaponDataConfiguration.WeaponReloadSpeed);
        }
        
        private void Update()
        {
            _agent.AgentAim.UpdateAimVisuals(_currentWeapon.WeaponDataConfiguration.GunPoint, BulletDirection(), 
                _weaponReady, _currentWeapon.WeaponDataConfiguration.WeaponDistance);

            if (_agent.AgentInputReader.CanShoot)
                WeaponShoot();
        }

        private void AssignDefaultWeapon()
        {
            foreach (var weapon in _agentWeaponsSlots)
            {
                if (weapon.WeaponDataConfiguration.WeaponType == _actualWeaponType)
                {
                    WeaponConfig(weapon);
                }
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
            _agentWeaponsSlots[_currentIndex].gameObject.SetActive(false);
            _currentIndex = (_currentIndex + 1) % _agentWeaponsSlots.Count;
            _agentWeaponsSlots[_currentIndex].gameObject.SetActive(true);
            CameraSystemBehaviour.Instance.ChangeCameraDistance(_currentWeapon.WeaponDataConfiguration.CameraDistance);
            _actualWeaponType = _agentWeaponsSlots[_currentIndex].WeaponDataConfiguration.WeaponType;
            _weaponAnimations.AttachLeftHand(_agentWeaponsSlots[_currentIndex].gameObject.transform);
            _weaponAnimations.SwitchAnimationLayer((int)_agentWeaponsSlots[_currentIndex].WeaponDataConfiguration.AnimationLayer);
            _weaponAnimations.PlayWeaponEquipAnimation(_agentWeaponsSlots[_currentIndex].WeaponDataConfiguration.EquipType,
                _agentWeaponsSlots[_currentIndex].WeaponDataConfiguration.WeaponEquipmentSpeed);
            _currentWeapon = _agentWeaponsSlots[_currentIndex];
            weaponIndex = 0;
        }

        private void SwitchWeaponFireMode()
        {
            _currentWeapon.WeaponDataConfiguration.FireMode = 
                _currentWeapon.WeaponDataConfiguration.WeaponFireMode.FireModeTypesList[weaponIndex].FireModeType;
            weaponIndex = (weaponIndex +1) % _currentWeapon.WeaponDataConfiguration.WeaponFireMode.FireModeTypesList.Count;
        }

        private void EquipWeaponBySpecificButtonPressed() // Actually 1,2,3 (buttons)
        {
            bool weaponFound = false;
            weaponIndex = 0;
            foreach (var weapon in _agentWeaponsSlots)
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
                CameraSystemBehaviour.Instance.ChangeCameraDistance(_currentWeapon.WeaponDataConfiguration.CameraDistance);
                _currentIndex = _currentWeapon.WeaponDataConfiguration.WeaponInputSlot;
            }
            Debug.Log("Selected weapon not found in the list, keeping current weapon.");
        }

        private Vector3 BulletDirection()
        {
            Transform aim = _agent.AgentAim.Aim;

            Vector3 direction = (aim.position - _currentWeapon.WeaponDataConfiguration.GunPoint.position).normalized;

            if (!_agent.AgentAim.CanAimPrecisely() &&
                _agent.AgentAim.Target(_agent.AgentAim.GetMouseHitInfo(Camera.main,
                    _agent.AgentInputReader.AimInputValue, _agent.AgentMovement.AimLayerMask)) == null)
                direction.y = 0;

            return direction;
        }

        private void WeaponShoot()
        {
            if (!_weaponReady) return;
            
            if (_currentWeapon.WeaponDataConfiguration.FireMode == WeaponEnums.FireModeType.Single)
                _agent.AgentInputReader.CanShoot = false;
            
            if (_currentWeapon != null && _currentWeapon.WeaponDataConfiguration.ReadyToShoot())
            {
                _weaponAnimations.TriggerShootAnimation();

                var fireModeSystem = new FireModeSystem();
                fireModeSystem.HandleFireMode(this);
            }
            else if(_currentWeapon.WeaponDataConfiguration.AmmoInMagazine == 0)
                EmptyMagazine();
        }

        public void FireSingleBullet()
        {
            _currentWeapon.WeaponDataConfiguration.AmmoInMagazine--;
            
            Bullet newBullet = _bulletPool.GetObject();

            newBullet.gameObject.transform.SetPositionAndRotation(
                _currentWeapon.WeaponDataConfiguration.GunPoint.position,
                Quaternion.LookRotation(_currentWeapon.WeaponDataConfiguration.GunPoint.forward));

            Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();

            var bullet = newBullet.GetComponent<Bullet>();
            bullet.BulletSetup(_currentWeapon.WeaponDataConfiguration.WeaponDistance);
                
            Vector3 bulletDirection = _currentWeapon.WeaponDataConfiguration.ApplyRecoil(BulletDirection());
                
            rbNewBullet.mass = 5f / _bulletSpeed;
            rbNewBullet.linearVelocity = bulletDirection * _bulletSpeed;
        }
        
        public IEnumerator BurstFireMode()
        {
            SetWeaponReady(false);
            
            foreach (var type in _currentWeapon.WeaponDataConfiguration.WeaponFireMode.FireModeTypesList
                         .Where(type => type.FireModeType 
                                        == _currentWeapon.WeaponDataConfiguration.FireMode))
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
            if (_agentWeaponsSlots.Count <= 1) return;

            _agentWeaponsSlots.Remove(_currentWeapon);
            _currentWeapon.gameObject.SetActive(false);
            _currentWeapon = _agentWeaponsSlots.FirstOrDefault();
            if (_currentWeapon == null) return;
            _actualWeaponType = _currentWeapon.WeaponDataConfiguration.WeaponType;
            _currentWeapon.gameObject.SetActive(true);
            _currentIndex = _currentWeapon.WeaponDataConfiguration.WeaponInputSlot;
        }

        public void SetWeaponReady(bool ready) => _weaponReady = ready;

        public void PickUpObject(WeaponEnums.WeaponType weaponType)
        {
            if (_agentWeaponsSlots.Count >= _maxWeaponsSlotsAllowed && _agentWeaponsSlots.Exists(
                    w => w.WeaponDataConfiguration.WeaponType != weaponType))
                return;

            foreach (var newWeapon in _totalWeaponsHolder.
                         Where(newWeapon => newWeapon.WeaponDataConfiguration.WeaponType == weaponType))
            {
                NotifyAction?.Invoke();
                _agentWeaponsSlots.Add(newWeapon);
                return;
            }
        }
    }
}