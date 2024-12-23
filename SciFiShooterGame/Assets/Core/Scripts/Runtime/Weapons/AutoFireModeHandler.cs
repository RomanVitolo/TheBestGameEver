using Core.Scripts.Runtime.Agents;
using Core.Scripts.Runtime.Weapons.Interfaces;

namespace Core.Scripts.Runtime.Weapons
{
    public class AutoFireModeHandler : IFireModeHandler
    {
        public void HandleFireMode(IAgentWeaponMotor weaponMotor)
        {
            weaponMotor.FireSingleBullet();
        }
    }
}