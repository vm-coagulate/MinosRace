using HarmonyLib;
using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.Modding;
using Kingmaker.ResourceLinks;
using Kingmaker.UI.MVVM._PCView.CharGen;
using Kingmaker.UI.MVVM._PCView.CharGen.Phases.Race;
using Kingmaker.UnitLogic;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.UnitLogic.Parts;
using Kingmaker.Visual.CharacterSystem;
using MinosRace.Context;
using Owlcat.Runtime.Core.Utils;
using Owlcat.Runtime.UI.Controls.Button;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabletopTweaks.Core.Utilities;
using UnityEngine;
using UnityEngine.UI.Extensions;
using static Kingmaker.Blueprints.ResourcesLibrary;

namespace MinosRace
{
    [HarmonyPatch(typeof(DollState))]
    public static class DollStatePatcher
    {
        public static string minosTailID = "a15b0963b1a047f7b151a1e4a0f38281";
        public static string tieflingTailID = "d15dbf2912cd6f94492a9e1053aa0ebd";
        public static List<Color> HardcodedColors = new List<Color>()
        {
            //1 color for each human hair color
            new Color( 30/255.0f,30/255.0f,30/255.0f ),      //0
            new Color( 48/255.0f,44/255.0f,38/255.0f ),      //1
            new Color( 66/255.0f,60/255.0f,55/255.0f ),      //2 2
            new Color( 44/255.0f,32/255.0f,27/255.0f ),      // 3
            new Color( 71/255.0f,52/255.0f,45/255.0f ),      // 4
            new Color( 104/255.0f,77/255.0f,52/255.0f ),     // 5
            new Color( 150/255.0f,126/255.0f,100 /255.0f),       // 6
            new Color( 200/255.0f,170/255.0f,140 /255.0f),       // 7
            new Color( 233/255.0f,222/255.0f,194 /255.0f),   //8
            new Color( 124/255.0f,55/255.0f,5/255.0f ),      //9
            new Color( 214/255.0f,125/255.0f,7/255.0f),
            new Color( 225/255.0f,165/255.0f,100 /255.0f),
            new Color( 233/255.0f,196/255.0f,166/255.0f ),
            new Color( 70/255.0f,74/255.0f,79 /255.0f),
            new Color( 121/255.0f,126/255.0f,134 /255.0f),
            new Color( 208/255.0f,105/255.0f,96/255.0f ),
            new Color( 184/255.0f,63/255.0f,53/255.0f ),
            new Color( 100/255.0f,27/255.0f,27/255.0f ),
            new Color( 214/255.0f,214/255.0f,214/255.0f  ),////
            new Color( 206/255.0f,196/255.0f,184 /255.0f),////
            new Color( 188/255.0f,200/255.0f,210 /255.0f),////

        };
        [HarmonyPatch(nameof(DollState.GetHairEntities)), HarmonyPostfix]
        public static void HairEntitiesPostfix_patch(ref List<DollState.EEAdapter> __result, DollState __instance)
        {
            //grab skin links, check for tail
            MC.mc.Logger.Log("hairentities ");
            var skinlinks = __instance.RacePreset?.Skin?.GetLinks(__instance.Gender, __instance.RacePreset.RaceId);
            if (skinlinks != null)
            {
                if (skinlinks.Any(p => p.AssetId == minosTailID))
                {
                    var tt = skinlinks.Where(p => p.AssetId == minosTailID).Single();
                    var ds = new DollState.EEAdapter(tt);
                    if (ds.Load() == null)
                    {
                        MC.mc.Logger.Log("Adding tail to hair entities FAIL!");

                    }
                    else
                    {
                        MC.mc.Logger.Log("Adding tail to hair entities");
                        __result.Add(ds);

                    }
                }
            }
        }
        [HarmonyPatch(nameof(DollState.GetSkinEntities)), HarmonyPostfix]
        public static void SkinEntitiesPostfix_patch(ref List<DollState.EEAdapter> __result, DollState __instance)
        {
            MC.Log("skinentities ");
            if (__result.Any(p => p.AssetId == minosTailID))
            {
                MC.mc.Logger.Log("Removing tail from skin entities");

                __result.RemoveAll(p => p.AssetId == minosTailID);

            }
        }
        [HarmonyPatch(nameof(DollState.ApplyRamps), typeof(Character))]
        [HarmonyPostfix]
        public static void AplyRamp_PostfixPatch(Character character, DollState __instance)
        {
            MC.Log("ApplyRamp " + character.name);
            var rampindex = character.m_RampIndices.Where(p => p.EquipmentEntity.name == minosTailID).FirstOrDefault();
            if (rampindex != null)
            {

                //grab color 
                var color = HardcodedColors[rampindex.PrimaryIndex];
                MC.Log($"Recoloring tail to {rampindex.PrimaryIndex}");
                //apply color to tail
                rampindex.EquipmentEntity.OutfitParts[0].m_Material.color = color;
                /*MC.Log($"color:{rampindex.EquipmentEntity.OutfitParts[0].m_Material.color.r}:" +
                    $"{rampindex.EquipmentEntity.OutfitParts[0].m_Material.color.g}:" +
                    $"{rampindex.EquipmentEntity.OutfitParts[0].m_Material.color.b}");*/

            }
        }
    }

