using Core.Scripts.Runtime.Agents;
using Core.Scripts.Runtime.Utilities;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Serialization;

namespace Core.Scripts.Runtime.Weapons
{
    public class WeaponAnimations : MonoBehaviour
    {
        [Header("Rig")]
        [SerializeField] private Rig _rig;
        [SerializeField] private float _rigWeightIncreaseRate;
        private bool _shouldIncrease_RigWeight;

        [Header("Left hand IK")]
        [SerializeField] private Transform _leftHandIK_Target;
        [SerializeField] private TwoBoneIKConstraint _leftHandIK;
        [SerializeField] private float _leftHandIKWeightIncreaseRate;
        private Transform _assignLeftHandCurrentWeapon;
        
        private bool _shouldIncrease_LeftHandIKWeight;
        private WeaponAnimationsKeys _weaponAnimationsKeys;
        
        private Agent _agent;

        private void Awake()
        {
            _agent = GetComponentInParent<Agent>();
            _rig ??= FindAnyObjectByType<Rig>();
            _weaponAnimationsKeys = new WeaponAnimationsKeys();
        }

        private void Update()
        {
            ControlAnimationRig();
        }
        
        public void PlayWeaponEquipAnimation(WeaponEnums.EquipType equipType, float currentWeapon)
        {
            float equipmentSpeed = currentWeapon;

            _leftHandIK.weight = 0;
            ReduceRigWeight();
            _agent.AgentAnimator.Animator.SetTrigger(_weaponAnimationsKeys.EquipWeapon);
            _agent.AgentAnimator.Animator.SetFloat(_weaponAnimationsKeys.EquipType, ((float)equipType));
            _agent.AgentAnimator.Animator.SetFloat(_weaponAnimationsKeys.EquipSpeed, equipmentSpeed);
        }
        
        private void ReduceRigWeight() => _rig.weight = 0.15f;
        public void MaximizeRigWeight() => _shouldIncrease_RigWeight = true;
        public void MaximizeLeftHandWeight() => _shouldIncrease_LeftHandIKWeight = true;
        
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
        
        public void AttachLeftHand(Transform weaponTransform)
        {
            _assignLeftHandCurrentWeapon = weaponTransform;

            Transform targetTransform =
                _assignLeftHandCurrentWeapon.GetComponentInChildren<LeftHandTargetTransform>().transform;
            _leftHandIK_Target.localPosition = targetTransform.localPosition;
            _leftHandIK_Target.localRotation = targetTransform.localRotation;
        }

        public void WeaponReloadAnimation(float weaponReloadSpeed)
        {
            float reloadSpeed = weaponReloadSpeed;
            _agent.AgentAnimator.Animator.SetTrigger(_weaponAnimationsKeys.Reload);
            _agent.AgentAnimator.Animator.SetFloat(_weaponAnimationsKeys.WeaponReloadSpeed, reloadSpeed);
            ReduceRigWeight();
        }
        
        public void SwitchAnimationLayer(int layerIndex)
        {
            for (int i = 0; i < _agent.AgentAnimator.Animator.layerCount; i++)
                _agent.AgentAnimator.Animator.SetLayerWeight(i, 0);

            _agent.AgentAnimator.Animator.SetLayerWeight(layerIndex, 1);
        }
        
        public void TriggerShootAnimation() => PlayFireAnimation();
        private void PlayFireAnimation() => _agent.AgentAnimator.Animator.SetTrigger(_weaponAnimationsKeys.Fire);
    }
}