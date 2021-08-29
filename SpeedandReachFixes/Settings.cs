using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using Noggog;

namespace SpeedandReachFixes
{
    internal static class Constants {
        public const float NullFloat = -1F; // default value assigned to null stat values
    }
    /**
     * @class GameSettings
     * @brief Contains settings related to GMST (Game Setting) records.
     */
    public class GameSettings
    {
        [MaintainOrder]
        [SettingName("[EXPERIMENTAL] Change Swing Angle for all races."), Tooltip("Don't change this unless you know what you're doing!")]
        public bool WeaponSwingAngleChanges = true;
        [SettingName("Fix Object Reach"), Tooltip("Changes how far away weapons can hit objects.")]
        public bool ObjectHitWeaponReach = true;
        [SettingName("Fix Combat Distance")]
        public bool CombatDistance = true;
        [SettingName("Fix Shield Bash Reach")]
        public bool CombatBashReach = true;
    }
    /**
     * @class Stats
     * @brief Represents the stats associated with a given weapon keyword, and an optional user-customizable matchlist to allow
     */
    public class Stats
    {
        public Stats(int priority, FormLink<IKeywordGetter> keyword, float? speed = null, float? reach = null /*, List<string>? matchlist = null*/)
        {
            Priority = priority;
            Keyword = keyword;
            Reach = reach ?? Constants.NullFloat;
            Speed = speed ?? Constants.NullFloat;
        }

        public Stats()
        {
            Priority = 0;
            Reach = Constants.NullFloat;
            Speed = Constants.NullFloat;
        }
        [MaintainOrder]
        [Tooltip("When multiple weapon types apply to the same category, the priority level .")]
        public int Priority;
        [Tooltip("The range of this weapon. A value of -1 means unchanged.")]
        public float Reach;
        [Tooltip("The speed of this weapon. A value of -1 means unchanged.")]
        public float Speed;
        [Tooltip("The keyword attached to this weapon type.")]
        public FormLink<IKeywordGetter> Keyword = new();
        
        public float GetReach(float reach, out bool isModified)
        {
            isModified = !Reach.Equals(reach) && !Reach.Equals(Constants.NullFloat);
            return isModified ? Reach : reach;
        }
        public float GetSpeed(float speed, out bool isModified)
        {
            isModified = !Speed.Equals(speed) && !Reach.Equals(Constants.NullFloat);
            return isModified ? Speed : speed;
        }
        // Retrieve the priority level of this instance, or -1 if it doesn't match anything.
        public int GetPriority(string id, ExtendedList<IFormLinkGetter<IKeywordGetter>>? keywords)
        {
            var valid = false;
            if ((keywords != null) && keywords.Any())
                foreach (var _ in keywords.Where(kywd => Keyword.Equals(kywd)))
                    valid = true;
            else
                valid = true;
            if (valid)
                return Priority;
            return -1;
        }
    }

    public class Settings
    {
        public GameSettings Gmst = new();
        
        [SettingName("Weapon Groups"), Tooltip("Change the stats of each weapon group."), JsonDiskName("weapon-groups")]
        public List<Stats> WeaponStats = new()
        { // TODO: Add Bow Weapon Type
            new Stats(1, Skyrim.Keyword.WeapTypeBattleaxe, 0.666667F, 0.8275F),
            new Stats(1, Skyrim.Keyword.WeapTypeDagger, 1.35F, 0.533F),
            new Stats(1, Skyrim.Keyword.WeapTypeGreatsword, 0.85F, 0.88F),
            new Stats(1, Skyrim.Keyword.WeapTypeMace, 0.9F, 0.75F),
            new Stats(1, Skyrim.Keyword.WeapTypeSword, 1.1F, 0.83F),
            new Stats(1, Skyrim.Keyword.WeapTypeWarAxe, 1F, 0.6F),
            new Stats(1, Skyrim.Keyword.WeapTypeWarhammer, 0.6F, 0.8F),
            new Stats(1, Skyrim.Keyword.WeapTypeWarhammer, 0.6F, 0.8F),
            new Stats(2, NewArmoury.Keyword.WeapTypeCestus),
            new Stats(2, NewArmoury.Keyword.WeapTypeClaw, null, 0.41F),
            new Stats(2, NewArmoury.Keyword.WeapTypeHalberd, null, 1.71F),
            new Stats(2, NewArmoury.Keyword.WeapTypePike, null, 1.88F),
            new Stats(2, NewArmoury.Keyword.WeapTypeQtrStaff, null, 0.88F),
            new Stats(2, NewArmoury.Keyword.WeapTypeRapier, null, 0.8275F),
            new Stats(2, NewArmoury.Keyword.WeapTypeSpear, null, 1.5F),
            new Stats(2, NewArmoury.Keyword.WeapTypeWhip, null, 1.1F)
        };

        /*public Stats GetHighestPriorityStatsFuzzy(Weapon weapon)
        {
            Stats highest = new();
            var reachPriority = 0; // priority of last selected reach value
            var speedPriority = 0; // priority of last selected speed value
            foreach (var match in WeaponStats) {
                // Get match priority
                var priority = match.GetPriority(weapon.EditorID!, weapon.Keywords);
                // Reach
                var highestReachNull = highest.Reach.Equals(-1F);
                if (highestReachNull || (!highest.Reach.Equals(match.Reach) && (priority > reachPriority)))  {
                    highest.Reach = match.Reach;
                    reachPriority = priority;
                }
                // Speed
                var highestSpeedNull = highest.Speed.Equals(-1F);
                if (highestSpeedNull || (!highest.Speed.Equals(match.Speed) && (priority > speedPriority))) {
                    highest.Speed = match.Speed;
                    speedPriority = priority;
                }
            }
            return highest;
        }*/

        public Stats GetHighestPriorityStats(Weapon weapon, out string chosenKeyword)
        {
            Stats highestStats = new();
            var highest = 0;
            chosenKeyword = "";
            foreach (var stats in WeaponStats)
            {
                var priority = stats.GetPriority(weapon.EditorID!, weapon.Keywords);
                if (priority <= highest) continue;
                highestStats = stats;
                highest = priority;
                chosenKeyword = stats.Keyword.FormKey.IDString();
            }
            return highestStats;
        }
    }
}
