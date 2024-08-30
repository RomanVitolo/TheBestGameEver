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
    [SerializeField] private AgentWeapon[] _agentWeaponsSlots;    
    [SerializeField] private WeaponType _initialWeaponType; 
     private WeaponAnimations _weaponAnimations;      
     private Transform _currentWeapon;
     private int _currentIndex;
    
    [Header("Left hand IK")] 
    [SerializeField] private TwoBoneIKConstraint _leftHandIK;
    [SerializeField] private Transform _leftHandIK_Target;
    [SerializeField] private float _leftHandIKWeightIncreaseRate;
     private bool _shouldIncrease_LeftHandIKWeight;
    
    [Header("Rig")]
    [SerializeField] private Rig _rig;
    [SerializeField] private float _rigWeightIncreaseRate;
     private bool _shouldIncrease_RigWeight;
     private bool _isGrabbingWeapon;
     
     [Header("Ammo Settings")]
     [SerializeField] private GameObject _bulletPrefab;
     [SerializeField] private float _bulletSpeed;
     [SerializeField] private Transform _gunPoint;
       
     [SerializeField] private Transform _aim;


    private void Awake()
    {
        _weaponAnimations = new WeaponAnimations();
        _agent = GetComponentInParent<Agent>();
        _agentWeaponsSlots = GetComponentsInChildren<AgentWeapon>();
        _rig ??= FindAnyObjectByType<Rig>();
        AssignDefaultWeapon();      
    }
    
    private void Start()
    {   
        _agent.AgentInputReader.NotifyCanShoot += WeaponShoot;
        _agent.AgentInputReader.NotifyWeaponSwitch += SwitchOffWeapons;
        _agent.AgentInputReader.NotifyMainWeaponSwitch += OnButtonPressed;
        _agent.AgentInputReader.NotifySecondaryWeaponSwitch += OnButtonPressed;
        _agent.AgentInputReader.NotifyMeleeWeaponSwitch += OnButtonPressed;
        _agent.AgentInputReader.NotifyWeaponReload += OnWeaponReload;
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
    }

    private void Update()
    {
        if (_shouldIncrease_RigWeight)
        {
            _rig.weight += _rigWeightIncreaseRate * Time.deltaTime;

            if (_rig.weight >= 1)
                _shouldIncrease_RigWeight = false;
        }

        if (_shouldIncrease_LeftHandIKWeight)
        {
           _leftHandIK.weight += _leftHandIKWeightIncreaseRate * Time.deltaTime;

           if (_leftHandIK.weight >= 1)
           {
               _shouldIncrease_LeftHandIKWeight = false;
           }
        }

        _agent.AgentAim.UpdateAimLaser(_gunPoint, BulletDirection());
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
            weapon.gameObject.SetActive(weapon.WeaponConfigConfiguration.WeaponType == _initialWeaponType);    
            AttachLeftHand(weapon.transform); 
        }   
        SwitchAnimationLayer(_agentWeaponsSlots[_currentIndex].WeaponConfigConfiguration.AnimLayer);
    }

    private void SwitchOffWeapons()
    {
        _agentWeaponsSlots[_currentIndex].gameObject.SetActive(false);     
        _currentIndex = (_currentIndex + 1) % _agentWeaponsSlots.Length;
        _agentWeaponsSlots[_currentIndex].gameObject.SetActive(true);    
        AttachLeftHand(_agentWeaponsSlots[_currentIndex].gameObject.transform);
        SwitchAnimationLayer(_agentWeaponsSlots[_currentIndex].WeaponConfigConfiguration.AnimLayer);
        PlayWeaponGrabAnimation(_agentWeaponsSlots[_currentIndex].WeaponConfigConfiguration.GrabType);
    }
    
    private void OnButtonPressed()
    {
        foreach (var weapon in _agentWeaponsSlots)
        {
            if (weapon.WeaponConfigConfiguration.WeaponInputSlot ==
                _agent.AgentInputReader.WeaponSlotLocation)
            {
                weapon.gameObject.SetActive(true);
                AttachLeftHand(weapon.transform); 
                SwitchAnimationLayer(weapon.WeaponConfigConfiguration.AnimLayer);
                PlayWeaponGrabAnimation(weapon.WeaponConfigConfiguration.GrabType);
            }
            else   
                weapon.gameObject.SetActive(false);      
        }                              
    }

    private void AttachLeftHand(Transform weaponTransform)
    {
        _currentWeapon = weaponTransform;
        
        Transform targetTransform = _currentWeapon.GetComponentInChildren<LeftHandTargetTransform>().transform;
        _leftHandIK_Target.localPosition = targetTransform.localPosition;
        _leftHandIK_Target.localRotation = targetTransform.localRotation;
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 0; i < _agent.AgentAnimator.Animator.layerCount; i++)     
            _agent.AgentAnimator.Animator.SetLayerWeight(i, 0);   
        
        _agent.AgentAnimator.Animator.SetLayerWeight(layerIndex, 1);
    }

    private void WeaponShoot()
    {   
        GameObject newBullet = Instantiate(_bulletPrefab, _gunPoint.position, Quaternion.LookRotation(_gunPoint.forward));
        
        newBullet.GetComponent<Rigidbody>().linearVelocity = BulletDirection() * _bulletSpeed;
        
        Destroy(newBullet, 3f);
        TriggerShootAnimation();
    }

    public Vector3 BulletDirection()
    {
        Vector3 direction = (_aim.position - _gunPoint.position).normalized;

        if (_agent.AgentAim.CanAimPrecisely() == false && 
            _agent.AgentAim.Target(_agent.AgentAim.GetMouseHitInfo(Camera.main, 
                _agent.AgentInputReader.AimInputValue, _agent.AgentMovement.AimLayerMask)) == null)  
         direction.y = 0;     
       
        _gunPoint.LookAt(_aim);
        
        return direction;
    }

    public Transform GunPoint() => _gunPoint;
    private void TriggerShootAnimation() => _agent.AgentAnimator.Animator.SetTrigger(_weaponAnimations.Fire);
 }
}     