    [HarmonyPatch(typeof(AssetBundle))]
    public static class AssetPatcher
    {
        static CharacterColorsProfile minosbodycolorprofile = (CharacterColorsProfile)ScriptableObject.CreateInstance(typeof(CharacterColorsProfile));
        static string tieflingTailID = "d15dbf2912cd6f94492a9e1053aa0ebd";
        static string aHumanHairID = "ee3945f41269aed4b9fcb6304c3fda79";//"d174619e428101e41b5675bd6286b1d4"//
        static string humanFBodyID = "bb6988a21733fad4296ad22537248fea";
        static string humanMBodyID = "e7c86166041c1e04a92276abdab68afa";

        static List<Texture2D> primaryRamps = null;
        static List<Texture2D> primaryHairRamps = null;

        public static Dictionary<string, EquipmentEntity> minosAssets = new Dictionary<string, EquipmentEntity>();

        public static Dictionary<string, string> guids = new Dictionary<string, string>()
        {
           // {"6a5ae89de41b6b149856718b6058168f",  "9F18B0C4E57646DDA309DABD030A4B79".ToLower() }, //f_body
            //{"7b27b2063f548794e845e0ee8ea7b91b",  "7E54EC353B6B4F6AB71BAAC3EBBDDFBD".ToLower()  },
            //{"4a4afd8ea46ff2e438bb078495bd3531",  "DB683CE3EF724EF6929CE723C5BA986D".ToLower()  },
            //{"49b32d9af554e6742bed805d80ccde93",  "7DCBF6B10FC44FF79AB0C74F8E5899E0".ToLower()  },
            //{"f7fdd6a03bc43da4da6d913a57f28c7c",  "E24AFA3ABEE0418E833E77239DDE8C1A".ToLower()  },
            //{"a88d1147b9c76364db8d34d956bb6fcb",  "0C58485A0FD3482DBCA901E8508B4CD3".ToLower()  },
            ////{"d1dcf6b4e326a9d459ee5b2e5b7b7cbc",  "3DC74FD208974CDDA9B53864635CC1E2".ToLower()  }, //m_body
            //{"6ae0e2be0e8f9f54981033b4a61f11ed",  "8F54696DE3524E67BED4FBF41BAE13BA".ToLower()  },
            //{"b354195728faa79449de9b3197f3b449",  "2F97A2D7186B4DA08737DADDF29C5C40".ToLower()  },
            //{"a8b95db20e630214dabfb79424494c34",  "38E57963F5D8430D8D0F0F5EB809F72D".ToLower()  },
            //{"cdc8632dd9b07744eb7baa143e39bf06",  "3F3C16C2626A446BBB8932C3539F4CB9".ToLower()   },
            //{"a957a3408601f9046b56015f6c40a8d3",  "7E8973B0DC58464F851B8ED610175E08".ToLower()   },
            {"d15dbf2912cd6f94492a9e1053aa0ebd",  "a15b0963b1a047f7b151a1e4a0f38281".ToLower()   },//tail
            //hairs
             {"d174619e428101e41b5675bd6286b1d4",   "Hair5AFC439F4ED0861D3E0DDBB57893".ToLower() },
             {"1730d9ec670411b49b6e4c0222abbd25" ,  "HairA33789FA4F009EF55A3F08AD4D7A".ToLower() },
             {"e60c9687e2e852143b0ddd32d4d65c0b" ,  "HairBF9CE7304284B323B2A739B58F10".ToLower() },
             {"50eac92ba30862940be4f70d329d070a" ,  "Hair06F385AE4927B303100A82D1EA2B".ToLower() },
             {"5509e7a1d63e1b14097292be114e4d00" ,  "HairA1A81EDE4721918787B1BEB2BB85".ToLower() },
             {"05eb3b064eb501149b1715156850bb8f" ,  "Hair00A17B25478881AC7020B47BA079".ToLower() },
             {"58115a09ef40db046adc4bb99c1ec5b8" ,  "HairCA2D95194F059836FB431C9F305C".ToLower() },
             {"b85db19d7adf6aa48b5dd2bb7bfe1502" ,  "HairA325D29940F4B830EF2EBBCD6A28".ToLower() },
             {"afa22656ed5030c4ba273583ba2b3a16" ,  "Hair136618EC4F4EA6680C1767AD8351".ToLower() },
             {"d71d2e53fce0f1d4baad8b20c8266676" ,  "Hair7A9700144F9F838A51A91958DCC9".ToLower()  },
             {"195bf2c26a914dc439121c5fdbded81f" ,  "Hair8FDB551C4ABCB7A3E3CFF5F1BDC5".ToLower() },
             {"c81febf186ba543438e5dec7d1c9bcf7" ,  "Hair74A96C214B89A36EF1A7C88FABFF".ToLower() },
             {"ee38a6b141c8ac54697fba55ce144094" ,  "Hair93629CBB473C9C9A104978AAF127".ToLower() },
             {"103e1f478c298a748bb13445840bc4c5" ,  "Hair05CD4EDC4560A85499CE2C75E9AD".ToLower() },
             {"01adb2fc579b26a419a6ea83867c824b" ,  "Hair3B71AF4F487E916EB7B8A5E2C39F".ToLower() },
             {"d90a0bf179ad5884a98092b58d8f76ad" ,  "HairA029D7214C949BFB4B391B846AC2".ToLower() },

        };
        private static bool tailpatched;

