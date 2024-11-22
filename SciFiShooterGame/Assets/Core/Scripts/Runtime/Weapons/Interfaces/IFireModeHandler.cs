using Core.Scripts.Runtime.Agents;

namespace Core.Scripts.Runtime.Weapons
{
    public interface IFireModeHandler
     {
         void HandleFireMode(AgentWeaponMotor weaponMotor);
     }
}