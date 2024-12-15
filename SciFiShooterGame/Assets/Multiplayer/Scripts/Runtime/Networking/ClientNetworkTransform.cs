using Unity.Netcode.Components;
using UnityEngine;

namespace Multiplayer.Runtime.Networking
{
    [DisallowMultipleComponent]
    public class ClientNetworkTransform : NetworkTransform
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            CanCommitToTransform = IsOwner;
        }
        
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}
