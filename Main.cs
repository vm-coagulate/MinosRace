using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Blueprints.Root;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.DLC;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.Utility;
using Kingmaker.View;
using Kingmaker.Visual.CharactersRigidbody;
using MinosRace.Context;
using Owlcat.Runtime.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TabletopTweaks.Core.NewComponents;
using TabletopTweaks.Core.Utilities;
using UnityModManagerNet;

namespace MinosRace
{
    public static class Main
    {
        public static bool Enabled;

        static bool Load(UnityModManager.ModEntry modEntry)
        {

            modEntry.OnToggle = OnToggle;
            MC.mc = new MC(modEntry);
            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            return true;
        }
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            Enabled = value;
            return true;
        }
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch
        {
            static bool Initialized = false;
            [HarmonyPriority(Priority.First)]
            [HarmonyPostfix]
            static void CreateNewBlueprints()
            {
                if (Initialized)
                    return;
                Initialized = true;
                AddNewRace();
            }

            private static void AddNewRace()
            {
                BlueprintRace HumanRace = BlueprintTools.GetBlueprint<BlueprintRace>("0a5d473ead98b0646b94495af250fdc4");
            BlueprintRace TieflingRace = BlueprintTools.GetBlueprint<BlueprintRace>("5c4e42124dc2b4647af6e36cf2590500");

            BlueprintFeature destinyBeyondbirth, minosHorns, beeingBuff,gregarious,emphatic,Minosweaponfamiliarity;
                CreateFeatures(out destinyBeyondbirth, out minosHorns, out beeingBuff,out gregarious, out emphatic, out Minosweaponfamiliarity);
                var MinosRace = Helpers.CreateBlueprint<BlueprintRace>(MC.mc, "MinosRace",
                    bp =>
                    {
                        bp.SetName(MC.mc, "Minos");
                        bp.SetDescription(MC.mc, "Minotaur blooded, or simply called minos, have both human and minotaur blood " +
                           "running through their veins, but are considered their own separate race. Their faces look human, and " +
                           "their skin is smooth and hairless, but they do have the horns and tail of a minotaur. Despite their " +
                           "size and heritage, they are mostly peaceful, which is why most true minotaurs consider it an insult " +
                           "that they are related. They are not drawn towards the abyss or hell like tieflings are, and they usually " +
                           "get along much better with humans due to their gregarious nature. Those that decide to adventure, instead " +
                           "of living a life of farming, often make use of their greater physical prowess in battle.");
                        bp.SelectableRaceStat = false;
                        bp.m_Features=bp.m_Features.AddRangeToArray(new BlueprintFeatureBaseReference[] {
                        minosHorns.ToReference<BlueprintFeatureBaseReference>(),
                        beeingBuff.ToReference<BlueprintFeatureBaseReference>() });
                        bp.m_Presets = TieflingRace.m_Presets;
                        bp.MaleOptions = TieflingRace.MaleOptions;
                        bp.FemaleOptions = TieflingRace.FemaleOptions;
                        bp.RaceId = Race.Catfolk;
                        bp.SoundKey = "Human";
                        bp.MaleSpeedSettings = HumanRace.MaleSpeedSettings;
                        bp.FemaleSpeedSettings = HumanRace.FemaleSpeedSettings;

                        bp.AddComponent<AddStatBonus>(
                           c =>
                           {
                               c.Descriptor = Kingmaker.Enums.ModifierDescriptor.Racial;
                               c.Stat = Kingmaker.EntitySystem.Stats.StatType.Strength;
                               c.Value = 4;
                           });
                        bp.AddComponent<AddStatBonusIfHasFact>(
                          c =>
                          {
                              c.Descriptor = Kingmaker.Enums.ModifierDescriptor.Racial;
                              c.Stat = Kingmaker.EntitySystem.Stats.StatType.Dexterity;
                              c.Value = -2;
                              c.InvertCondition = true;
                              c.m_CheckedFacts = c.m_CheckedFacts.AddToArray(destinyBeyondbirth.ToReference<BlueprintUnitFactReference>());
                          });
                        bp.AddComponent<AddStatBonus>(
                          c =>
                          {
                              c.Descriptor = Kingmaker.Enums.ModifierDescriptor.Racial;
                              c.Stat = Kingmaker.EntitySystem.Stats.StatType.SkillPersuasion;
                              c.Value = 2;
                          });

                        bp.AddComponent<AddClassSkill>(c => { c.Skill = Kingmaker.EntitySystem.Stats.StatType.SkillPersuasion; });
                    });
                BlueprintRoot.Instance.Progression.m_CharacterRaces=BlueprintRoot.Instance.Progression.m_CharacterRaces.AddToArray(MinosRace.ToReference<BlueprintRaceReference>());
            }

            private static void AddNewHeritage()
            {
                
                //heritage-none
                var NoAlternateTrait = Helpers.CreateBlueprint<BlueprintFeature>(MC.mc, "NoAlternateTrait", bp =>
                {
                    bp.IsClassFeature = true;
                    bp.Ranks = 1;
                    bp.Groups = new FeatureGroup[] { FeatureGroup.Racial };
                    bp.HideInUI = true;
                    bp.HideInCharacterSheetAndLevelUp = true;
                    bp.SetName(MC.mc, "None");
                    bp.SetDescription(MC.mc, "No Alternate Trait");
                    bp.AddComponent<ChangeVisualSize>(c => { c.Multiplier = 1f; c.reset = true; });
                });
                //heritage-minos
                BlueprintFeature destinyBeyondbirth, minosHorns, beeingBuff, gregarious, emphatic, Minosweaponfamiliarity;
                CreateFeatures(out destinyBeyondbirth, out minosHorns, out beeingBuff, out gregarious, out emphatic, out Minosweaponfamiliarity);
                var minosTrait = Helpers.CreateBlueprint<BlueprintFeature>(MC.mc, "MinosHeritage",
                    bp =>
                    {
                        bp.IsClassFeature = true;
                        bp.Ranks = 1;
                        bp.Groups = new FeatureGroup[] { FeatureGroup.Racial };
                        bp.HideInUI = true;
                        bp.HideInCharacterSheetAndLevelUp = true;
                        bp.SetName(MC.mc, "Minos");
                        bp.SetDescription(MC.mc, "Minotaur blooded, or simply called minos, have both human and minotaur blood " +
                            "running through their veins, but are considered their own separate race. Their faces look human, and " +
                            "their skin is smooth and hairless, but they do have the horns and tail of a minotaur. Despite their " +
                            "size and heritage, they are mostly peaceful, which is why most true minotaurs consider it an insult " +
                            "that they are related. They are not drawn towards the abyss or hell like tieflings are, and they usually " +
                            "get along much better with humans due to their gregarious nature. Those that decide to adventure, instead " +
                            "of living a life of farming, often make use of their greater physical prowess in battle.");
                        bp.AddComponent<AddStatBonus>(
                            c =>
                            {
                                c.Descriptor = Kingmaker.Enums.ModifierDescriptor.Racial;
                                c.Stat = Kingmaker.EntitySystem.Stats.StatType.Strength;
                                c.Value = 4;
                            });
                        bp.AddComponent<AddStatBonusIfHasFact>(
                          c =>
                          {
                              c.Descriptor = Kingmaker.Enums.ModifierDescriptor.Racial;
                              c.Stat = Kingmaker.EntitySystem.Stats.StatType.Dexterity;
                              c.Value = -2;
                              c.InvertCondition = true;
                              c.m_CheckedFacts = c.m_CheckedFacts.AddToArray(destinyBeyondbirth.ToReference<BlueprintUnitFactReference>());
                          });
                        bp.AddComponent<AddStatBonus>(
                          c =>
                          {
                              c.Descriptor = Kingmaker.Enums.ModifierDescriptor.Racial;
                              c.Stat = Kingmaker.EntitySystem.Stats.StatType.SkillPersuasion;
                              c.Value = 2;
                          });

                        bp.AddComponent<AddClassSkill>(c => { c.Skill = Kingmaker.EntitySystem.Stats.StatType.SkillPersuasion; });
                        bp.AddComponent<AddFacts>(
                            c =>
                            {
                                c.m_Facts = c.m_Facts.AddRangeToArray(new BlueprintUnitFactReference[]
                                {
                                    minosHorns.ToReference<BlueprintUnitFactReference>(),
                                    beeingBuff.ToReference<BlueprintUnitFactReference>()
                                });
                                //c.AddActionActivated(new ContextActionAddFeature() { m_PermanentFeature = minosHorns.ToReference<BlueprintFeatureReference>() });
                                //c.AddActionActivated(new ContextActionAddFeature() { m_PermanentFeature = beeingBuff.ToReference<BlueprintFeatureReference>() });

                            });
                    });
                //create heritage selection 
                BlueprintFeatureSelection heritageSelection = Helpers.CreateBlueprint<BlueprintFeatureSelection>(MC.mc, "HumanHeritageSelection",
                    bp =>
                    {
                        bp.SetName(MC.mc, "Racial heritage");
                        bp.SetDescription(MC.mc, "Some humans have other races in their family tree.");
                        bp.Groups = bp.Groups.AddToArray(FeatureGroup.Racial);
                        bp.IsClassFeature = true;
                        bp.SetFeatures(NoAlternateTrait, minosTrait);

                    });

                //add heritage selection to race
                var HumanRace = BlueprintTools.GetBlueprint<BlueprintRace>("0a5d473ead98b0646b94495af250fdc4");
                HumanRace.m_Features = HumanRace.m_Features.AddToArray(heritageSelection.ToReference<BlueprintFeatureBaseReference>());
            }

            private static void CreateFeatures(out BlueprintFeature destinyBeyondbirth, out BlueprintFeature minosHorns, 
                out BlueprintFeature beeingBuff, out BlueprintFeature gregarious, out BlueprintFeature emphatic, out BlueprintFeature minosweaponfamiliarity)
            {
                destinyBeyondbirth = BlueprintTools.GetBlueprint<BlueprintFeature>("325f078c584318849bfe3da9ea245b9d");
                var gore1d6 = BlueprintTools.GetBlueprint<BlueprintItemWeapon>("daf4ab765feba8548b244e174e7af5be");
                minosHorns = Helpers.CreateBlueprint<BlueprintFeature>(MC.mc, "MinosHorns",
                    bp =>
                    {
                        bp.SetName(MC.mc, "Minos horns");
                        bp.SetDescription(MC.mc, "Minos have horns that deals 1d6 damage.");
                        bp.AddComponent<AddAdditionalLimb>(
                            c =>
                            {
                                c.name = "Minos horns";
                                c.m_Weapon = gore1d6.ToReference<BlueprintItemWeaponReference>();
                            });
                        bp.IsClassFeature = true;

                    });
                beeingBuff = Helpers.CreateBlueprint<BlueprintFeature>(MC.mc, "PowerfulBuild",
                    bp =>
                    {
                        bp.SetName(MC.mc, "Powerful build");
                        bp.SetDescription(MC.mc, "Minos are buff AF.");
                        bp.AddComponent<MeleeWeaponSizeChange>(
                            c =>
                            {
                                c.SizeCategoryChange = 1;
                            });
                        bp.AddComponent<AddStatBonus>(
                            c =>
                            {
                                c.Descriptor = Kingmaker.Enums.ModifierDescriptor.Racial;
                                c.Stat = Kingmaker.EntitySystem.Stats.StatType.AdditionalCMB;
                                c.Value = 2;
                            });
                        bp.AddComponent<AddStatBonus>(
                            c =>
                            {
                                c.Descriptor = Kingmaker.Enums.ModifierDescriptor.Racial;
                                c.Stat = Kingmaker.EntitySystem.Stats.StatType.AdditionalCMD;
                                c.Value = 2;
                            });
                        bp.AddComponent<AddPartyEncumbrance>(
                            c =>
                            {
                                c.Value = 300;
                            });
                        bp.AddComponent<ChangeVisualSize>(c =>
                        {
                            c.Multiplier = 1.3f;
                        });
                    });
                gregarious = Helpers.CreateBlueprint<BlueprintFeature>(MC.mc, "Gregarious",
                    bp =>
                    {
                        bp.SetName(MC.mc, "Gregarious");
                        bp.SetDescription(MC.mc, "Minos gain a +2 racial bonus on Persuasion (Diplomacy) checks.");
                    });
                emphatic = Helpers.CreateBlueprint<BlueprintFeature>(MC.mc, "Emphatic",
                   bp =>
                   {
                       bp.SetName(MC.mc, "Emphatic");
                       bp.SetDescription(MC.mc, "Persuasion is always considered a class skill for Minos.");
                   });
                minosweaponfamiliarity =  Helpers.CreateBlueprint<BlueprintFeature>(MC.mc, "minosweaponfamiliarity",
                   bp =>
                   {
                       bp.SetName(MC.mc, "Weapon familiarity");
                       bp.SetDescription(MC.mc, "Minos are proficient with greataxes and earth breakers.");
                       bp.AddComponent<AddProficiencies>(
                           c => {
                               c.WeaponProficiencies = c.WeaponProficiencies.AddToArray(Kingmaker.Enums.WeaponCategory.EarthBreaker)
                               .AddToArray(Kingmaker.Enums.WeaponCategory.Greataxe);
                           });
                   });
            }
        }
    }
}
