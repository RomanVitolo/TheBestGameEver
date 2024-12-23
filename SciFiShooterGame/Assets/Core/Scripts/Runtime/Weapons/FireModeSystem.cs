using System.Collections.Generic;
using Core.Scripts.Runtime.Agents;
using Core.Scripts.Runtime.Weapons.Interfaces;

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

        public void HandleFireMode(IAgentWeaponMotor weaponMotor)
        {
            if (!weaponMotor.IWeapon.WeaponDataConfiguration.HasThisWeaponFireMode())
                return;

            if (_fireModeHandlers.TryGetValue(weaponMotor.IWeapon.WeaponDataConfiguration.FireMode,
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