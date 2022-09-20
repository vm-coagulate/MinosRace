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
            MC.mc.Logger.Log("skinentities ");
            if (__result.Any(p => p.AssetId == minosTailID))
            {
                MC.mc.Logger.Log("Removing tail from skin entities");

                __result.RemoveAll(p => p.AssetId == minosTailID);

            }
        }


    }

    [HarmonyPatch(typeof(AssetBundle))]
    public static class AssetPatcher
    {
        static CharacterColorsProfile minosbodycolorprofile = (CharacterColorsProfile)ScriptableObject.CreateInstance(typeof(CharacterColorsProfile));
        static string tieflingTailID = "d15dbf2912cd6f94492a9e1053aa0ebd";
        static string aHumanHairID = "ee3945f41269aed4b9fcb6304c3fda79";
        static string humanBodyID = "bb6988a21733fad4296ad22537248fea";

    
        static List<Texture2D> primaryRamps = null;
        static List<Texture2D> primaryHairRamps = null;

        public static Dictionary<string, EquipmentEntity> minosAssets = new Dictionary<string, EquipmentEntity>();

        static Dictionary<string, string> guids = new Dictionary<string, string>()
        {
            {"6a5ae89de41b6b149856718b6058168f",  "9F18B0C4E57646DDA309DABD030A4B79".ToLower() }, //f_body
            {"7b27b2063f548794e845e0ee8ea7b91b",  "7E54EC353B6B4F6AB71BAAC3EBBDDFBD".ToLower()  },
            {"4a4afd8ea46ff2e438bb078495bd3531",  "DB683CE3EF724EF6929CE723C5BA986D".ToLower()  },
            {"49b32d9af554e6742bed805d80ccde93",  "7DCBF6B10FC44FF79AB0C74F8E5899E0".ToLower()  },
            {"f7fdd6a03bc43da4da6d913a57f28c7c",  "E24AFA3ABEE0418E833E77239DDE8C1A".ToLower()  },
            {"a88d1147b9c76364db8d34d956bb6fcb",  "0C58485A0FD3482DBCA901E8508B4CD3".ToLower()  },
            {"d1dcf6b4e326a9d459ee5b2e5b7b7cbc",  "3DC74FD208974CDDA9B53864635CC1E2".ToLower()  }, //m_body
            {"6ae0e2be0e8f9f54981033b4a61f11ed",  "8F54696DE3524E67BED4FBF41BAE13BA".ToLower()  },
            {"b354195728faa79449de9b3197f3b449",  "2F97A2D7186B4DA08737DADDF29C5C40".ToLower()  },
            {"a8b95db20e630214dabfb79424494c34",  "38E57963F5D8430D8D0F0F5EB809F72D".ToLower()  },
            {"cdc8632dd9b07744eb7baa143e39bf06",  "3F3C16C2626A446BBB8932C3539F4CB9".ToLower()   },
            {"a957a3408601f9046b56015f6c40a8d3",  "7E8973B0DC58464F851B8ED610175E08".ToLower()   },
            {"d15dbf2912cd6f94492a9e1053aa0ebd",  "a15b0963b1a047f7b151a1e4a0f38281".ToLower()   }//tail

        };


        //
        //public Object LoadAsset(string name, Type type)
        [HarmonyPatch(nameof(AssetBundle.LoadAsset), typeof(string), typeof(Type)), HarmonyPostfix]
        public static void LoadAsset(string name, ref UnityEngine.Object __result)
        {
            if (name == aHumanHairID)
            {
                if (__result is EquipmentEntity)
                {
                    MC.mc.Logger.Log($"Caching hair ramp: {name}");
                   var humanHair= __result as EquipmentEntity;
                    primaryHairRamps = new List<Texture2D>();
                    primaryHairRamps.AddRange(humanHair.PrimaryRamps);
                    if (minosAssets.ContainsKey(guids[tieflingTailID]))
                    {
                        minosAssets[guids[tieflingTailID]].PrimaryColorsProfile = ScriptableObject.CreateInstance<CharacterColorsProfile>();
                        minosAssets[guids[tieflingTailID]].m_PrimaryRamps.Clear();
                        minosAssets[guids[tieflingTailID]].m_PrimaryRamps.AddRange(primaryHairRamps);

                        minosAssets[guids[tieflingTailID]].PrimaryColorsProfile.Ramps = minosAssets[guids[tieflingTailID]].m_PrimaryRamps;
                    }
                }
            }

            if (name==humanBodyID)
            {
                MC.mc.Logger.Log($"Caching body ramp: {name}");
                var humanBody = __result as EquipmentEntity;
                primaryRamps = new List<Texture2D>();
                primaryRamps.AddRange(humanBody.PrimaryRamps);

                foreach (var item in minosAssets.Values.Where(p => p.name != guids[tieflingTailID]))
                {
                    if (item.m_PrimaryRamps != null)
                    {
                        item.m_PrimaryRamps.AddRange(primaryRamps);
                        item.PrimaryColorsProfile.Ramps = item.m_PrimaryRamps;
                    }
                }
            }
            else if (guids.ContainsKey(name))
            {
                var tieflingAsset = (__result as EquipmentEntity);
                if (tieflingAsset != null)
                {
                    MC.mc.Logger.Log($"Added new EE with new ramp: {name}:{tieflingAsset.ToString()}");
                    var ma = Clone(tieflingAsset);
                    minosAssets.Add(guids[name].ToLower(), ma);
                    if (tieflingTailID == name && primaryHairRamps != null)
                    {
                        ma.PrimaryColorsProfile = ScriptableObject.CreateInstance<CharacterColorsProfile>();
                        ma.m_PrimaryRamps.AddRange(primaryHairRamps);
                        ma.PrimaryColorsProfile.Ramps = ma.m_PrimaryRamps;
                    }
                    else
                    {
                        if (primaryRamps != null)
                        {
                            ma.m_PrimaryRamps.AddRange(primaryRamps);
                            ma.PrimaryColorsProfile.Ramps = ma.m_PrimaryRamps;

                        }
                    }
                    var loadedResource = new ResourcesLibrary.LoadedResource();
                    loadedResource.AssetId = guids[name].ToLower();
                    ResourcesLibrary.StoreResource<EquipmentEntity>(loadedResource, ma);
                    ResourcesLibrary.s_LoadedResources.Add(loadedResource.AssetId, loadedResource);

                }
            }
        }

        private static EquipmentEntity Clone(EquipmentEntity ee)
        {
            EquipmentEntity ret = (EquipmentEntity)ScriptableObject.CreateInstance(typeof(EquipmentEntity));
            ret.BakedTextures = ee.BakedTextures;
            ret.BodyParts = ee.BodyParts;
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
            ret.OutfitParts = ee.OutfitParts;
            ret.PrimaryColorsAvailableForPlayer = ee.PrimaryColorsAvailableForPlayer;
            ret.PrimaryColorsProfile = minosbodycolorprofile;
            {
                minosbodycolorprofile.Ramps = ee.PrimaryColorsProfile.Ramps;
                minosbodycolorprofile.RampDlcLocks = ee.PrimaryColorsProfile.RampDlcLocks;
                minosbodycolorprofile.SpecialRamps = ee.PrimaryColorsProfile.SpecialRamps;
            };
            ret.SecondaryColorsProfile = ee.SecondaryColorsProfile;
            ret.SecondaryColorsAvailableForPlayer = ee.SecondaryColorsAvailableForPlayer;
            ret.ShowLowerMaterials = ee.ShowLowerMaterials;
            ret.SkeletonModifiers = ee.SkeletonModifiers;
            return ret;

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
