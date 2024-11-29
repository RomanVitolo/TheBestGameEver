using UnityEngine;

namespace Core.Scripts.Runtime.Items
{
    internal abstract class BasePickUpComponents : MonoBehaviour
    {
        protected virtual void DestroyComponent()
        {
            Destroy(this.gameObject);
        }
        
    }
}