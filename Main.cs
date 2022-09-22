using HarmonyLib;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.CharGen;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.Blueprints.Root;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.Designers.Mechanics.Buffs;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.DLC;
using Kingmaker.ResourceLinks;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases;
using Kingmaker.UI.MVVM._VM.CharGen.Phases;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.UnitLogic.Mechanics.Components;
using Kingmaker.Utility;
using Kingmaker.View;
using Kingmaker.Visual.CharactersRigidbody;
using Kingmaker.Visual.CharacterSystem;
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
            var myOriginalMethods = harmony.GetPatchedMethods();
            foreach (var method in myOriginalMethods)
            {
                MC.mc.Logger.Log("Patched " + method.Name);
            };
            return true;
        }
        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            Enabled = value;
            return true;
        }
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        public static class BlueprintsCache_Init_Patch
        {
            static bool Initialized = false;
            static BlueprintFeature destinyBeyondbirth, minosHorns, beeingBuff, gregarious, emphatic, Minosweaponfamiliarity;
            static BlueprintRace HumanRace = BlueprintTools.GetBlueprint<BlueprintRace>("0a5d473ead98b0646b94495af250fdc4");
            static BlueprintRace TieflingRace = BlueprintTools.GetBlueprint<BlueprintRace>("5c4e42124dc2b4647af6e36cf2590500");

            [HarmonyPriority(Priority.First)]
            [HarmonyPostfix]
            static void CreateNewBlueprints()
            {
                if (Initialized)
                    return;
                Initialized = true;
                PreloadResources();
                var minosRace = AddNewRace();
                AddHeritages(minosRace);
            }

            public static void PreloadResources()
            {
                //List<string> resources = new List<string>() {
                //"bb6988a21733fad4296ad22537248fea",
                //"6a5ae89de41b6b149856718b6058168f",
                //"7b27b2063f548794e845e0ee8ea7b91b",
                //"4a4afd8ea46ff2e438bb078495bd3531",
                //"49b32d9af554e6742bed805d80ccde93",
                //"f7fdd6a03bc43da4da6d913a57f28c7c",
                //"a88d1147b9c76364db8d34d956bb6fcb",
                //"d1dcf6b4e326a9d459ee5b2e5b7b7cbc",
                //"6ae0e2be0e8f9f54981033b4a61f11ed",
                //"b354195728faa79449de9b3197f3b449",
                //"a8b95db20e630214dabfb79424494c34",
                //"cdc8632dd9b07744eb7baa143e39bf06",
                //"a957a3408601f9046b56015f6c40a8d3"};
                //foreach (var item in resources)
                //{
                //    
                //    ResourcesLibrary.PreloadResource<EquipmentEntity>(item);
                //}
                KingmakerEquipmentEntity humanbody = BlueprintTools.GetBlueprint<KingmakerEquipmentEntity>("3bdac1aeeac9158489bb53916b303271");
                humanbody.m_FemaleArray[0].Load();
                MC.Log($"MHeadCount: {TieflingRace.MaleOptions.m_Heads.Length}");
                foreach (var item in TieflingRace.MaleOptions.m_Heads)
                {
                    MC.mc.Logger.Log("Preloading " + item.AssetId);
                    item.Load();
                }
                MC.Log($"FHeadCount: {TieflingRace.FemaleOptions.m_Heads.Length}");
                foreach (var item in TieflingRace.FemaleOptions.m_Heads)
                {
                    MC.mc.Logger.Log("Preloading " + item.AssetId);

                    item.Load();
                }
                foreach (var item in TieflingRace.MaleOptions.Hair)
                {
                    item.Load();
                }
                foreach (var item in TieflingRace.FemaleOptions.Hair)
                {
                    item.Load();
                }
                var preset = TieflingRace.Presets[0];
                {
                    MC.Log($"MSkin: {preset.m_Skin.Get().m_MaleArray.Length}");

                    foreach (var item in preset.m_Skin.Get().m_MaleArray)
                    {
                        MC.mc.Logger.Log("Preloading " + item.AssetId);

                        item.Load();
                    }
                    MC.Log($"FSkin: {preset.m_Skin.Get().m_FemaleArray.Length}");

                    foreach (var item in preset.m_Skin.Get().m_FemaleArray)
                    {
                        MC.mc.Logger.Log("Preloading " + item.AssetId);

                        item.Load();
                    }
                }

            }

            private static BlueprintRace AddNewRace()
            {
                BlueprintRaceVisualPreset tiefling_standard = BlueprintTools.GetBlueprint<BlueprintRaceVisualPreset>("4bbf7e0e8d2f0e84e8236bea2df185ff");
                BlueprintRaceVisualPreset tiefling_thin = BlueprintTools.GetBlueprint<BlueprintRaceVisualPreset>("4d9124908caec8145b733ecbd2896b23");
                BlueprintRaceVisualPreset tiefling_thicc = BlueprintTools.GetBlueprint<BlueprintRaceVisualPreset>("8b485a757f883734585da9b8b816d1d6");
                KingmakerEquipmentEntity tieflingskin = BlueprintTools.GetBlueprint<KingmakerEquipmentEntity>("d38a9d4a2cfe38e499a72b583317f993");
                KingmakerEquipmentEntity minosskin = Helpers.CreateBlueprint<KingmakerEquipmentEntity>(MC.mc, "MinosSkin",
                    bp =>
                    {
                        bp.m_MaleArray = new Kingmaker.ResourceLinks.EquipmentEntityLink[] {
                            new Kingmaker.ResourceLinks.EquipmentEntityLink(){AssetId="3DC74FD208974CDDA9B53864635CC1E2".ToLower()},
                        new Kingmaker.ResourceLinks.EquipmentEntityLink()

                        {AssetId="a15b0963b1a047f7b151a1e4a0f38281".ToLower() } };
                        bp.m_FemaleArray = new Kingmaker.ResourceLinks.EquipmentEntityLink[] {
                            new Kingmaker.ResourceLinks.EquipmentEntityLink(){AssetId="9F18B0C4E57646DDA309DABD030A4B79".ToLower()},
                        new Kingmaker.ResourceLinks.EquipmentEntityLink()
                        //{ AssetId=tieflingskin.m_FemaleArray[1].AssetId } };
                        {AssetId="a15b0963b1a047f7b151a1e4a0f38281".ToLower() } };

                    });
                var standardpreset = Helpers.CreateBlueprint<BlueprintRaceVisualPreset>(MC.mc, "minosStandardVisual",
                    bp =>
                    {
                        bp.MaleSkeleton = tiefling_standard.MaleSkeleton;
                        bp.FemaleSkeleton = tiefling_standard.FemaleSkeleton;
                        bp.m_Skin = minosskin.ToReference<KingmakerEquipmentEntityReference>();
                    });
                var thiccpreset = Helpers.CreateBlueprint<BlueprintRaceVisualPreset>(MC.mc, "minosThiccVisual",
                   bp =>
                   {
                       bp.MaleSkeleton = tiefling_thicc.MaleSkeleton;
                       bp.FemaleSkeleton = tiefling_thicc.FemaleSkeleton;
                       bp.m_Skin = minosskin.ToReference<KingmakerEquipmentEntityReference>();
                   });
                var thinpreset = Helpers.CreateBlueprint<BlueprintRaceVisualPreset>(MC.mc, "minosThinVisual",
                   bp =>
                   {
                       bp.MaleSkeleton = tiefling_thin.MaleSkeleton;
                       bp.FemaleSkeleton = tiefling_thin.FemaleSkeleton;
                       bp.m_Skin = minosskin.ToReference<KingmakerEquipmentEntityReference>();
                   });
                CreateFeatures(out destinyBeyondbirth, out minosHorns, out beeingBuff, out gregarious, out emphatic, out Minosweaponfamiliarity);
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
                        bp.m_Features = bp.m_Features.AddRangeToArray(new BlueprintFeatureBaseReference[] {
                        minosHorns.ToReference<BlueprintFeatureBaseReference>(),
                        beeingBuff.ToReference<BlueprintFeatureBaseReference>(),
                        gregarious.ToReference<BlueprintFeatureBaseReference>(),
                        emphatic.ToReference<BlueprintFeatureBaseReference>(),
                        Minosweaponfamiliarity.ToReference<BlueprintFeatureBaseReference>()
                    });
                        bp.m_Presets = new BlueprintRaceVisualPresetReference[] {
                        standardpreset.ToReference<BlueprintRaceVisualPresetReference>(),
                        thiccpreset.ToReference<BlueprintRaceVisualPresetReference>(),
                        thinpreset.ToReference<BlueprintRaceVisualPresetReference>() };
                        bp.MaleOptions = new CustomizationOptions()
                        {
                            m_Heads =
                            new Kingmaker.ResourceLinks.EquipmentEntityLink[]
                            {
                                MakeEELink("8F54696DE3524E67BED4FBF41BAE13BA".ToLower()),
                                MakeEELink("2F97A2D7186B4DA08737DADDF29C5C40".ToLower()),
                                MakeEELink("38E57963F5D8430D8D0F0F5EB809F72D".ToLower()),
                                MakeEELink("3F3C16C2626A446BBB8932C3539F4CB9".ToLower()),
                                MakeEELink("7E8973B0DC58464F851B8ED610175E08".ToLower()),

                            },
                            m_Eyebrows = TieflingRace.MaleOptions.m_Eyebrows,
                            m_Hair = new EquipmentEntityLink[] {
                                MakeEELink( "Hair5AFC439F4ED0861D3E0DDBB57893".ToLower()),
                                MakeEELink( "HairA33789FA4F009EF55A3F08AD4D7A".ToLower()),
                                MakeEELink( "HairBF9CE7304284B323B2A739B58F10".ToLower()),
                                MakeEELink( "Hair06F385AE4927B303100A82D1EA2B".ToLower()),
                                MakeEELink( "HairA1A81EDE4721918787B1BEB2BB85".ToLower()),
                                MakeEELink( "Hair00A17B25478881AC7020B47BA079".ToLower()),
                                MakeEELink( "HairCA2D95194F059836FB431C9F305C".ToLower()),
                                MakeEELink( "HairA325D29940F4B830EF2EBBCD6A28".ToLower()),
                            },
                            Horns = TieflingRace.MaleOptions.Horns
                        };


                        bp.FemaleOptions = new CustomizationOptions()
                        {
                            m_Heads = new Kingmaker.ResourceLinks.EquipmentEntityLink[]
                            {
                            MakeEELink("7E54EC353B6B4F6AB71BAAC3EBBDDFBD".ToLower()),
                            MakeEELink("DB683CE3EF724EF6929CE723C5BA986D".ToLower()),
                            MakeEELink("7DCBF6B10FC44FF79AB0C74F8E5899E0".ToLower()),
                            MakeEELink("E24AFA3ABEE0418E833E77239DDE8C1A".ToLower()),
                            MakeEELink("0C58485A0FD3482DBCA901E8508B4CD3".ToLower()),

                            },


                            m_Eyebrows = TieflingRace.FemaleOptions.m_Eyebrows,
                            m_Hair = new EquipmentEntityLink[]
                            {
                             MakeEELink("Hair136618EC4F4EA6680C1767AD8351".ToLower() ),
                             MakeEELink("Hair7A9700144F9F838A51A91958DCC9".ToLower() ),
                             MakeEELink("Hair8FDB551C4ABCB7A3E3CFF5F1BDC5".ToLower() ),
                             MakeEELink("Hair74A96C214B89A36EF1A7C88FABFF".ToLower() ),
                             MakeEELink("Hair93629CBB473C9C9A104978AAF127".ToLower() ),
                             MakeEELink("Hair05CD4EDC4560A85499CE2C75E9AD".ToLower() ),
                             MakeEELink("Hair3B71AF4F487E916EB7B8A5E2C39F".ToLower() ),
                             MakeEELink("HairA029D7214C949BFB4B391B846AC2".ToLower() ),
                             MakeEELink("HairA325D29940F4B830EF2EBBCD6A28".ToLower()),
                            },
                            Horns = TieflingRace.FemaleOptions.Horns
                        }; ;
                        bp.RaceId = Race.Catfolk;
                        bp.SoundKey = "Human";
                        bp.MaleSpeedSettings = HumanRace.MaleSpeedSettings;
                        bp.FemaleSpeedSettings = HumanRace.FemaleSpeedSettings;



                    });
                BlueprintRoot.Instance.Progression.m_CharacterRaces = BlueprintRoot.Instance.Progression.m_CharacterRaces.AddToArray(MinosRace.ToReference<BlueprintRaceReference>());
                return MinosRace;
            }

            private static EquipmentEntityLink MakeEELink(string assetId)
            {
                return new Kingmaker.ResourceLinks.EquipmentEntityLink()
                {
                    AssetId = assetId,
                    m_Handle = new Kingmaker.ResourceManagement.BundledResourceHandle<EquipmentEntity>()
                    {
                        m_AssetId = assetId,
                        Object = ResourcesLibrary.TryGetResource<EquipmentEntity>(assetId, false, true),
                        m_Held = true,
                    },
                };
            }

            private static void AddHeritages(BlueprintRace minosRace)
            {
                var extrafeat = BlueprintTools.GetBlueprint<BlueprintFeatureSelection>("247a4068296e8be42890143f451b4b45");
                //heritage-none
                var NoAlternateTrait = Helpers.CreateBlueprint<BlueprintFeature>(MC.mc, "NoAlternateTrait", bp =>
                {
                    bp.IsClassFeature = true;
                    bp.Ranks = 1;
                    bp.Groups = new FeatureGroup[] { FeatureGroup.Racial };
                    bp.HideInUI = true;
                    bp.HideInCharacterSheetAndLevelUp = true;
                    bp.SetName(MC.mc, "Basic");
                    bp.SetDescription(MC.mc, "Your typical minos.");
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
                });
                //heritage-minos
                var SmallminosTrait = Helpers.CreateBlueprint<BlueprintFeature>(MC.mc, "SmallBuildHeritage",
                   bp =>
                   {
                       bp.IsClassFeature = true;
                       bp.Ranks = 1;
                       bp.Groups = new FeatureGroup[] { FeatureGroup.Racial };
                       bp.HideInUI = true;
                       bp.HideInCharacterSheetAndLevelUp = true;
                       bp.SetName(MC.mc, "Small build");
                       bp.SetDescription(MC.mc, "Although it doesn't happen often due to the size difference between minos and other " +
                           "humanoids, some minos have a parent from another race (usually human) and are smaller than ordinary minos, " +
                           "but are also more adaptable. They select one extra feat at 1st level, and gain a +2 racial bonus to " +
                           "Strength. This racial trait replaces Powerful Build and the minos' usual racial ability score modifiers.");
                       bp.AddComponent<RemoveFeatureOnApply>(
                           c =>
                           {
                               c.m_Feature = beeingBuff.ToReference<BlueprintUnitFactReference>();
                           });
                       bp.AddComponent<AddStatBonus>(
                         c =>
                         {
                             c.Descriptor = Kingmaker.Enums.ModifierDescriptor.Racial;
                             c.Stat = Kingmaker.EntitySystem.Stats.StatType.Strength;
                             c.Value = 2;
                         });
                       bp.AddComponent<ChangeVisualSize>(c => { c.ExtraSizePercent = 0.1f; });

                   });
                var charismaBonus = Helpers.CreateBlueprint<BlueprintFeature>(MC.mc, "TimidCharming",
                    bp =>
                    {
                        bp.SetName(MC.mc, "Charming");
                        bp.SetDescription(MC.mc, "Some minos are born more timid than others, but are also more gifted in their mental " +
                        "pursuits. They gain a +2 racial bonus to Charisma. This racial trait replaces Gregarious, Minos Horns, " +
                        "and Minos Weapon Familiarity.");

                        bp.AddComponent<RemoveFeatureOnApply>(
                         c =>
                         {
                             c.m_Feature = gregarious.ToReference<BlueprintUnitFactReference>();
                         });
                        bp.AddComponent<RemoveFeatureOnApply>(
                           c =>
                           {
                               c.m_Feature = minosHorns.ToReference<BlueprintUnitFactReference>();
                           });
                        bp.AddComponent<RemoveFeatureOnApply>(
                           c =>
                           {
                               c.m_Feature = Minosweaponfamiliarity.ToReference<BlueprintUnitFactReference>();
                           });
                        bp.AddComponent<AddStatBonus>(
                      c =>
                      {
                          c.Descriptor = Kingmaker.Enums.ModifierDescriptor.Racial;
                          c.Stat = Kingmaker.EntitySystem.Stats.StatType.Charisma;
                          c.Value = 2;
                      });
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

                    });
                var wisdomBonus = Helpers.CreateBlueprint<BlueprintFeature>(MC.mc, "TimidContemplating",
                  bp =>
                  {
                      bp.SetName(MC.mc, "Contemplating");
                      bp.SetDescription(MC.mc, "Some minos are born more timid than others, but are also more gifted in their mental " +
                      "pursuits. They gain a +2 racial bonus to  Wisdom. This racial trait replaces Gregarious, Minos Horns, " +
                      "and Minos Weapon Familiarity.");

                      bp.AddComponent<RemoveFeatureOnApply>(
                       c =>
                       {
                           c.m_Feature = gregarious.ToReference<BlueprintUnitFactReference>();
                       });
                      bp.AddComponent<RemoveFeatureOnApply>(
                         c =>
                         {
                             c.m_Feature = minosHorns.ToReference<BlueprintUnitFactReference>();
                         });
                      bp.AddComponent<RemoveFeatureOnApply>(
                         c =>
                         {
                             c.m_Feature = Minosweaponfamiliarity.ToReference<BlueprintUnitFactReference>();
                         });
                      bp.AddComponent<AddStatBonus>(
                    c =>
                    {
                        c.Descriptor = Kingmaker.Enums.ModifierDescriptor.Racial;
                        c.Stat = Kingmaker.EntitySystem.Stats.StatType.Wisdom;
                        c.Value = 2;
                    });
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

                  });
                var intBonus = Helpers.CreateBlueprint<BlueprintFeature>(MC.mc, "TimidGenius",
                bp =>
                {
                    bp.SetName(MC.mc, "Genius");
                    bp.SetDescription(MC.mc, "Some minos are born more timid than others, but are also more gifted in their mental " +
                    "pursuits. They gain a +2 racial bonus to  Intelligence. This racial trait replaces Gregarious, Minos Horns, " +
                    "and Minos Weapon Familiarity.");

                    bp.AddComponent<RemoveFeatureOnApply>(
                     c =>
                     {
                         c.m_Feature = gregarious.ToReference<BlueprintUnitFactReference>();
                     });
                    bp.AddComponent<RemoveFeatureOnApply>(
                       c =>
                       {
                           c.m_Feature = minosHorns.ToReference<BlueprintUnitFactReference>();
                       });
                    bp.AddComponent<RemoveFeatureOnApply>(
                       c =>
                       {
                           c.m_Feature = Minosweaponfamiliarity.ToReference<BlueprintUnitFactReference>();
                       });
                    bp.AddComponent<AddStatBonus>(
                          c =>
                          {
                              c.Descriptor = Kingmaker.Enums.ModifierDescriptor.Racial;
                              c.Stat = Kingmaker.EntitySystem.Stats.StatType.Intelligence;
                              c.Value = 2;
                          });
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

                });
                var TimidMinosTrait = Helpers.CreateBlueprint<BlueprintFeatureSelection>(MC.mc, "TimidHeritage",
                   bp =>
                   {
                       bp.IsClassFeature = true;
                       bp.Ranks = 1;
                       bp.Groups = new FeatureGroup[] { FeatureGroup.Racial };
                       bp.HideInUI = true;
                       bp.HideInCharacterSheetAndLevelUp = true;
                       bp.SetName(MC.mc, "Timid");
                       bp.SetDescription(MC.mc, "Some minos are born more timid than others, but are also more gifted in their mental " +
                           "pursuits. They gain a +2 racial bonus to one mental ability score of their choice (Intelligence, Wisdom, " +
                           "or Charisma). This racial trait replaces Gregarious, Minos Horns, and Minos Weapon Familiarity.");
                       bp.AddFeatures(intBonus, wisdomBonus, charismaBonus);

                   });
                //create heritage selection 
                BlueprintFeatureSelection heritageSelection = Helpers.CreateBlueprint<BlueprintFeatureSelection>(MC.mc, "MinosHeritageSelection",
                    bp =>
                    {
                        bp.SetName(MC.mc, "Racial heritage");
                        bp.SetDescription(MC.mc, "Various places and living conditions create sub-races different from their peers. " +
                            "They gain unique racial traits in exchange for losing some of the usual ones.");
                        bp.Groups = bp.Groups.AddToArray(FeatureGroup.Racial);
                        bp.IsClassFeature = true;
                        bp.SetFeatures(NoAlternateTrait, SmallminosTrait, TimidMinosTrait);

                    });

                //add heritage selection to race

                minosRace.m_Features = minosRace.m_Features.AddToArray(heritageSelection.ToReference<BlueprintFeatureBaseReference>());
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
                        bp.SetDescription(MC.mc, "Minos have a powerful build that grants them a +2 bonus to combat maneuver checks and combat maneuver defense, 300 lbs. increased carrying capacity, and increased damage with melee weapons as if they were one size larger than they actually are.");
                        bp.IsClassFeature = true;
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
                            c.ExtraSizePercent = 0.3f;
                        });
                    });
                gregarious = Helpers.CreateBlueprint<BlueprintFeature>(MC.mc, "Gregarious",
                    bp =>
                    {
                        bp.SetName(MC.mc, "Gregarious");
                        bp.SetDescription(MC.mc, "Minos gain a +2 racial bonus on Persuasion (Diplomacy) checks.");
                        bp.IsClassFeature = true;
                        bp.AddComponent<AddStatBonus>(
                        c =>
                        {
                            c.Descriptor = Kingmaker.Enums.ModifierDescriptor.Racial;
                            c.Stat = Kingmaker.EntitySystem.Stats.StatType.SkillPersuasion;
                            c.Value = 2;
                        });
                    });
                emphatic = Helpers.CreateBlueprint<BlueprintFeature>(MC.mc, "Emphatic",
                   bp =>
                   {
                       bp.SetName(MC.mc, "Emphatic");
                       bp.SetDescription(MC.mc, "Persuasion is always considered a class skill for Minos.");
                       bp.IsClassFeature = true;
                       bp.AddComponent<AddClassSkill>(c => { c.Skill = Kingmaker.EntitySystem.Stats.StatType.SkillPersuasion; });

                   });
                minosweaponfamiliarity = Helpers.CreateBlueprint<BlueprintFeature>(MC.mc, "minosweaponfamiliarity",
                   bp =>
                   {
                       bp.SetName(MC.mc, "Minos weapon familiarity");
                       bp.SetDescription(MC.mc, "Minos are proficient with greataxes and earth breakers.");
                       bp.AddComponent<AddProficiencies>(
                           c =>
                           {
                               c.WeaponProficiencies = c.WeaponProficiencies.AddToArray(Kingmaker.Enums.WeaponCategory.EarthBreaker)
                               .AddToArray(Kingmaker.Enums.WeaponCategory.Greataxe);
                           });
                   });
            }
        }
    }
}
