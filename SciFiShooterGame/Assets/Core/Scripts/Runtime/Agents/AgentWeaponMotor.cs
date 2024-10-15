using System;
using System.Collections.Generic;
using System.Linq;
using Core.Scripts.Runtime.Ammo;
using Core.Scripts.Runtime.Utilities;
using Core.Scripts.Runtime.Weapons;
using Core.Scripts.Runtime.Items;
using UnityEngine;
using UnityEngine.Animations.Rigging;      

namespace Core.Scripts.Runtime.Agents
{    
    public class AgentWeaponMotor : MonoBehaviour, IItemPickUP<WeaponType>, UtilityEvent
  {                
    public event Action NotifyAction;      
    private Agent _agent;      
     
    [Header("Actual Weapon Type")]
    [SerializeField] private WeaponType _actualWeaponType; 
    
    [Header("Weapon Settings")]      
     private Weapon[] _totalWeaponsHolder;
    [SerializeField] private List<Weapon> _agentWeaponsSlots = new List<Weapon>();    
     private Weapon _currentWeapon;
     private int _currentIndex;       
     private const int _maxWeaponsSlotsAllowed = 3;

     public Weapon CurrentWeapon() => _currentWeapon;      
    
    [Header("Left hand IK")] 
    [SerializeField] private TwoBoneIKConstraint _leftHandIK;
    [SerializeField] private Transform _leftHandIK_Target;
    [SerializeField] private float _leftHandIKWeightIncreaseRate;
     private WeaponAnimations _weaponAnimations;      
     private Transform _assignLeftHandCurrentWeapon;
     private bool _shouldIncrease_LeftHandIKWeight;
    
    [Header("Rig")]
    [SerializeField] private Rig _rig;
    [SerializeField] private float _rigWeightIncreaseRate;
     private bool _shouldIncrease_RigWeight;
     private bool _isGrabbingWeapon;
     
     [Header("Ammo Settings")]
     [SerializeField] private GameObject _bulletPrefab;
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
        _rig ??= FindAnyObjectByType<Rig>();  
        _bulletPool ??= FindAnyObjectByType<BulletPool>();
        _weaponAnimations = new WeaponAnimations();
    }
         
    private void Start()
    {
        _totalWeaponsHolder = _agentWeaponsSlots.ToArray();
        _agent.AgentInputReader.NotifyCanShoot += WeaponShoot;
        _agent.AgentInputReader.NotifyWeaponSwitch += SwitchOffWeapons;
        _agent.AgentInputReader.NotifyMainWeaponSwitch += OnButtonPressed;
        _agent.AgentInputReader.NotifySecondaryWeaponSwitch += OnButtonPressed;
        _agent.AgentInputReader.NotifyMeleeWeaponSwitch += OnButtonPressed;
        _agent.AgentInputReader.NotifyWeaponReload += OnWeaponReload;    
        _agent.AgentInputReader.NotifyWhenWeaponDropped += DropWeapon;   
        AssignDefaultWeapon();
    }             

    private void OnDestroy()
    {
        _agent.AgentInputReader.NotifyCanShoot -= WeaponShoot;
        _agent.AgentInputReader.NotifyWeaponSwitch -= SwitchOffWeapons;
        _agent.AgentInputReader.NotifyMainWeaponSwitch -= OnButtonPressed;
        _agent.AgentInputReader.NotifySecondaryWeaponSwitch -= OnButtonPressed;
        _agent.AgentInputReader.NotifyMeleeWeaponSwitch -= OnButtonPressed;
        _agent.AgentInputReader.NotifyWeaponReload -= OnWeaponReload;
        _agent.AgentInputReader.NotifyWhenWeaponDropped -= DropWeapon;
    }  
  
    
    private void OnWeaponReload()
    {
        if (!_currentWeapon.WeaponDataConfiguration.CanReload()) return;
        if (_isGrabbingWeapon) return;

        float reloadSpeed = _currentWeapon.WeaponDataConfiguration.WeaponReloadSpeed;
        
        _agent.AgentAnimator.Animator.SetTrigger(_weaponAnimations.Reload);
        _agent.AgentAnimator.Animator.SetFloat(_weaponAnimations.WeaponReloadSpeed, reloadSpeed);
        ReduceRigWeight();
    }                              
    private void ReduceRigWeight() => _rig.weight = 0.15f;      
    public void MaximizeRigWeight() => _shouldIncrease_RigWeight = true; 
    public void MaximizeLeftHandWeight() => _shouldIncrease_LeftHandIKWeight = true;       

    private void Update()
    {
        ControlAnimationRig();       
        
        _agent.AgentAim.UpdateAimLaser(_currentWeapon.WeaponDataConfiguration.GunPoint, BulletDirection());
    }

    private void ControlAnimationRig()
    {
        if (_shouldIncrease_RigWeight)
        {
            _rig.weight += _rigWeightIncreaseRate * Time.deltaTime;

            if (_rig.weight >= 1)
                _shouldIncrease_RigWeight = false;
        }

        if (!_shouldIncrease_LeftHandIKWeight) return;
        _leftHandIK.weight += _leftHandIKWeightIncreaseRate * Time.deltaTime;

        if (_leftHandIK.weight >= 1)
        {
            _shouldIncrease_LeftHandIKWeight = false;
        }
    }

