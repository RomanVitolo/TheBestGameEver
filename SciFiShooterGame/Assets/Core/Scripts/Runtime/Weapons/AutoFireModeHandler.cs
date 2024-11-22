using Core.Scripts.Runtime.Agents;

namespace Core.Scripts.Runtime.Weapons
{
    public class AutoFireModeHandler : IFireModeHandler
    {
        public void HandleFireMode(AgentWeaponMotor weaponMotor)
        {
            weaponMotor.FireSingleBullet();
        }
    }
}