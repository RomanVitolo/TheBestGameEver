using System.Collections.Generic;     
using Core.Scripts.Runtime.Utilities;
using Core.Scripts.Runtime.Weapon;
using UnityEngine;
using UnityEngine.Animations.Rigging;     

namespace Core.Scripts.Runtime.Agent
{    
    public class AgentWeaponMotor : MonoBehaviour
 {     
    [Header("Agent Objects")]
    [SerializeField] private Agent _agent;

    [Header("Weapon Settings")]      
    [SerializeField] private List<AgentWeapon> _agentWeaponsSlots = new List<AgentWeapon>();    
    [SerializeField] private Transform[] _gunPointTransforms; 
     private Transform _assignLeftHandCurrentWeapon;
     private int _currentIndex;
     [SerializeField] private AgentWeapon _currentWeapon;
     
    [Header("Actual Weapon Type")]
    [SerializeField] private WeaponType _actualWeaponType;
    
    [Header("Left hand IK")] 
    [SerializeField] private TwoBoneIKConstraint _leftHandIK;
    [SerializeField] private Transform _leftHandIK_Target;
    [SerializeField] private float _leftHandIKWeightIncreaseRate;
     private bool _shouldIncrease_LeftHandIKWeight;
     private WeaponAnimations _weaponAnimations;      
    
    [Header("Rig")]
    [SerializeField] private Rig _rig;
    [SerializeField] private float _rigWeightIncreaseRate;
     private bool _shouldIncrease_RigWeight;
     private bool _isGrabbingWeapon;
     
     [Header("Ammo Settings")]
     [SerializeField] private GameObject _bulletPrefab;
     [SerializeField] private float _bulletSpeed;

    private void Awake()
    {   
        _agent = GetComponentInParent<Agent>();          
        _rig ??= FindAnyObjectByType<Rig>();  
        _weaponAnimations = new WeaponAnimations();
    }
         
    private void Start()
    {   
        AssignDefaultWeapon();
        _agent.AgentInputReader.NotifyCanShoot += WeaponShoot;
        _agent.AgentInputReader.NotifyWeaponSwitch += SwitchOffWeapons;
        _agent.AgentInputReader.NotifyMainWeaponSwitch += OnButtonPressed;
        _agent.AgentInputReader.NotifySecondaryWeaponSwitch += OnButtonPressed;
        _agent.AgentInputReader.NotifyMeleeWeaponSwitch += OnButtonPressed;
        _agent.AgentInputReader.NotifyWeaponReload += OnWeaponReload;    
        _agent.AgentInputReader.NotifyWhenWeaponDropped += DropWeapon;    
    }

    private void OnWeaponReload()
    {
        if (_isGrabbingWeapon) return;
        _agent.AgentAnimator.Animator.SetTrigger(_weaponAnimations.Reload);
        ReduceRigWeight();
    }

    private void ReduceRigWeight()
    {
        _rig.weight = 0.15f;
    }

    public void MaximizeRigWeight() => _shouldIncrease_RigWeight = true; 
    public void MaximizeLeftHandWeight() => _shouldIncrease_LeftHandIKWeight = true;       

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

