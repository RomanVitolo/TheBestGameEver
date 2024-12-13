using System.Collections.Generic;
using System.Linq;
using Core.Scripts.Runtime.Utilities;
using DG.Tweening;
using UnityEngine;

namespace Core.Scripts.Runtime.Items
{
    internal abstract class BasePickUpComponents : MonoBehaviour
    {
        [SerializeField] protected string _itemId;
        [SerializeField] protected Material _componentMaterial;
        [SerializeField] protected float _animationDuration = 2f;
        
        protected INotifyEvent onPickUp;
        protected MeshRenderer itemMeshRenderer;
        
        private Tween rotationTween;

        protected virtual void LoadItemEffect(MeshRenderer meshRenderer, Transform itemTransform)
        {
            itemMeshRenderer = GetComponent<MeshRenderer>();
            itemMeshRenderer.material = _componentMaterial;

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
        
        private INotifyEvent SelectNotifyEventComponent(IEnumerable<INotifyEvent> components)
        {
            return components.FirstOrDefault(component => component.ItemId == _itemId);
        }
        
        protected virtual void PickUpBehaviour(Collider other)
        {
            var notifyEventComponents = other.GetComponentsInChildren<MonoBehaviour>()
                .OfType<INotifyEvent>()
                .ToList();

            if (!notifyEventComponents.Any())
            {
                Debug.Log("No INotifyEvent components found.");
                return;
            }

            onPickUp = SelectNotifyEventComponent(notifyEventComponents);

            if (onPickUp != null) return;
            Debug.Log($"No component with ID '{_itemId}' found.");
            return;
        }
        
        protected void DestroyComponent()
        {
            Destroy(this.gameObject);
        }
    }
}