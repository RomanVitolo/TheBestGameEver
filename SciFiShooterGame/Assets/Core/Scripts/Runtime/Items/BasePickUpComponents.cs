using System;
using DG.Tweening;
using UnityEngine;

namespace Core.Scripts.Runtime.Items
{
    public abstract class BasePickUpComponents : MonoBehaviour
    {
        [SerializeField] protected Material _componentMaterial;
        [SerializeField] protected  float _animationDuration = 2f;
        
        private Tween rotationTween;

        protected void LoadItemEffect(MeshRenderer renderer, Transform itemTransform)
        {
            renderer = GetComponent<MeshRenderer>();
            renderer.material = _componentMaterial;

            if (itemTransform  != null)
            {
                itemTransform.DORotate(new Vector3(0, 360, 0), _animationDuration, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear) 
                    .SetLoops(-1, LoopType.Restart);
            }
        }

        protected void KillItemEffect(Transform itemTransform)
        {
            if (rotationTween != null && itemTransform == null) return;
            rotationTween.Kill(); 
            rotationTween = null;
        }

        protected virtual void DestroyComponent()
        {
            Destroy(this.gameObject);
        }
        
    }
}