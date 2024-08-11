using System;
using Core.Scripts.Runtime.Agent;
using UnityEngine;

public class AgentWeaponMotor : MonoBehaviour
{
    [SerializeField] private WeaponType _weaponSlot;
    
    [SerializeField] private Agent _agent;
    private AgentWeapon[] _agentWeaponsSlots;

    private void OnEnable()
    {
        _agent = GetComponentInParent<Agent>();
        _agentWeaponsSlots = GetComponentsInChildren<AgentWeapon>();
        AssignDefaultWeapon();
       _agent.AgentInputReader.NotifyWeaponSwitch += SwitchOffWeapons;
       _agent.AgentInputReader.NotifyMainWeaponSwitch += OnButtonPressed;
       _agent.AgentInputReader.NotifySecondaryWeaponSwitch += OnButtonPressed;
       _agent.AgentInputReader.NotifyMeleeWeaponSwitch += OnButtonPressed;
    }

    private void OnDisable()
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
        }          
    }

    public void SwitchOffWeapons()
    {
        foreach (var t in _agentWeaponsSlots)
        {
            t.gameObject.SetActive(!t.gameObject.activeSelf);
        }
    }
    
    private void OnButtonPressed()
    {
        foreach (var weapon in _agentWeaponsSlots)
        {
            weapon.gameObject.SetActive(weapon.WeaponConfigConfiguration.WeaponInputSlot ==
                                        _agent.AgentInputReader.WeaponSlotLocation);
        }
    }
    

    private void TriggerShootAnimation()
    {
        if(_agent.AgentInputReader.CanShoot)
            _agent.AgentAnimator.Animator.SetTrigger(_agent.AgentAnimator.Fire);
    }  
}

