using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using System.Threading.Tasks;

namespace SpeedandReachFixes
{
    public static class MyExtensions
    {
        public static bool hasKeyword(this IRaceGetter race, FormKey formKey)
        {
            foreach (var kwda in race.Keywords.EmptyIfNull())
            {
                if (kwda.FormKey == formKey)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool hasKeyword(this IWeaponGetter weapon, FormKey formKey)
        {
            foreach (var kwda in weapon.Keywords.EmptyIfNull())
            {
                if (kwda.FormKey == formKey)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool hasAnyKeyword(this IWeaponGetter weapon, FormKey[] formKeys)
        {
            foreach (var formKey in formKeys)
            {
                if (weapon.hasKeyword(formKey))
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class Program
    {
        public static Dictionary<string, FormKey> Keywords = new Dictionary<string, FormKey>
        {
            { "ActorTypeNPC", new FormKey("Skyrim.esm", 0x013794) },

            // Vanilla KWDA
            { "WeapTypeBattleaxe", new FormKey("Skyrim.esm", 0x06d932) },
            { "WeapTypeDagger", new FormKey("Skyrim.esm", 0x01e713) },
            { "WeapTypeGreatsword", new FormKey("Skyrim.esm", 0x06d931) },
            { "WeapTypeMace", new FormKey("Skyrim.esm", 0x01e714) },
            { "WeapTypeSword", new FormKey("Skyrim.esm", 0x01e711) },
            { "WeapTypeWarAxe", new FormKey("Skyrim.esm", 0x01e712) },
            { "WeapTypeWarhammer", new FormKey("Skyrim.esm", 0x06d930) },

            // Animated Armoury KWDA
            { "WeapTypeCestus", new FormKey("NewArmoury.esp", 0x19aab3) },
            { "WeapTypeClaw", new FormKey("NewArmoury.esp", 0x19aab4) },
            { "WeapTypeHalberd", new FormKey("NewArmoury.esp", 0x0e4580) },
            { "WeapTypePike", new FormKey("NewArmoury.esp", 0x0e457e) },
            { "WeapTypeQtrStaff", new FormKey("NewArmoury.esp", 0x0e4581) },
            { "WeapTypeRapier", new FormKey("NewArmoury.esp", 0x000801) },
            { "WeapTypeSpear", new FormKey("NewArmoury.esp", 0x0e457f) },
            { "WeapTypeWhip", new FormKey("NewArmoury.esp", 0x20f2a1) }
        };

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

                if (!race.hasKeyword(Program.Keywords["ActorTypeNPC"])) continue;

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
            if      (weapon.hasKeyword(Program.Keywords["WeapTypeBattleaxe"]))  weapon.Data.Reach = 0.8275F;
            else if (weapon.hasKeyword(Program.Keywords["WeapTypeDagger"]))     weapon.Data.Reach = 0.533F;
            else if (weapon.hasKeyword(Program.Keywords["WeapTypeGreatsword"])) weapon.Data.Reach = 0.88F;
            else if (weapon.hasKeyword(Program.Keywords["WeapTypeMace"]))       weapon.Data.Reach = 0.75F;
            else if (weapon.hasKeyword(Program.Keywords["WeapTypeSword"]))      weapon.Data.Reach = 0.83F;
            else if (weapon.hasKeyword(Program.Keywords["WeapTypeWarAxe"]))     weapon.Data.Reach = 0.6F;
            else if (weapon.hasKeyword(Program.Keywords["WeapTypeWarhammer"]))  weapon.Data.Reach = 0.8F;

            // Animated Armoury support
            if      (weapon.hasKeyword(Program.Keywords["WeapTypeCestus"]))     weapon.Data.Reach = weapon.Data.Reach - 0F;     // Intentionally left untouched
            else if (weapon.hasKeyword(Program.Keywords["WeapTypeClaw"]))       weapon.Data.Reach = weapon.Data.Reach - 0.41F;
            else if (weapon.hasKeyword(Program.Keywords["WeapTypeHalberd"]))    weapon.Data.Reach = weapon.Data.Reach - 0.58F;
            else if (weapon.hasKeyword(Program.Keywords["WeapTypePike"]))       weapon.Data.Reach = weapon.Data.Reach - 0.2F;
            else if (weapon.hasKeyword(Program.Keywords["WeapTypeQtrStaff"]))   weapon.Data.Reach = weapon.Data.Reach - 0.25F;
            else if (weapon.hasKeyword(Program.Keywords["WeapTypeRapier"]))     weapon.Data.Reach = weapon.Data.Reach - 0.2F;
            else if (weapon.hasKeyword(Program.Keywords["WeapTypeSpear"]))      weapon.Data.Reach = weapon.Data.Reach - 0F;     // Intentionally left untouched
            else if (weapon.hasKeyword(Program.Keywords["WeapTypeWhip"]))       weapon.Data.Reach = weapon.Data.Reach - 0.5F;

            // Revert any changes to giant clubs as they may cause issues with the AI
            if (weapon.EditorID?.ContainsInsensitive("GiantClub") == true)
            {
                weapon.Data.Reach = 1.3F;
            }
        }

        public static void AdjustWeaponSpeed(Weapon weapon)
        {
            if (weapon.Data == null) return;

            FormKey[] exclusionList = { 
                Program.Keywords["WeapTypeCestus"], 
                Program.Keywords["WeapTypeClaw"], 
                Program.Keywords["WeapTypeHalberd"], 
                Program.Keywords["WeapTypePike"], 
                Program.Keywords["WeapTypeQtrStaff"], 
                Program.Keywords["WeapTypeRapier"], 
                Program.Keywords["WeapTypeSpear"],
                Program.Keywords["WeapTypeWhip"]
            };

            if (weapon.hasAnyKeyword(exclusionList)) return;

            if      (weapon.hasKeyword(Program.Keywords["WeapTypeBattleaxe"]))  weapon.Data.Speed = 0.666667F;
            else if (weapon.hasKeyword(Program.Keywords["WeapTypeDagger"]))     weapon.Data.Speed = 1.35F;
            else if (weapon.hasKeyword(Program.Keywords["WeapTypeGreatsword"])) weapon.Data.Speed = 0.85F;
            else if (weapon.hasKeyword(Program.Keywords["WeapTypeMace"]))       weapon.Data.Speed = 0.9F;
            else if (weapon.hasKeyword(Program.Keywords["WeapTypeSword"]))      weapon.Data.Speed = 1.1F;
            else if (weapon.hasKeyword(Program.Keywords["WeapTypeWarAxe"]))     weapon.Data.Speed = 1F;
            else if (weapon.hasKeyword(Program.Keywords["WeapTypeWarhammer"]))  weapon.Data.Speed = 0.6F;
        }
    }
}