    private void PlayWeaponEquipAnimation(EquipType equipType, float currentWeapon)
    {
        float equipmentSpeed = currentWeapon;
        
        _leftHandIK.weight = 0;
        ReduceRigWeight();
        _agent.AgentAnimator.Animator.SetTrigger(_weaponAnimations.EquipWeapon);
        _agent.AgentAnimator.Animator.SetFloat(_weaponAnimations.EquipType, ((float)equipType)); 
        _agent.AgentAnimator.Animator.SetFloat(_weaponAnimations.EquipSpeed, equipmentSpeed);

        SetBusyGrabbingWeaponTo(true);
    }

    public void SetBusyGrabbingWeaponTo(bool isBusy)
    {
        _isGrabbingWeapon = isBusy;
        _agent.AgentAnimator.Animator.SetBool(_weaponAnimations.BusyEquippingWeapon, _isGrabbingWeapon);
    }

    private void AssignDefaultWeapon()
    {
        foreach (var weapon in _agentWeaponsSlots)
        {   
            if (weapon.WeaponDataConfiguration.WeaponType == _actualWeaponType)   
                WeaponConfig(weapon);  
            else
                weapon.gameObject.SetActive(false);      
        }                  
    }

    private void WeaponConfig(Weapon weapon)
    {
        AttachLeftHand(weapon.transform);
        SwitchAnimationLayer((int)weapon.WeaponDataConfiguration.AnimationLayer);
        PlayWeaponEquipAnimation(weapon.WeaponDataConfiguration.EquipType, weapon.WeaponDataConfiguration.WeaponEquipmentSpeed);
        _currentWeapon = weapon;
        weapon.gameObject.SetActive(true);
    }

    private void SwitchOffWeapons()
    {
        _agentWeaponsSlots[_currentIndex].gameObject.SetActive(false);     
        _currentIndex = (_currentIndex + 1) % _agentWeaponsSlots.Count;
        _agentWeaponsSlots[_currentIndex].gameObject.SetActive(true);
        _actualWeaponType = _agentWeaponsSlots[_currentIndex].WeaponDataConfiguration.WeaponType;
        AttachLeftHand(_agentWeaponsSlots[_currentIndex].gameObject.transform);
        SwitchAnimationLayer((int)_agentWeaponsSlots[_currentIndex].WeaponDataConfiguration.AnimationLayer);
        PlayWeaponEquipAnimation(_agentWeaponsSlots[_currentIndex].WeaponDataConfiguration.EquipType,
            _agentWeaponsSlots[_currentIndex].WeaponDataConfiguration.WeaponEquipmentSpeed);
        _currentWeapon = _agentWeaponsSlots[_currentIndex];
    }
    
    private void OnButtonPressed()
    {  
        bool weaponFound = false;   
        
        foreach (var weapon in _agentWeaponsSlots)
        {   
            if (weapon.WeaponDataConfiguration.WeaponInputSlot == _agent.AgentInputReader.WeaponSlotLocation)
            {
                weaponFound = true;      
               
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
            _currentIndex = _currentWeapon.WeaponDataConfiguration.WeaponInputSlot;
        }                          
        Debug.Log("Selected weapon not found in the list, keeping current weapon.");
    }

    private void AttachLeftHand(Transform weaponTransform)
    {
        _assignLeftHandCurrentWeapon = weaponTransform;
        
        Transform targetTransform =
            _assignLeftHandCurrentWeapon.GetComponentInChildren<LeftHandTargetTransform>().transform;
        _leftHandIK_Target.localPosition = targetTransform.localPosition;
        _leftHandIK_Target.localRotation = targetTransform.localRotation;
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 0; i < _agent.AgentAnimator.Animator.layerCount; i++)     
            _agent.AgentAnimator.Animator.SetLayerWeight(i, 0);   
        
        _agent.AgentAnimator.Animator.SetLayerWeight(layerIndex, 1);
    }

    private Vector3 BulletDirection()
    {
        Transform aim = _agent.AgentAim.Aim;
        
        Vector3 direction = (aim.position - _currentWeapon.WeaponDataConfiguration.GunPoint.position).normalized;

        if (_agent.AgentAim.CanAimPrecisely() == false && 
            _agent.AgentAim.Target(_agent.AgentAim.GetMouseHitInfo(Camera.main, 
                _agent.AgentInputReader.AimInputValue, _agent.AgentMovement.AimLayerMask)) == null)  
         direction.y = 0;           
        
        return direction;
    }

    private void WeaponShoot()
    {      
        if (_currentWeapon is not null && _currentWeapon.WeaponDataConfiguration.CanShoot())
        {
            
            Bullet newBullet = _bulletPool.GetObject();
                
            newBullet.gameObject.transform.position = _currentWeapon.WeaponDataConfiguration.GunPoint.position;
            newBullet.gameObject.transform.rotation = 
                Quaternion.LookRotation(_currentWeapon.WeaponDataConfiguration.GunPoint.forward);
            
            Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();
            rbNewBullet.mass = 5f / _bulletSpeed;
            rbNewBullet.linearVelocity = BulletDirection() * _bulletSpeed;      
        
            TriggerShootAnimation();
        }
        else  
            DoSomethingAndRefactorThis();  
    }

    private void DoSomethingAndRefactorThis() => Debug.Log("NEED MORE AMMO");  
    private void TriggerShootAnimation() => _agent.AgentAnimator.Animator.SetTrigger(_weaponAnimations.Fire);

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

    public void PickUpObject(WeaponType weaponType)
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