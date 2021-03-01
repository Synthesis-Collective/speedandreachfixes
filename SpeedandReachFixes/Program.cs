using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using System.Threading.Tasks;
using Mutagen.Bethesda.FormKeys.SkyrimSE;

namespace SpeedandReachFixes
{
    public class Program
    {
        public static Task<int> Main(string[] args)
        {
            return SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .Run(args, new RunPreferences()
                {
                    ActionsForEmptyArgs = new RunDefaultPatcher()
                    {
                        IdentifyingModKey = "SpeedAndReachFixes.esp",
                        TargetRelease = GameRelease.SkyrimSE
                    }
                });
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
            if      (weapon.HasKeyword(Skyrim.Keyword.WeapTypeBattleaxe))  weapon.Data.Reach = 0.8275F;
            else if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeDagger))     weapon.Data.Reach = 0.533F;
            else if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeGreatsword)) weapon.Data.Reach = 0.88F;
            else if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeMace))       weapon.Data.Reach = 0.75F;
            else if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeSword))      weapon.Data.Reach = 0.83F;
            else if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeWarAxe))     weapon.Data.Reach = 0.6F;
            else if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeWarhammer))  weapon.Data.Reach = 0.8F;

            // Animated Armoury support
            if      (weapon.HasKeyword(NewArmoury.Keyword.WeapTypeCestus))     weapon.Data.Reach = weapon.Data.Reach - 0F;     // Intentionally left untouched
            else if (weapon.HasKeyword(NewArmoury.Keyword.WeapTypeClaw))       weapon.Data.Reach = weapon.Data.Reach - 0.41F;
            else if (weapon.HasKeyword(NewArmoury.Keyword.WeapTypeHalberd))    weapon.Data.Reach = weapon.Data.Reach - 0.58F;
            else if (weapon.HasKeyword(NewArmoury.Keyword.WeapTypePike))       weapon.Data.Reach = weapon.Data.Reach - 0.2F;
            else if (weapon.HasKeyword(NewArmoury.Keyword.WeapTypeQtrStaff))   weapon.Data.Reach = weapon.Data.Reach - 0.25F;
            else if (weapon.HasKeyword(NewArmoury.Keyword.WeapTypeRapier))     weapon.Data.Reach = weapon.Data.Reach - 0.2F;
            else if (weapon.HasKeyword(NewArmoury.Keyword.WeapTypeSpear))      weapon.Data.Reach = weapon.Data.Reach - 0F;     // Intentionally left untouched
            else if (weapon.HasKeyword(NewArmoury.Keyword.WeapTypeWhip))       weapon.Data.Reach = weapon.Data.Reach - 0.5F;

            // Revert any changes to giant clubs as they may cause issues with the AI
            if (weapon.EditorID?.ContainsInsensitive("GiantClub") == true)
            {
                weapon.Data.Reach = 1.3F;
            }
        }

        public static void AdjustWeaponSpeed(Weapon weapon)
        {
            if (weapon.Data == null) return;

            HashSet<FormKey> exclusionList = new()
            {
                NewArmoury.Keyword.WeapTypeCestus,
                NewArmoury.Keyword.WeapTypeClaw,
                NewArmoury.Keyword.WeapTypeHalberd,
                NewArmoury.Keyword.WeapTypePike,
                NewArmoury.Keyword.WeapTypeQtrStaff,
                NewArmoury.Keyword.WeapTypeRapier,
                NewArmoury.Keyword.WeapTypeSpear,
                NewArmoury.Keyword.WeapTypeWhip,
            };

            if (weapon.Keywords.EmptyIfNull().Any(k => exclusionList.Contains(k.FormKey))) return;

            if      (weapon.HasKeyword(Skyrim.Keyword.WeapTypeBattleaxe))  weapon.Data.Speed = 0.666667F;
            else if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeDagger))     weapon.Data.Speed = 1.35F;
            else if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeGreatsword)) weapon.Data.Speed = 0.85F;
            else if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeMace))       weapon.Data.Speed = 0.9F;
            else if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeSword))      weapon.Data.Speed = 1.1F;
            else if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeWarAxe))     weapon.Data.Speed = 1F;
            else if (weapon.HasKeyword(Skyrim.Keyword.WeapTypeWarhammer))  weapon.Data.Speed = 0.6F;
        }
    }
}
