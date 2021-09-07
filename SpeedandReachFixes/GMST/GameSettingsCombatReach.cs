using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace SpeedandReachFixes.GMST
{
    /// <summary>
    /// Contains game settings related to base reach multipliers, specifically fCombatDistance and fCombatBashReach.
    /// </summary>
    public class GameSettingsCombatReach
    {
        [MaintainOrder]

        [Tooltip("Enables this category. It is highly recommended that you leave this enabled!")]
        public bool Enabled = true;

        [SettingName("fCombatDistance")]
        [Tooltip("The base reach multiplier used for all attacks, except for shield bashes.")]
        public float fCombatDistance = 141F;

        [SettingName("fCombatBashReach")]
        [Tooltip("The base reach multiplier used for shield bash attacks.")]
        public float fCombatBashReach = 61F;

        public int AddGameSettings(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (!Enabled) return 0;
            state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease)
            {
                EditorID = "fCombatDistance",
                Data = fCombatDistance
            });
            state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease)
            {
                EditorID = "fCombatBashReach",
                Data = fCombatBashReach
            });
            return 2;
        }
    }
}
