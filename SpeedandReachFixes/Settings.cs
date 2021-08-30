using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using Noggog;

namespace SpeedandReachFixes
{
    /**
     * @struct Constants
     * @brief Contains default value constants used internally.
     */
    internal struct Constants
    {
        public const float NullFloat = -0F; // default value assigned to null stat values
        public const int DefaultPriority = -1; // default priority returned when no matches were found
    }

    /**
     * @class GameSettings
     * @brief Contains settings related to GMST (Game Setting) records.
     */
    public class GameSettings
    {
        [SettingName("Fix Shield Bash Reach")] public bool CombatBashReach = true;

        [SettingName("Fix Combat Distance")] public bool CombatDistance = true;

        [SettingName("Fix Object Reach")] [Tooltip("Changes how far away weapons can hit objects.")] public bool ObjectHitWeaponReach = true;

        [MaintainOrder] [SettingName("[EXPERIMENTAL] Change Swing Angle for all races.")] [Tooltip("Don't change this unless you know what you're doing!")] public bool WeaponSwingAngleChanges = false;
    }

    /**
     * @class WeaponStats
     * @brief Represents the stats associated with a given weapon keyword, and an optional user-customizable matchlist to allow
     */
    public class WeaponStats
    {
        [SettingName("Modifier")] [Tooltip("When checked, adds the specified values rather than overwriting them. Negative values will subtract.")] public bool IsModifier;

        // VARS

        [MaintainOrder] [Tooltip("The keyword attached to this weapon type.")] public FormLink<IKeywordGetter>? Keyword;

        [Tooltip("When multiple weapon types apply to the same category, the highest priority wins.")] public int Priority;

        [Tooltip("The range of this weapon. A modifier value of 0 means unchanged.")] public float Reach;

        [Tooltip("The speed of this weapon. A modifier value of 0 means unchanged.")] public float Speed;

        // Default Constructor
        public WeaponStats()
        {
            Priority = 0;
            IsModifier = false;
            Keyword = null;
            Reach = Constants.NullFloat;
            Speed = Constants.NullFloat;
        }

        // Constructor
        public WeaponStats(int priority, bool modifier, FormLink<IKeywordGetter> keyword, float speed = Constants.NullFloat, float reach = Constants.NullFloat)
        {
            Priority = priority;
            IsModifier = modifier;
            Keyword = keyword;
            Reach = reach;
            Speed = speed;
        }

        // FUNCTIONS

        private float GetFloat(float current, float local, out bool changed)
        {
            changed = !local.Equals(current) && !local.Equals(Constants.NullFloat);
            return changed ? IsModifier ? current + local : local : current;
        }

        public float GetReach(float current, out bool changed)
        {
            return GetFloat(current, Reach, out changed);
        }

        public float GetSpeed(float current, out bool changed)
        {
            return GetFloat(current, Speed, out changed);
        }

        // Retrieve the priority level of this instance, or -1 if it doesn't match anything.
        public int GetPriority(string id, ExtendedList<IFormLinkGetter<IKeywordGetter>>? keywords)
        {
            if ((keywords != null) && (Keyword != null!) && keywords.Any(kywd => Keyword.Equals(kywd)))
                return Priority;
            return Constants.DefaultPriority;
        }

        public bool IsNull()
        {
            return (Keyword == null) || (Reach.Equals(Constants.DefaultPriority) && Speed.Equals(Constants.DefaultPriority));
        }
    }

    public class Settings
    {
        [SettingName("Game Setting Changes")] [Tooltip("Don't change these unless you know what you're doing!")] public GameSettings GameSettings = new();

        [MaintainOrder] [SettingName("Weapon Groups")] [Tooltip("Change the stats of each weapon group.")] public List<WeaponStats> WeaponStats = new() {
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
            // TODO: Find appropriate value for NWTA & WotM unarmed weapons
            new WeaponStats(2, true, NWTA.Keyword.WeapTypeKatana),
            new WeaponStats(2, true, NWTA.Keyword.WeapTypeCurvedSword),
            new WeaponStats(2, true, WayOfTheMonk.Keyword.WeapTypeUnarmed)
        };

        public WeaponStats GetHighestPriorityStats(Weapon weapon)
        {
            WeaponStats highestStats = new();
            var highest = 0;
            foreach (var stats in WeaponStats) {
                var priority = stats.GetPriority(weapon.EditorID!, weapon.Keywords);
                if (priority <= highest) continue;
                highestStats = stats;
                highest = priority;
            }

            return highestStats;
        }

        public bool ApplyChangesToWeapon(Weapon weapon)
        {
            var stats = GetHighestPriorityStats(weapon);

            if (stats.IsNull())
                return false;

            weapon.Data!.Reach = stats.GetReach(weapon.Data.Reach, out var changedReach);
            weapon.Data!.Speed = stats.GetSpeed(weapon.Data.Speed, out var changedSpeed);

            // Revert any reach changes to giant clubs as they may cause issues with the AI
            if (weapon.EditorID?.ContainsInsensitive("GiantClub") == true)
                weapon.Data.Reach = 1.3F;
            return changedReach || changedSpeed;
        }
    }
}
