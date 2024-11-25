using System.Collections;
using UnityEngine;

namespace Core.Scripts.Runtime.Ammo
{
    public class BulletImpact : MonoBehaviour
    {
        private BulletPoolImpact _bulletPoolImpact;
        private void OnEnable()
        {
            _bulletPoolImpact = FindAnyObjectByType<BulletPoolImpact>();
            StartCoroutine(nameof(ReturnToPool));
        }

        IEnumerator ReturnToPool()
        {
            yield return new WaitForSeconds(0.3f);
            _bulletPoolImpact.ReturnObject(this);
        }
    }
}