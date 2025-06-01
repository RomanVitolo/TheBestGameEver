using System.Collections;
using Core.Scripts.Runtime.Utilities;
using UnityEngine;

namespace Core.Scripts.Runtime.Ammo
{
    public class BulletImpact : MonoBehaviour
    {
        private void OnEnable()
        {
            StartCoroutine(nameof(ReturnToPool));
        }

        IEnumerator ReturnToPool()
        {
            yield return new WaitForSeconds(0.3f);
            GlobalPoolContainer.Instance.BulletPoolImpact.ReturnObject(this);
        }
    }
}