using System;
using System.Collections;
using System.Collections.Generic;
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

    private void TriggerShootAnimation()
    {
        if(_agent.AgentMovement.InputReader.CanShoot)
            _agent.AgentAnimator.Animator.SetTrigger(_agent.AgentAnimator.Fire);
    }  
}