        private static bool minosAssetRequested=false;
        [HarmonyPatch(nameof(AssetBundle.LoadAsset), typeof(string), typeof(Type)), HarmonyPrefix]
        public static bool LoadAsset_Prefix(ref string name, Type type,
            ref UnityEngine.Object __result)
        {
            minosAssetRequested = false;
            // MC.Log("LoadAsset prefix");
            foreach (var item in guids.Keys)
            {
                if (guids[item] == name)
                {
                    MC.Log($"minos asset requested, changed {name} to {item}");
                    name = item;
                    minosAssetRequested = true;
                }
            }
            //MC.Log("Loadasset prefix exit");

            return true;
            //try
            //{
            //    if (guids.Values.Any(p => p == name))
            //    {
            //        MC.Log("loading minos resource");
            //        __result = Res.Get<EquipmentEntity>(name);
            //        if (__result != null)
            //            return true;
            //    }
            //    return false;
            //}
            //catch (Exception e)
            //{
            //    MC.Log(e.Message);
            //    return false;
            //}

        }
        //public Object LoadAsset(string name, Type type)
        [HarmonyPatch(nameof(AssetBundle.LoadAsset), typeof(string), typeof(Type)), HarmonyPostfix]
        public static void LoadAsset(string name, Type type, ref UnityEngine.Object __result)
        {
            if(minosAssetRequested)
            {
                MC.Log("Minos asset requested postfix");
                name = guids[name];
                minosAssetRequested = false;
            }
            if (__result == null)
            {
                MC.Log($"LoadAsset {name} failed");
                return;
            }
            if (!(__result is EquipmentEntity))
            {
                return;

            }

            if (name == aHumanHairID)
            {
                MC.mc.Logger.Log($"Caching hair ramp: {name}");
                var humanHair = __result as EquipmentEntity;
                primaryHairRamps = new List<Texture2D>();
                primaryHairRamps.AddRange(humanHair.PrimaryRamps);
                if (minosAssets.ContainsKey(guids[tieflingTailID]))
                {
                    if (!tailpatched)
                    {
                        ModifyTail(minosAssets[guids[tieflingTailID]], null);
                        PatchTail(minosAssets[guids[tieflingTailID]], primaryHairRamps);
                        tailpatched = true;
                    }
                }
                foreach (var item in minosAssets.Values)
                {
                    if (item.name.StartsWith("hair"))
                    {
                        PatchEE(item);
                    }
                }
            }
            //if (name == humanFBodyID)
            //{
            //    MC.mc.Logger.Log($"Caching body ramp: {name} ");
            //    MC.Log($"humanbody , type:{__result.GetType()}");
            //    var humanBody = __result as EquipmentEntity;
            //    if (humanBody == null)
            //    {
            //        MC.Log($"humanbody is null , type:{__result.GetType()}");

            //    }
            //    primaryRamps = new List<Texture2D>();
            //    if (humanBody.PrimaryRamps == null)
            //    {
            //        MC.mc.Logger.Log("humanbody primaryramp null");
            //    }
            //    else
            //    {
            //        MC.Log("ok 1");
            //    }
            //    primaryRamps.AddRange(humanBody.PrimaryRamps);
            //    if (minosAssets == null)
            //    {
            //        MC.mc.Logger.Log("minosAssets  null");
            //    }
            //    else
            //    {
            //        MC.Log($"ok 2, count={minosAssets.Count}");
            //    }
            //    foreach (var item in minosAssets.Values)
            //    {
            //        if (name != guids[tieflingTailID] && !name.StartsWith("hair"))
            //        {
            //            PatchEE(item);
            //        }
            //    }
            //}
            else if (guids.ContainsKey(name))
            {
                if (minosAssets.ContainsKey(guids[name]))
                {
                    MC.Log("Asset already copied");
                    if (!ResourcesLibrary.HasLoadedResource(guids[name]))
                    {
                        MC.Log("Not in resiurceslib!!!");
                        Res.Refresh(guids[name]);
                    }
                }
                else
                {

                    var tieflingAsset = (__result as EquipmentEntity);
                    if (tieflingAsset != null)
                    {
                        MC.mc.Logger.Log($"Added new EE with new ramp: {name}:{tieflingAsset.name}");
                        var ma = Clone(tieflingAsset, tieflingTailID == name);
                        ma.name = guids[name];


                        if (tieflingTailID == name && primaryHairRamps != null)
                        {
                            if (!tailpatched)
                            {
                                ModifyTail(ma, tieflingAsset);
                                PatchTail(ma, primaryHairRamps);
                                tailpatched = true;
                            }
                        }
                        else
                        {
                            PatchEE(ma);
                        }
                        minosAssets.Add(guids[name].ToLower(), ma);
                        Res.Put(guids[name].ToLower(), ma);

                    }
                }
            }
            else if (guids.Values.Contains(name))
            {
                MC.Log($"Getting minos asset {name}");
                __result = Res.Get<EquipmentEntity>(name);
            }
            // MC.Log("Loadasset postfix exit");
        }

