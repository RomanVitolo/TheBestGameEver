using Core.Scripts.Runtime.Agents;
using Core.Scripts.Runtime.Weapons.Interfaces;

namespace Core.Scripts.Runtime.Weapons
{
    public class BurstFireModeHandler : IFireModeHandler
    {
        public void HandleFireMode(AgentWeaponMotor weaponMotor)
        {
            weaponMotor.StartCoroutine(weaponMotor.BurstFireMode());
        }
    }
}