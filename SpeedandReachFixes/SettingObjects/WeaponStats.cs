using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using Noggog;
using System.Linq;

namespace SpeedandReachFixes.SettingObjects
{
    /// <summary>
    /// Represents the stats associated with a given weapon keyword, as well as a priority level which determines the winning category if a weapon has multiple keywords.
    /// </summary>
    [ObjectNameMember(nameof(Keyword))]
    public class WeaponStats
    {
        [MaintainOrder]

        [Tooltip("The keyword attached to this weapon type.")]
        public FormLink<IKeywordGetter> Keyword;

        [Tooltip("When multiple weapon types apply to the same category, the highest priority wins.")]
        public int Priority;

        [SettingName("Is Additive Modifier")]
        [Tooltip("When checked, adds the specified values rather than overwriting them. Negative values will subtract.")]
        public bool IsAdditiveModifier;

        [Tooltip("The range of this weapon. A modifier value of 0 means unchanged.")]
        public float Reach;

        [Tooltip("The speed of this weapon. A modifier value of 0 means unchanged.")]
        public float Speed;

        // Default Constructor
        public WeaponStats()
        {
            Priority = 0;
            IsAdditiveModifier = false;
            Keyword = new();
            Keyword.SetToNull(); // set keyword to null (all 0s)
            Reach = Constants.NullFloat;
            Speed = Constants.NullFloat;
        }

        // Constructor
        public WeaponStats(int priority, bool modifier, FormLink<IKeywordGetter> keyword, float speed = Constants.NullFloat, float reach = Constants.NullFloat)
        {
            Priority = priority;
            IsAdditiveModifier = modifier;
            Keyword = keyword;
            Reach = reach;
            Speed = speed;
        }

        /// <summary>
        /// Takes the current & member values for Reach / Speed, returns their sum if IsModifier is true, the member value if
        /// Private function, only usable within WeaponStats
        /// See GetReach() & GetSpeed() for public access functions.
        /// </summary>
        /// <param name="current">The current value of any given weapon's speed or reach stat.</param>
        /// <param name="local">The member value of either Speed or Reach depending on which stat is being requested.</param>
        /// <param name="changed">When true, the return value != current, else the returned value is equal to current.</param>
        /// <returns>float</returns>
        private float GetFloat(float current, float local, out bool changed)
        {
            changed = !local.EqualsWithin(current) && !local.EqualsWithin(Constants.NullFloat); // if current != local and local is set to a valid number
            if (changed) // return sum if additive modifier is true, else return local
                return IsAdditiveModifier ? (current + local) : local;
            return current;
        }

        /// <summary>
        /// Takes a weapon record's current reach value and calculates the final value using this category's configured stats.
        /// </summary>
        /// <param name="current">Current reach value</param>
        /// <param name="changed">Set to true if the return value does not equal current</param>
        /// <returns>float</returns>
        public float GetReach(float current, out bool changed)
        {
            return GetFloat(current, Reach, out changed);
        }

        /// <summary>
        /// Takes a weapon record's current speed value and calculates the final value using this category's configured stats.
        /// </summary>
        /// <param name="current">Current speed value</param>
        /// <param name="changed">Set to true if the return value does not equal current</param>
        /// <returns>float</returns>
        public float GetSpeed(float current, out bool changed)
        {
            return GetFloat(current, Speed, out changed);
        }

        /// <summary>
        /// Check if this WeaponStats object is not null, and not using default values.
        /// </summary>
        /// <returns>bool</returns>
        public bool ShouldSkip()
        {
            return (Keyword.IsNull) || (Reach.Equals(Constants.DefaultPriority) && Speed.Equals(Constants.DefaultPriority));
        }

        /// <summary>
        /// Retrieve the priority level of this WeaponStats instance, if it is not null and contains at least one valid value.
        /// </summary>
        /// <param name="keywords">List of keywords currently applied to a weapon.</param>
        /// <returns>int</returns>
        public int GetPriority(ExtendedList<IFormLinkGetter<IKeywordGetter>>? keywords)
        {
            if ((keywords != null) && (!ShouldSkip()) && keywords.Any(kywd => Keyword.Equals(kywd)))
                return Priority;
            return Constants.DefaultPriority;
        }
    }
}
