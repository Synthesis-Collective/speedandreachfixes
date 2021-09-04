using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.WPF.Reflection.Attributes;
using Noggog;

namespace SpeedandReachFixes {
	/// <summary>
	/// Contains default value constants used internally.
	/// </summary>
	internal struct Constants
    {
        public const float NullFloat = -0F; // default value assigned to null stat values, indicates that they should be skipped.
        public const int DefaultPriority = -1; // default priority returned when no category matches were found
    }

	/// <summary>
	/// Contains game settings related to weapon type reach modifiers, specifically:
	///		fObjectHitWeaponReach, 
	///		fObjectHitTwoHandReach, 
	///	  & fObjectHitH2HReach.
	/// </summary>
	public class GameSettingsWeaponTypeReach {
        [MaintainOrder]

        [Tooltip("Enables this category. It is highly recommended that you leave this enabled!")] 
        public bool Enabled = true;

        [SettingName( "fObjectHitWeaponReach" ), Tooltip( "Modifier added to the reach of one-handed weapons.")]
        public float fObjectHitWeaponReach = 81F;

        [SettingName( "fObjectHitTwoHandReach" ), Tooltip( "Modifier added to the reach of two-handed weapons." )]
        public float fObjectHitTwoHandReach = 135F;

        [SettingName( "fObjectHitH2HReach" ), Tooltip( "Modifier added to unarmed reach." )]
        public float fObjectHitH2HReach = 61F;

