using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer.Runtime
{
    public class JoinServer : MonoBehaviour
    {
        [SerializeField] private Button _startServerButton;
        [SerializeField] private Button _joinServerButton;
        
        private void Awake()
        {
            _joinServerButton.onClick.AddListener(Connect);
            _startServerButton.onClick.AddListener(Host);
        }

        private void Connect()
        {
            NetworkManager.Singleton.StartClient();
        }
        
        private void Host()
        {
            NetworkManager.Singleton.StartHost();
        }

        private void OnDestroy()
        {
            _joinServerButton.onClick.RemoveAllListeners();
            _startServerButton.onClick.RemoveAllListeners();
        }
    }
}
