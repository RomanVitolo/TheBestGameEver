using System.Collections.Generic;
using Core.Scripts.Runtime.Agents;

namespace Core.Scripts.Runtime.Weapons
{
    public class FireModeSystem
    {
        private readonly Dictionary<WeaponEnums.FireModeType, IFireModeHandler> _fireModeHandlers = new()
        {
            { WeaponEnums.FireModeType.Burst, new BurstFireModeHandler() },
            { WeaponEnums.FireModeType.Auto, new AutoFireModeHandler() },
            { WeaponEnums.FireModeType.MultiShoot, new MultiShootFireModeHandler() }
        };

        public void HandleFireMode(AgentWeaponMotor weaponMotor)
        {
            if (!weaponMotor.CurrentWeapon().WeaponDataConfiguration.HasThisWeaponFireMode())
                return;

            if (_fireModeHandlers.TryGetValue(weaponMotor.CurrentWeapon().WeaponDataConfiguration.FireMode,
                    out var handler))
            {
                handler.HandleFireMode(weaponMotor);
            }
            else
            {
                new DefaultFireModeHandler().HandleFireMode(weaponMotor);
            }
        }
    }
}