    private void Update()
    {
        ControlAnimationRig();

        _agent.AgentAim.UpdateAimLaser(_gunPointTransforms[_currentIndex], BulletDirection());
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

    private void PlayWeaponGrabAnimation(GrabType grabType)
    {
        _leftHandIK.weight = 0;
        ReduceRigWeight();
        _agent.AgentAnimator.Animator.SetFloat(_weaponAnimations.WeaponGrabType, ((float)grabType));
        _agent.AgentAnimator.Animator.SetTrigger(_weaponAnimations.WeaponGrab);

        SetBusyGrabbingWeaponTo(true);
    }

    public void SetBusyGrabbingWeaponTo(bool isBusy)
    {
        _isGrabbingWeapon = isBusy;
        _agent.AgentAnimator.Animator.SetBool(_weaponAnimations.BusyGrabbingWeapon, _isGrabbingWeapon);
    }

    private void AssignDefaultWeapon()
    {
        foreach (var weapon in _agentWeaponsSlots)
        {
            weapon.gameObject.SetActive(weapon.WeaponConfigConfiguration.WeaponType == _actualWeaponType);
            weapon.WeaponConfigConfiguration.CurrentAmmo = weapon.WeaponConfigConfiguration.MaxWeaponAmmo;
            AttachLeftHand(weapon.transform);
            _currentWeapon = weapon;
        }   
        SwitchAnimationLayer(_agentWeaponsSlots[_currentIndex].WeaponConfigConfiguration.AnimationLayer);
    }

    private void SwitchOffWeapons()
    {
        _agentWeaponsSlots[_currentIndex].gameObject.SetActive(false);     
        _currentIndex = (_currentIndex + 1) % _agentWeaponsSlots.Count;
        _agentWeaponsSlots[_currentIndex].gameObject.SetActive(true);
        _actualWeaponType = _agentWeaponsSlots[_currentIndex].WeaponConfigConfiguration.WeaponType;
        AttachLeftHand(_agentWeaponsSlots[_currentIndex].gameObject.transform);
        SwitchAnimationLayer(_agentWeaponsSlots[_currentIndex].WeaponConfigConfiguration.AnimationLayer);
        PlayWeaponGrabAnimation(_agentWeaponsSlots[_currentIndex].WeaponConfigConfiguration.GrabType);
        _currentWeapon = _agentWeaponsSlots[_currentIndex];
    }
    
    private void OnButtonPressed()
    {  
        bool weaponFound = false;   
        
        foreach (var weapon in _agentWeaponsSlots)
        {   
            if (weapon.WeaponConfigConfiguration.WeaponInputSlot == _agent.AgentInputReader.WeaponSlotLocation)
            {
                weaponFound = true;      
               
                _actualWeaponType = weapon.WeaponConfigConfiguration.WeaponType;
                _currentWeapon = weapon;          
               
                AttachLeftHand(weapon.transform);
                SwitchAnimationLayer(weapon.WeaponConfigConfiguration.AnimationLayer);
                PlayWeaponGrabAnimation(weapon.WeaponConfigConfiguration.GrabType);
                _currentIndex = weapon.WeaponConfigConfiguration.WeaponInputSlot;        
               
                weapon.gameObject.SetActive(true);
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
            _currentIndex = _currentWeapon.WeaponConfigConfiguration.WeaponInputSlot;
        }                          
        Debug.Log("Selected weapon not found in the list, keeping current weapon.");
    }

    private void AttachLeftHand(Transform weaponTransform)
    {
        _assignLeftHandCurrentWeapon = weaponTransform;
        
        Transform targetTransform = _assignLeftHandCurrentWeapon.GetComponentInChildren<LeftHandTargetTransform>().transform;
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
        
        Vector3 direction = (aim.position - _gunPointTransforms[_currentIndex].position).normalized;

        if (_agent.AgentAim.CanAimPrecisely() == false && 
            _agent.AgentAim.Target(_agent.AgentAim.GetMouseHitInfo(Camera.main, 
                _agent.AgentInputReader.AimInputValue, _agent.AgentMovement.AimLayerMask)) == null)  
         direction.y = 0;           
        //_gunPoint.LookAt(_aim);        TODO: find a better place for it.
        
        return direction;
    }

    private void WeaponShoot()
    {        
        var getCurrentAmmo = _currentWeapon.WeaponConfigConfiguration.CurrentAmmo;
            
        if (_currentWeapon != null && getCurrentAmmo > 0)
        {
            _currentWeapon.WeaponConfigConfiguration.CurrentAmmo--;     
            Debug.Log("Weapon is: " + _currentWeapon.gameObject.name + " " +
            _gunPointTransforms[_currentIndex].gameObject.name + " " + _currentIndex);
            
            
            GameObject newBullet = Instantiate(_bulletPrefab, _gunPointTransforms[_currentIndex].position,
                Quaternion.LookRotation(_gunPointTransforms[_currentIndex].forward));
        
            Rigidbody rbNewBullet = newBullet.GetComponent<Rigidbody>();
            rbNewBullet.mass = 5f / _bulletSpeed;
            rbNewBullet.linearVelocity = BulletDirection() * _bulletSpeed;      
        
            Destroy(newBullet, 3f);
            TriggerShootAnimation();
        }
        else  
            DoSomethingAndRefactorThis();  
    }

    private static void DoSomethingAndRefactorThis() => Debug.Log("NEED MORE AMMO");  
    private void TriggerShootAnimation() => _agent.AgentAnimator.Animator.SetTrigger(_weaponAnimations.Fire);

    private void DropWeapon()
    {
        if (_agentWeaponsSlots.Count <= 1) return;
        
        _agentWeaponsSlots.Remove(_currentWeapon);
        _currentWeapon.gameObject.SetActive(false);
        _currentWeapon = _agentWeaponsSlots[0];
        _actualWeaponType = _currentWeapon.WeaponConfigConfiguration.WeaponType;
        _currentWeapon.gameObject.SetActive(true);
        _currentIndex = _currentWeapon.WeaponConfigConfiguration.WeaponInputSlot;
    }
 }
}     