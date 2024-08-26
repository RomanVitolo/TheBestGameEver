using System;
using UnityEngine;

namespace Core.Scripts.Runtime.Agent
{
    public class AgentAim : MonoBehaviour
    {
        [SerializeField] private Transform _aimPosition;
        
        public void UpdateAgentMousePosition(Vector3 mousePosition)
        {
            _aimPosition.position = new Vector3(mousePosition.x, transform.position.y + 1, mousePosition.z);
        }

        public Vector3 GetMousePosition(Camera agentCamera, Vector2 aimInput, LayerMask aimLayerMask)
        {
            Ray ray = agentCamera.ScreenPointToRay(aimInput);

            return Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, aimLayerMask) ? 
                hitInfo.point : Vector3.zero;
        }                                               
    }
}