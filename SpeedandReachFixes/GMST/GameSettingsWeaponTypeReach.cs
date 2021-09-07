// GameSettings subsection containing reach modifiers.
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace SpeedandReachFixes.GMST
{
    /// <summary>
    /// Contains game settings related to weapon type reach modifiers, specifically:
    ///		fObjectHitWeaponReach,
    ///		fObjectHitTwoHandReach,
    ///	  & fObjectHitH2HReach.
    /// </summary>
    public class GameSettingsWeaponTypeReach
    {
        [MaintainOrder]
        [Tooltip("Enables this category. It is highly recommended that you leave this enabled!")]
        public bool Enabled = true;

        [SettingName("fObjectHitWeaponReach")]
        [Tooltip("Modifier added to the reach of one-handed weapons.")]
        public float fObjectHitWeaponReach = 81F;

        [SettingName("fObjectHitTwoHandReach")]
        [Tooltip("Modifier added to the reach of two-handed weapons.")]
        public float fObjectHitTwoHandReach = 135F;

        [SettingName("fObjectHitH2HReach")]
        [Tooltip("Modifier added to unarmed reach.")]
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
}
