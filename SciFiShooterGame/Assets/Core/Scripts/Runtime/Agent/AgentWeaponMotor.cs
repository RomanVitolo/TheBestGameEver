using System;
using Core.Scripts.Runtime.Agent;
using Core.Scripts.Runtime.Utilities;
using UnityEngine;

public class AgentWeaponMotor : MonoBehaviour
{
    [Header("Left hand IK")] 
    [SerializeField] private Transform _leftHand;
    
    [SerializeField] private WeaponType _weaponSlot;
    
    [SerializeField] private Agent _agent;
    private AgentWeapon[] _agentWeaponsSlots;
    private int _currentIndex = 0;

    private Transform _currentWeapon;

    private void Awake()
    {
        _agent = GetComponentInParent<Agent>();
        _agentWeaponsSlots = GetComponentsInChildren<AgentWeapon>();
        AssignDefaultWeapon();      
    }
    
    private void Start()
    {   
        _agent.AgentInputReader.NotifyWeaponSwitch += SwitchOffWeapons;
        _agent.AgentInputReader.NotifyMainWeaponSwitch += OnButtonPressed;
        _agent.AgentInputReader.NotifySecondaryWeaponSwitch += OnButtonPressed;
        _agent.AgentInputReader.NotifyMeleeWeaponSwitch += OnButtonPressed;
    }         

    private void OnDestroy()
    {
        _agent.AgentInputReader.NotifyWeaponSwitch -= SwitchOffWeapons;
        _agent.AgentInputReader.NotifyMainWeaponSwitch -= OnButtonPressed;
        _agent.AgentInputReader.NotifySecondaryWeaponSwitch -= OnButtonPressed;
        _agent.AgentInputReader.NotifyMeleeWeaponSwitch -= OnButtonPressed;
    }

    private void Update()
    {
        TriggerShootAnimation(); 
    }

    private void AssignDefaultWeapon()
    {
        foreach (var weapon in _agentWeaponsSlots)
        {
            weapon.gameObject.SetActive(weapon.WeaponConfigConfiguration.WeaponType == _weaponSlot);    
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
            }
            else   
                weapon.gameObject.SetActive(false);      
        }    
        
    }

    private void AttachLeftHand(Transform weaponTransform)
    {
        _currentWeapon = weaponTransform;
        
        Transform targetTransform = _currentWeapon.GetComponentInChildren<LeftHandTargetTransform>().transform;
        _leftHand.localPosition = targetTransform.localPosition;
        _leftHand.localRotation = targetTransform.localRotation;
    }

    private void SwitchAnimationLayer(int layerIndex)
    {
        for (int i = 0; i < _agent.AgentAnimator.Animator.layerCount; i++)
        {
            _agent.AgentAnimator.Animator.SetLayerWeight(i, 0);
        }
        
        _agent.AgentAnimator.Animator.SetLayerWeight(layerIndex, 1);
    }

    private void TriggerShootAnimation()
    {
        if(_agent.AgentInputReader.CanShoot)
            _agent.AgentAnimator.Animator.SetTrigger(_agent.AgentAnimator.Fire);
    }  
}

