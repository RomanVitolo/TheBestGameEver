using System.Collections.Generic;
using Core.Scripts.Runtime.Agents;
using UnityEngine;

namespace Core.Scripts.Runtime.Networking
{
    /// <summary>
    /// Every spawned <see cref="Agent"/>, on every peer. Enemies used to grab their target with
    /// FindAnyObjectByType&lt;Agent&gt;() in Awake, which returns null now that players are spawned on connect
    /// rather than placed in the scene — and it could only ever find one of them.
    /// </summary>
    public static class AgentRegistry
    {
        private static readonly List<Agent> _agents = new();

        public static IReadOnlyList<Agent> Agents => _agents;

        // Statics survive play sessions when Enter Play Mode Options disables domain reload.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset() => _agents.Clear();

        public static void Register(Agent agent)
        {
            if (agent != null && !_agents.Contains(agent))
                _agents.Add(agent);
        }

        public static void Unregister(Agent agent) => _agents.Remove(agent);

        public static Agent GetNearestAlive(Vector3 position)
        {
            Agent nearest = null;
            float nearestSqr = float.MaxValue;

            foreach (Agent agent in _agents)
            {
                if (agent == null || agent.Health == null || !agent.Health.IsAlive) continue;

                float sqr = (agent.transform.position - position).sqrMagnitude;
                if (sqr >= nearestSqr) continue;

                nearestSqr = sqr;
                nearest = agent;
            }

            return nearest;
        }
    }
}
