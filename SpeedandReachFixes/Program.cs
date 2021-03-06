using Mutagen.Bethesda;
using Mutagen.Bethesda.FormKeys.SkyrimSE;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpeedandReachFixes
{
    public static class MyExtensions
    {
        public static bool HasKeyword(this IKeywordedGetter hasKeywords, IFormLink<IKeywordCommonGetter> formKey) => hasKeywords.Keywords?.Contains(formKey) == true;

        public static bool HasAnyKeyword(this IKeywordedGetter hasKeywords, ISet<IFormLink<IKeywordCommonGetter>> formKeys)
        {
            if (hasKeywords.Keywords is null) return false;
            foreach (var keyword in hasKeywords.Keywords)
            {
                if (formKeys.Contains(keyword))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .Run(
                    args,
                    new()
                    {
                        ActionsForEmptyArgs = new()
                        {
                            IdentifyingModKey = "SpeedAndReachFixes.esp",
                            TargetRelease = GameRelease.SkyrimSE
                        }
                    }
                );
        }

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease)
            {
                EditorID = "fObjectHitWeaponReach",
                Data = 81
            });

            state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease)
            {
                EditorID = "fObjectHitTwoHandReach",
                Data = 135
            });

            state.PatchMod.GameSettings.Add(new GameSettingFloat(state.PatchMod.GetNextFormKey(), state.PatchMod.SkyrimRelease)
            {
                EditorID = "fObjectHitH2HReach",
                Data = 61
            });

            foreach (var gmst in state.LoadOrder.PriorityOrder.WinningOverrides<IGameSettingGetter>())
            {
                if (gmst.EditorID?.Contains("fCombatDistance") == true)
                {
                    var modifiedGmst = state.PatchMod.GameSettings.GetOrAddAsOverride(gmst);
                    ((GameSettingFloat)modifiedGmst).Data = 141;
                }

                if (gmst.EditorID?.Contains("fCombatBashReach") == true)
                {
                    var modifiedGmst = state.PatchMod.GameSettings.GetOrAddAsOverride(gmst);
                    ((GameSettingFloat)modifiedGmst).Data = 61;
                }
            }

            foreach (var race in state.LoadOrder.PriorityOrder.WinningOverrides<IRaceGetter>())
            {
                if (race.Attacks == null) continue;

                if (!race.HasKeyword(Skyrim.Keyword.ActorTypeNPC)) continue;

                var modifiedRace = state.PatchMod.Races.GetOrAddAsOverride(race);

                foreach (var attack in modifiedRace.Attacks)
                {
                    if (attack.AttackData == null) continue;
                    attack.AttackData.StrikeAngle = attack.AttackData.StrikeAngle + 7;
                }
            }

            foreach (var weap in state.LoadOrder.PriorityOrder.WinningOverrides<IWeaponGetter>())
            {
                if (weap.Data == null) continue;

                var weapon = state.PatchMod.Weapons.GetOrAddAsOverride(weap);

                Program.AdjustWeaponReach(weapon);
                Program.AdjustWeaponSpeed(weapon);
            }
        }

        public static void AdjustWeaponReach(Weapon weapon)
        {
            if (weapon.Data == null) return;

            // Set the vanilla values first so that they can be overidden by more specific
            // settings later such as the case with Animated Armory where multiple keywords
            // for weapon type exist on a single weapon.
            if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeBattleaxe))       weapon.Data.Reach = 0.8275F;
            else if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeDagger))     weapon.Data.Reach = 0.533F;
            else if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeGreatsword)) weapon.Data.Reach = 0.88F;
            else if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeMace))       weapon.Data.Reach = 0.75F;
            else if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeSword))      weapon.Data.Reach = 0.83F;
            else if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeWarAxe))     weapon.Data.Reach = 0.6F;
            else if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeWarhammer))  weapon.Data.Reach = 0.8F;

            // Animated Armoury support
            if (weapon.HasKeyword(AnimatedArmoury.Keyword.WeapTypeCestus))        weapon.Data.Reach = weapon.Data.Reach - 0F;     // Intentionally left untouched
            else if (weapon.HasKeyword(AnimatedArmoury.Keyword.WeapTypeClaw))     weapon.Data.Reach = weapon.Data.Reach - 0.41F;
            else if (weapon.HasKeyword(AnimatedArmoury.Keyword.WeapTypeHalberd))  weapon.Data.Reach = weapon.Data.Reach - 0.58F;
            else if (weapon.HasKeyword(AnimatedArmoury.Keyword.WeapTypePike))     weapon.Data.Reach = weapon.Data.Reach - 0.2F;
            else if (weapon.HasKeyword(AnimatedArmoury.Keyword.WeapTypeQtrStaff)) weapon.Data.Reach = weapon.Data.Reach - 0.25F;
            else if (weapon.HasKeyword(AnimatedArmoury.Keyword.WeapTypeRapier))   weapon.Data.Reach = weapon.Data.Reach - 0.2F;
            else if (weapon.HasKeyword(AnimatedArmoury.Keyword.WeapTypeSpear))    weapon.Data.Reach = weapon.Data.Reach - 0F;     // Intentionally left untouched
            else if (weapon.HasKeyword(AnimatedArmoury.Keyword.WeapTypeWhip))     weapon.Data.Reach = weapon.Data.Reach - 0.5F;

            // Revert any changes to giant clubs as they may cause issues with the AI
            if (weapon.EditorID?.ContainsInsensitive("GiantClub") == true)
            {
                weapon.Data.Reach = 1.3F;
            }
        }

        private static readonly HashSet<IFormLink<IKeywordCommonGetter>> WeaponSpeedAdjustmentExclusionList = new()
        {
            AnimatedArmoury.Keyword.WeapTypeCestus,
            AnimatedArmoury.Keyword.WeapTypeClaw,
            AnimatedArmoury.Keyword.WeapTypeHalberd,
            AnimatedArmoury.Keyword.WeapTypePike,
            AnimatedArmoury.Keyword.WeapTypeQtrStaff,
            AnimatedArmoury.Keyword.WeapTypeRapier,
            AnimatedArmoury.Keyword.WeapTypeSpear,
            AnimatedArmoury.Keyword.WeapTypeWhip
        };

        public static void AdjustWeaponSpeed(Weapon weapon)
        {
            if (weapon.Data == null) return;

            if (weapon.HasAnyKeyword(WeaponSpeedAdjustmentExclusionList)) return;

            if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeBattleaxe))       weapon.Data.Speed = 0.666667F;
            else if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeDagger))     weapon.Data.Speed = 1.35F;
            else if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeGreatsword)) weapon.Data.Speed = 0.85F;
            else if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeMace))       weapon.Data.Speed = 0.9F;
            else if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeSword))      weapon.Data.Speed = 1.1F;
            else if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeWarAxe))     weapon.Data.Speed = 1F;
            else if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeWarhammer))  weapon.Data.Speed = 0.6F;
        }
    }
}