        private static void PatchEE(EquipmentEntity ma)
        {
            MC.Log($"Patching ma {ma.name}");
            if (ma.name.StartsWith("hair"))
            {
                if (primaryHairRamps != null)
                {
                    ma.m_PrimaryRamps.Clear();
                    ma.m_PrimaryRamps.AddRange(primaryHairRamps);
                    ma.PrimaryColorsProfile.Ramps = ma.m_PrimaryRamps;
                }
            }
            else
            {
                if (primaryRamps != null)
                {
                    ma.m_PrimaryRamps.Clear();
                    ma.m_PrimaryRamps.AddRange(primaryRamps);
                    ma.PrimaryColorsProfile.Ramps = ma.m_PrimaryRamps;
                }
            }
        }

        private static void PatchTail(EquipmentEntity ma, List<Texture2D> _primaryHairRamps)
        {
            MC.Log("patching at 1");
            ma.PrimaryColorsProfile = ScriptableObject.CreateInstance<CharacterColorsProfile>();
            ma.m_PrimaryRamps.AddRange(_primaryHairRamps);
            //ma.BakedTextures = null;//.ColorRamps=ma.m_PrimaryRamps;
            ma.PrimaryColorsProfile.Ramps = ma.m_PrimaryRamps;

            ma.ColorPresets = null;
        }

