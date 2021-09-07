using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace SpeedandReachFixes.GMST
{
    /// <summary>
    /// Contains settings related to GMST (Game Setting) records.
    /// </summary>
    public class GameSettings
    {
        [MaintainOrder]
        // Combat reach
        [SettingName("Base Combat Reach Multipliers")]
        [Tooltip("Melee Reach Formula: ( reach = ( fCombatDistance | fCombatBashReach ) * NPCRaceScale * WeaponReach + ( fObjectHitWeaponReach | fObjectHitTwoHandReach | fObjectHitH2HReach ) )")]
        public GameSettingsCombatReach CombatReach = new();
        // Weapon type reach
        [SettingName("Weapon Type Reach Modifiers")]
        [Tooltip("Melee Reach Formula: ( reach = ( fCombatDistance | fCombatBashReach ) * NPCRaceScale * WeaponReach + ( fObjectHitWeaponReach | fObjectHitTwoHandReach | fObjectHitH2HReach ) )")]
        public GameSettingsWeaponTypeReach WeaponTypeReach = new();

        public int AddGameSettingsToPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            var count = 0;
            // add game settings from weapon type reach category
            count += WeaponTypeReach.AddGameSettings(state);

            // add game settings from combat reach category
            count += CombatReach.AddGameSettings(state);
            return count;
        }
    }
}
