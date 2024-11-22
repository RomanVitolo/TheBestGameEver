using Core.Scripts.Runtime.Agents;

namespace Core.Scripts.Runtime.Weapons
{
    public class DefaultFireModeHandler : IFireModeHandler
    {
        public void HandleFireMode(AgentWeaponMotor weaponMotor)
        {
            weaponMotor.FireSingleBullet();
        }
    }
}