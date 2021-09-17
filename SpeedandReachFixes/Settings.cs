using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using Noggog;
using SpeedandReachFixes.GMST;
using SpeedandReachFixes.SettingObjects;
using System.Collections.Generic;

namespace SpeedandReachFixes
{
    /// <summary>
    /// Contains all settings used throughout the patcher.
    /// This is the object passed to SetAutoGeneratedSettings.
    /// </summary>
    public class Settings
    {
        [MaintainOrder]

        // Game Settings
        [SettingName("Game Setting Changes")]
        [Tooltip("Don't change these unless you know what you're doing!")]
        public GameSettings GameSettings = new();

        // Race Attack Angle
        [SettingName("Experimental Race Strike Angle Modifier")]
        [Tooltip("Changes the angle at which an NPC can be hit by an attack. This value is added to the current value for all attacks and races. Set to 0 to disable, or negative to subtract.")]
        public float AttackStrikeAngleModifier = 7F;

        // Global toggles
        [SettingName("Enable Reach Changes")]
        [Tooltip("Global toggle for reach changes, if unchecked, all reach changes will be disabled.")]
        public bool EnableReachChangesGlobal = true;

        [SettingName("Enable Speed Changes")]
        [Tooltip("Global toggle for speed changes, if unchecked, all speed changes will be disabled.")]
        public bool EnableSpeedChangesGlobal = true;

        // List of WeaponStats objects, each relating to a different weapon keyword.
        [SettingName("Stat Categories")]
        [Tooltip("Change the stats of each weapon type.")]
        public List<WeaponStats> WeaponStats { get; set; } = new()
        {
            new WeaponStats(1, false, Skyrim.Keyword.WeapTypeBattleaxe, 0.666667F, 0.8275F),
            new WeaponStats(1, false, Skyrim.Keyword.WeapTypeDagger, 1.35F, 0.533F),
            new WeaponStats(1, false, Skyrim.Keyword.WeapTypeGreatsword, 0.85F, 0.88F),
            new WeaponStats(1, false, Skyrim.Keyword.WeapTypeMace, 0.9F, 0.75F),
            new WeaponStats(1, false, Skyrim.Keyword.WeapTypeSword, 1.1F, 0.83F),
            new WeaponStats(1, false, Skyrim.Keyword.WeapTypeWarAxe, 1F, 0.6F),
            new WeaponStats(1, false, Skyrim.Keyword.WeapTypeWarhammer, 0.6F, 0.8F),
            new WeaponStats(1, false, Skyrim.Keyword.WeapTypeBow),
            new WeaponStats(2, true, NewArmoury.Keyword.WeapTypeCestus),
            new WeaponStats(2, true, NewArmoury.Keyword.WeapTypeClaw, Constants.NullFloat, 0.41F),
            new WeaponStats(2, true, NewArmoury.Keyword.WeapTypeHalberd, Constants.NullFloat, 0.58F),
            new WeaponStats(2, true, NewArmoury.Keyword.WeapTypePike, Constants.NullFloat, 0.2F),
            new WeaponStats(2, true, NewArmoury.Keyword.WeapTypeQtrStaff, Constants.NullFloat, 0.25F),
            new WeaponStats(2, true, NewArmoury.Keyword.WeapTypeRapier, Constants.NullFloat, 0.2F),
            new WeaponStats(2, true, NewArmoury.Keyword.WeapTypeSpear),
            new WeaponStats(2, true, NewArmoury.Keyword.WeapTypeWhip, Constants.NullFloat, 0.5F),
            // TODO: Find appropriate values for NWTA & WotM unarmed weapons
            new WeaponStats(2, true, NWTA.Keyword.WeapTypeKatana),
            new WeaponStats(2, true, NWTA.Keyword.WeapTypeCurvedSword),
            new WeaponStats(2, true, WayOfTheMonk.Keyword.WeapTypeUnarmed)
        };

        // Modify an attack angle by adding the current AttackStrikeAngleModifier value to it.
        public float GetModifiedStrikeAngle(float current)
        {
            return current += AttackStrikeAngleModifier;
        }

        // Private function that retrieves the highest priority applicable WeaponStats instance from the current settings
        private WeaponStats GetHighestPriorityStats(Weapon weapon)
        {
            WeaponStats highestStats = new();
            var highest = 0; // the priority level associated with the current highestStats
            foreach (var stats in WeaponStats)
            {
                var priority = stats.GetPriority(weapon.Keywords);
                if (priority <= highest || stats.ShouldSkip())
                    continue;
                highestStats = stats;
                highest = priority;
            }

            return highestStats;
        }

        // Applies the current weapon stats configuration to a given weapon
        public bool ApplyChangesToWeapon(Weapon weapon)
        {
            if (weapon.Data == null || weapon.EditorID == null)
                return false; // return early if the given weapon is invalid

            var stats = GetHighestPriorityStats(weapon);

            if (stats.ShouldSkip())
                return false;

            // Apply reach changes if they are enabled globally
            bool changedReach = false;
            if (EnableReachChangesGlobal)
                weapon.Data.Reach = stats.GetReach(weapon.Data.Reach, out changedReach);

            // Apply speed changes if they are enabled globally
            bool changedSpeed = false;
            if (EnableSpeedChangesGlobal)
                weapon.Data.Speed = stats.GetSpeed(weapon.Data.Speed, out changedSpeed);

            // Revert any reach changes to giant clubs as they may cause issues with the AI
            if (weapon.EditorID.ContainsInsensitive("GiantClub"))
                weapon.Data.Reach = 1.3F;
            return changedReach || changedSpeed; // returns true if either the speed or the reach values were changed.
        }
    }
}