        private static EquipmentEntity Clone(EquipmentEntity ee, bool logIt = false)
        {
            //var ret = Helpers.CreateCopy<EquipmentEntity>(ee);
            EquipmentEntity ret = (EquipmentEntity)ScriptableObject.CreateInstance(typeof(EquipmentEntity));
            if (logIt)
            {
                MC.Log($"Bakedtextures :" + ee.BakedTextures?.ToString());
            }
            ret.BakedTextures = Helpers.CreateCopy(ee.BakedTextures);
            if (logIt)
            {
                MC.Log($"BodypartCount:{ee.BodyParts.Count()}");
                foreach (var item in ee.BodyParts)
                {
                    MC.Log($"bodypart type:{item.m_Type}");
                }
            }
            ret.BodyParts = Helpers.CreateCopy(ee.BodyParts);
            if (logIt)
            {
                MC.Log($"Ret BodypartCount:{ret.BodyParts.Count()}");
                foreach (var item in ret.BodyParts)
                {
                    MC.Log($"bodypart type:{item.m_Type}");
                }
            }
            ret.CantBeHiddenByDollRoom = ee.CantBeHiddenByDollRoom;
            ret.ColorPresets = ee.ColorPresets;
            ret.IsExportEnabled = ee.IsExportEnabled;
            ret.Layer = ee.Layer;
            ret.m_BonesByName = ee.m_BonesByName;
            ret.m_DlcReward = ee.m_DlcReward;
            ret.m_HideBodyParts = ee.m_HideBodyParts;
            ret.m_IsDirty = ee.m_IsDirty;
            ret.m_PrimaryRamps = new List<Texture2D>();
            ret.m_SecondaryRamps = ee.m_SecondaryRamps;
            ret.m_SpecialPrimaryRamps = ee.m_SpecialPrimaryRamps;
            ret.m_SpecialSecondaryRamps = ee.m_SpecialSecondaryRamps;
            ret.OutfitParts = Helpers.CreateCopy(ee.OutfitParts);
            ret.PrimaryColorsAvailableForPlayer = ee.PrimaryColorsAvailableForPlayer;
            ret.PrimaryColorsProfile = new CharacterColorsProfile();
            {
                ret.PrimaryColorsProfile.Ramps = ee.PrimaryColorsProfile.Ramps;
                ret.PrimaryColorsProfile.RampDlcLocks = ee.PrimaryColorsProfile.RampDlcLocks;
                ret.PrimaryColorsProfile.SpecialRamps = ee.PrimaryColorsProfile.SpecialRamps;
            };
            ret.SecondaryColorsProfile = ee.SecondaryColorsProfile;
            ret.SecondaryColorsAvailableForPlayer = ee.SecondaryColorsAvailableForPlayer;
            ret.ShowLowerMaterials = ee.ShowLowerMaterials;
            ret.SkeletonModifiers = ee.SkeletonModifiers;
            return ret;

        }
        public static void ModifyTail(EquipmentEntity minosTail, EquipmentEntity tieflingTail)
        {
            //minosTail.BodyParts = new List<BodyPart>();
            //minosTail.BodyParts.AddRange(minosTail.OutfitParts.Select(p => new BodyPart()
            //{
            //    RendererPrefab = p.m_Prefab,
            //    Material = p.m_Material,
            //    m_SkinnedRenderer = tieflingTail.OutfitParts[0].m_Prefab.
            //}));
            //minosTail.OutfitParts = new List<EquipmentEntity.OutfitPart>();
            //minosTail.OutfitParts[0].m_Material.color = new Color(30, 30, 30);
            MC.Log($"tailoutfitpart: {minosTail.OutfitParts[0].Special.ToString()}");

            MC.Log($"color:{minosTail.OutfitParts[0].m_Material.color.r}:{minosTail.OutfitParts[0].m_Material.color.g}:{minosTail.OutfitParts[0].m_Material.color.b}");
        }
    }


    public static class Res
    {
        static Dictionary<string, object> resources = new Dictionary<string, object>();
        public static T Get<T>(string name) where T : UnityEngine.Object
        {
            if (resources.ContainsKey(name))
            {
                var r = resources[name] as T;
                if (r != null)
                {
                    if (!ResourcesLibrary.HasLoadedResource(name))
                    {
                        
                        var loadedResource = new ResourcesLibrary.LoadedResource();
                        loadedResource.AssetId = name.ToLower();
                        ResourcesLibrary.StoreResource<EquipmentEntity>(loadedResource, r);
                        loadedResource.RequestCounter += 10;
                        ResourcesLibrary.s_LoadedResources.Add(loadedResource.AssetId, loadedResource);
                        loadedResource.Resource.hideFlags |= HideFlags.DontUnloadUnusedAsset;
                        ResourcesLibrary.HoldResource(loadedResource.AssetId);
                    }
                }
                return r;
            }
            return null;
        }
        public static void Put<T>(string name, T val) where T : UnityEngine.Object
        {
            resources.Add(name, val);
            AddToRL(name, val);
        }

