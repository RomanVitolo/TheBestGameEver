using Core.Scripts.Runtime.AI.Entities.StateMachine;
using Core.Scripts.Runtime.Ammo;
using Core.Scripts.Runtime.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace Core.Scripts.Runtime.Agents
{
    /// <summary>
    /// Replicates a shot. Exactly one bullet in the whole session is authoritative — the one living on the
    /// server — and it is the only one allowed to deal damage. Every other peer spawns a cosmetic copy so
    /// the shot is visible, including on the shooter, which spawns its own immediately for zero-lag feedback.
    ///
    /// The direction is computed once by the shooter and sent as-is. WeaponRuntime.ApplyRecoil draws from
    /// Random, so letting each peer derive its own direction would fan the same shot in three directions.
    /// </summary>
    public struct BulletFireParams : INetworkSerializable
    {
        public Vector3 Origin;
        public Vector3 Direction;
        public float FlyDistance;
        public float ImpactForce;
        public float BulletSpeed;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Origin);
            serializer.SerializeValue(ref Direction);
            serializer.SerializeValue(ref FlyDistance);
            serializer.SerializeValue(ref ImpactForce);
            serializer.SerializeValue(ref BulletSpeed);
        }
    }

    [RequireComponent(typeof(Agent))]
    public class AgentWeaponFire : NetworkBehaviour
    {
        [SerializeField] private int _bulletDamage = 1;

        public int BulletDamage => _bulletDamage;

        /// <summary>Called on the owning client when its weapon fires.</summary>
        public void Fire(BulletFireParams fireParams)
        {
            if (!IsOwner) return;

            if (IsServer)
            {
                // Host: the shooter's own bullet is the authoritative one, so it must not be spawned twice.
                SpawnBullet(fireParams, isAuthoritative: true);
                ResolveServerSideHitReactions(fireParams);
                SpawnCosmeticBulletRpc(fireParams);
                return;
            }

            SpawnBullet(fireParams, isAuthoritative: false);
            FireRpc(fireParams);
        }

        [Rpc(SendTo.Server)]
        private void FireRpc(BulletFireParams fireParams)
        {
            SpawnBullet(fireParams, isAuthoritative: true);
            ResolveServerSideHitReactions(fireParams);
            SpawnCosmeticBulletRpc(fireParams);
        }

        [Rpc(SendTo.NotServer)]
        private void SpawnCosmeticBulletRpc(BulletFireParams fireParams)
        {
            // The shooter already spawned its own copy the instant it pulled the trigger.
            if (IsOwner) return;

            SpawnBullet(fireParams, isAuthoritative: false);
        }

        private static void SpawnBullet(BulletFireParams fireParams, bool isAuthoritative)
        {
            Bullet newBullet = GlobalPoolContainer.Instance.BulletPool.GetObject();

            newBullet.transform.SetPositionAndRotation(fireParams.Origin,
                Quaternion.LookRotation(fireParams.Direction));

            newBullet.BulletSetup(fireParams.FlyDistance, fireParams.ImpactForce, isAuthoritative);

            Rigidbody bulletRigidbody = newBullet.GetComponent<Rigidbody>();
            bulletRigidbody.mass = 5f / fireParams.BulletSpeed;
            bulletRigidbody.linearVelocity = fireParams.Direction * fireParams.BulletSpeed;
        }

        // The dodge roll changes AI state, so it may only be decided by the server. It used to be raycast on
        // whichever client pulled the trigger, which would have desynced that enemy for everyone.
        private void ResolveServerSideHitReactions(BulletFireParams fireParams)
        {
            if (!Physics.Raycast(fireParams.Origin, fireParams.Direction, out RaycastHit hitInfo, Mathf.Infinity))
                return;

            Entity_Melee entityMelee = hitInfo.collider.gameObject.GetComponentInParent<Entity_Melee>();

            if (entityMelee != null)
                entityMelee.ActivateDodgeRoll();
        }
    }
}
