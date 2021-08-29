using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using Noggog;

namespace SpeedandReachFixes
{
    public class GameSettings
    {
        [MaintainOrder]
        [SettingName("Swing Angle Changes"), Tooltip("This setting is experimental! Only use it if you know what you're doing!")]
        public bool WeaponSwingAngleChanges = true;
        [SettingName("Fix Object Reach"), Tooltip("Changes how far away weapons can hit objects.")]
        public bool ObjectHitWeaponReach = true;
        [SettingName("Fix Combat Distance")]
        public bool CombatDistance = true;
        [SettingName("Fix Shield Bash Reach")]
        public bool CombatBashReach = true;
    }
    public class Matchable
    {
        [MaintainOrder]
        [Tooltip("The stats of the highest applicable category win overrides.")]
        public int Priority;
        [Tooltip("Weapons with this keyword are considered applicable.")]
        public FormLink<IKeywordGetter> Keyword = null!;
        [SettingName("Common Names"), Tooltip("Weapons with any of these words in their editor IDs are considered applicable.")]
        public List<string> MatchList = new ();

        private bool HasMatch(string id)
        {
            return MatchList.Any(match => id.Contains(match, StringComparison.OrdinalIgnoreCase));
        }
        // Retrieve the priority level of this instance, or -1 if it doesn't match anything.
        public int GetPriority(string id, ExtendedList<IFormLinkGetter<IKeywordGetter>>? keywords)
        {
            var valid = false;
            if (keywords != null && keywords.Any())
                foreach (var _ in keywords.Where(kywd => Keyword.Equals(kywd)))
                    valid = true;
            else
                valid = true;
            if (valid || HasMatch(id))
                return Priority;
            return -1;
        }
    }
    
    public class Stats : Matchable
    {
        public Stats(int priority, FormLink<IKeywordGetter> keyword, float? speed = null, float? reach = null, List<string>? matchlist = null)
        {
            Priority = priority;
            Keyword = keyword;
            Reach = reach ?? 0F;
            Speed = speed ?? 0F;
            if (matchlist != null)
                MatchList = matchlist;
        }

        public Stats()
        {
            Priority = 0;
            Reach = 0F;
            Speed = 0F;
        }
        [MaintainOrder]
        
        [Tooltip("The range of this weapon.")]
        public float Reach;
        [Tooltip("The speed of this weapon.")]
        public float Speed;
        
        public float GetReach(float reach, out bool isModified)
        {
            isModified = !Reach.Equals(reach) && !Reach.Equals(0F);
            return isModified ? Reach : reach;
        }
        public float GetSpeed(float speed, out bool isModified)
        {
            isModified = !Speed.Equals(speed) && !Reach.Equals(0F);
            return isModified ? Speed : speed;
        }
    }

    public class Settings
    {
        public GameSettings Gmst = new();
        
        [SettingName("Weapon Groups"), Tooltip("Change the stats of each weapon group."), JsonDiskName("weapon-groups")]
        public List<Stats> WeaponStats = new()
        {
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

        public Stats GetHighestPriorityStats(Weapon weapon)
        {
            Stats highestStats = new();
            var highest = 0;
            foreach (var stats in WeaponStats)
            {
                var priority = stats.GetPriority(weapon.EditorID!, weapon.Keywords);
                if (priority <= highest) continue;
                highestStats = stats;
                highest = priority;
            }
            return highestStats;
        }
    }
}