		// Adds the game settings from this class to the current patcher state
		public int AddGameSettings(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (!Enabled) return 0;
            state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease)
            {
                EditorID = "fObjectHitWeaponReach",
                Data = fObjectHitWeaponReach
            });
            state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease)
            {
                EditorID = "fObjectHitTwoHandReach",
                Data = fObjectHitTwoHandReach
            });
            state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease)
            {
                EditorID = "fObjectHitH2HReach",
                Data = fObjectHitH2HReach
            });
            return 3;
        }
    }

	/// <summary>
	/// Contains game settings related to base reach multipliers, specifically fCombatDistance and fCombatBashReach.
	/// </summary>
	public class GameSettingsCombatReach {
        [MaintainOrder]

        [Tooltip("Enables this category. It is highly recommended that you leave this enabled!")]
        public bool Enabled = true;

        [SettingName( "fCombatDistance" ), Tooltip( "The base reach multiplier used for all attacks, except for shield bashes." )]
        public float fCombatDistance = 141F;

        [SettingName( "fCombatBashReach" ), Tooltip( "The base reach multiplier used for shield bash attacks.")]
        public float fCombatBashReach = 61F;

		// Adds the game settings from this class to the current patcher state
        public int AddGameSettings(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (!Enabled) return 0;
			state.PatchMod.GameSettings.Add( new GameSettingFloat( state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease ) {
				EditorID = "fCombatDistance",
				Data = fCombatDistance
			} );
			state.PatchMod.GameSettings.Add( new GameSettingFloat( state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease ) {
				EditorID = "fCombatBashReach",
				Data = fCombatBashReach
			} );
            return 2;
        }
    }

	/// <summary>
	/// Contains settings related to GMST (Game Setting) records.
	/// </summary>
	public class GameSettings
    {
        [MaintainOrder]
		// Combat reach
		[SettingName( "Base Combat Reach Multipliers" ), Tooltip( "Melee Reach Formula: ( reach = { fCombatDistance | fCombatBashReach } * NPCRaceScale * WeaponReach + { fObjectHitWeaponReach | fObjectHitTwoHandReach | fObjectHitH2HReach } )" )]
		public GameSettingsCombatReach CombatReach = new();
		// Weapon type reach
		[SettingName( "Weapon Type Reach Modifiers" ), Tooltip("Melee Reach Formula: ( reach = ( fCombatDistance | fCombatBashReach ) * NPCRaceScale * WeaponReach + ( fObjectHitWeaponReach | fObjectHitTwoHandReach | fObjectHitH2HReach ) )")]
        public GameSettingsWeaponTypeReach WeaponTypeReach = new();
		
		public int AddGameSettingsToPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
		{
			var count = 0;
			count += WeaponTypeReach.AddGameSettings( state ); // add game settings from weapon type reach category
			count += CombatReach.AddGameSettings( state ); // add game settings from combat reach category
			return count;
		}
    }

	/// <summary>
	/// Represents the stats associated with a given weapon keyword, as well as a priority level which determines the winning category if a weapon has multiple keywords.
	/// </summary>
	public class WeaponStats
    {
		[MaintainOrder]

		[Tooltip( "The keyword attached to this weapon type." )]
		public FormLink<IKeywordGetter>? Keyword;

		[Tooltip( "When multiple weapon types apply to the same category, the highest priority wins." )]
		public int Priority;

		[SettingName("Is Additive Modifier"), Tooltip("When checked, adds the specified values rather than overwriting them. Negative values will subtract.")] 
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
            Keyword = null;
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
            changed =  !local.EqualsWithin(current) && !local.EqualsWithin(Constants.NullFloat); // if current != local and local is set to a valid number
			if ( changed )
				return  IsAdditiveModifier ? ( current + local ) : local ;
			return current;
        }

		/// <summary>
		/// Takes a weapon record's current reach value and calculates the final value using this category's configured stats.
		/// </summary>
		/// <param name="current">Current reach value</param>
		/// <param name="changed">Set to true if the return value does not equal current</param>
		/// <returns></returns>
        public float GetReach(float current, out bool changed)
        {
            return GetFloat(current, Reach, out changed);
        }

		/// <summary>
		/// Takes a weapon record's current speed value and calculates the final value using this category's configured stats.
		/// </summary>
		/// <param name="current">Current speed value</param>
		/// <param name="changed">Set to true if the return value does not equal current</param>
		/// <returns></returns>
		public float GetSpeed(float current, out bool changed)
        {
            return GetFloat(current, Speed, out changed);
        }

        // Retrieve the priority level of this instance, or -1 if it doesn't match anything.
        public int GetPriority(ExtendedList<IFormLinkGetter<IKeywordGetter>>? keywords)
        {
            if ((keywords != null) && (Keyword != null!) && keywords.Any(kywd => Keyword.Equals(kywd)))
                return Priority;
            return Constants.DefaultPriority;
        }
		// Returns true if the keyword is not set, or the reach and speed values are both unset.
        public bool ShouldSkip()
        {
            return (Keyword == null) || (Reach.Equals(Constants.DefaultPriority) && Speed.Equals(Constants.DefaultPriority));
        }
    }

	/// <summary>
	/// Contains all settings used throughout the patcher.
	/// This is the object passed to SetAutoGeneratedSettings.
	/// </summary>
	public class Settings
    {
        [MaintainOrder]

		// Game Settings
        [SettingName("Game Setting Changes"), Tooltip("Don't change these unless you know what you're doing!")] 
        public GameSettings GameSettings = new();

		// Race Attack Angle
        [SettingName("Experimental Race Strike Angle Modifier"), Tooltip("Changes the angle at which an NPC can be hit by an attack. This value is a modifier and is added to the current value for all attacks and races. Set to 0 to disable.")]
        public float AttackStrikeAngleModifier = 7F;

		// List of WeaponStats objects, each relating to a different weapon keyword.
        [SettingName("Weapon Groups"), Tooltip("Change the stats of each weapon group.")] 
        public List<WeaponStats> WeaponStats = new() {
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
            var highest = 0;
            foreach (var stats in WeaponStats) {
                var priority = stats.GetPriority(weapon.Keywords);
                if (priority <= highest) continue;
                highestStats = stats;
                highest = priority;
            }

            return highestStats;
        }
		
		// Applies the current weapon stats configuration to a given weapon ref.
        public bool ApplyChangesToWeapon(Weapon weapon)
        {
			if ( weapon.EditorID == null )
				return false;
            var stats = GetHighestPriorityStats(weapon);

            if ( stats.ShouldSkip() )
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
