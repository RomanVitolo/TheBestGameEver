using Core.Scripts.Runtime.Agents;

namespace Core.Scripts.Runtime.Weapons.Interfaces
{
    public interface IFireModeHandler
     {
         void HandleFireMode(IAgentWeaponMotor weaponMotor);
     }
}