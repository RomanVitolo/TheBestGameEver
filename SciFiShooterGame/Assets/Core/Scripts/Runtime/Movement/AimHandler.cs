using Core.Scripts.Runtime.Agents;
using Core.Scripts.Runtime.Movement.Interfaces;
using UnityEngine;

namespace Core.Scripts.Runtime.Movement
{
    public class AimHandler : IAimHandler
    {
        private readonly Agent _agent;

        public AimHandler(Agent agent)
        {
            _agent = agent;
        }

        public void HandleAim(Vector3 movementValue, RaycastHit mouseHitInfo)
        {
            Vector3 mouseHitPoint = mouseHitInfo.point;
            
            _agent.AgentAim.UpdateAgentCameraPosition(mouseHitPoint, movementValue);
            _agent.AgentAim.UpdateAgentAimPosition(mouseHitPoint, mouseHitInfo);
        }
    }
}