        private static void AddToRL<T>(string name, T val) where T : UnityEngine.Object
        {
            if (!ResourcesLibrary.HasLoadedResource(name))
            {
                var loadedResource = new ResourcesLibrary.LoadedResource();
                loadedResource.AssetId = name.ToLower();
                ResourcesLibrary.StoreResource<T>(loadedResource, val);
                loadedResource.RequestCounter += 10;
                ResourcesLibrary.s_LoadedResources.Add(loadedResource.AssetId, loadedResource);
                ResourcesLibrary.HoldResource(loadedResource.AssetId);
            }
            else
            {
                MC.Log($"{name} already in resourceslibrary");
            }
        }

        public static void Refresh(string name)
        {
            AddToRL(name, (UnityEngine.Object)resources[name]);

        }

        internal static void RefreshAll()
        {
            foreach (var item in resources.Keys)
            {
                Refresh(item);
            }
        }
    }
    //    [HarmonyPatch(typeof(CharGenView), nameof(CharGenView.GoToNextPhaseOrComplete))]
    //    public static class CharGenRacePhaseDetailedPCView_PressConfirmOnPhase_Patch
    //    {
    //        public static List<Texture2D> skinRamps;
    //        public static BlueprintRace HumanRace = BlueprintTools.GetBlueprint<BlueprintRace>("0a5d473ead98b0646b94495af250fdc4");
    //        public static BlueprintRace MinosRace = BlueprintTools.GetBlueprint<BlueprintRace>("0870DC9C34184AFCA5820185842F4F37");
    //        [HarmonyPostfix]
    //        public static void Postfix()
    //        {
    //            MC.mc.Logger.Log("CharGenRace_Next() called");
    //            var dollRoom = Game.Instance.UI.Common.DollRoom;
    //            if(dollRoom.Unit!=null)
    //            {
    //                MC.mc.Logger.Log("dollroom.unit is");
    //                dollRoom.Unit.Progression.CurrentScalePercent = 0.5f;
    //            }
    //            if(dollRoom.GetAvatar()!=null)
    //            {
    //                MC.mc.Logger.Log("dollroom.Avatar is");
    //                if(dollRoom.m_Avatar.m_Race==Kingmaker.Visual.CharacterSystem.CharacterStudio.Race.Catfolk)
    //                {
    //                    MC.mc.Logger.Log("dollroom.Avatar is catfolk");

    //                }
    //            }
    //            if(dollRoom.m_DollState!=null)
    //            {
    //                MC.mc.Logger.Log("dollroom.DollState is");
    //                if (dollRoom.m_DollState.Race == HumanRace)
    //                {
    //                    skinRamps = dollRoom.m_DollState.GetSkinRamps();
    //                }
    //                else if(dollRoom.m_DollState.Race == MinosRace)
    //                {
    //                    if(skinRamps!=null)
    //                        dollRoom.m_DollState.
    //                }

    //            }
    //            //UnitEntityData currentchar = Kingmaker.Game.Instance.Player.AllCharacters.First();
    //            //if (currentchar != null)
    //            //{
    //            //    MC.mc.Logger.Log("Current char not null");

    //            //    //has unitentityview,unitdescriptor-<dolldata
    //            //    //
    //            //    var doll = currentchar.Descriptor.m_LoadedDollData;
    //            //    if (doll != null)
    //            //    {
    //            //        MC.mc.Logger.Log("Current doll not null");

    //            //        if (currentchar.View.IsInDollRoom)
    //            //        {
    //            //            MC.mc.Logger.Log("Current doll in doll room");

    //            //            currentchar.View.Data.Progression.CurrentScalePercent = 2.0f;
    //            //        }
    //            //    }
    //            //    //DollState doll = currentchar.GetDollState();
    //            //}
    //            //else
    //            //    MC.mc.Logger.Log("Current char null");


    //        }
    //    }
}
