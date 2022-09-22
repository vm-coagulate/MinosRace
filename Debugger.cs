using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Settings;
using Kingmaker.UnitLogic.Class.LevelUp;
using Kingmaker.Visual.CharacterSystem;
using MinosRace.Context;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MinosRace
{
    [HarmonyPatch(typeof(Character))]
    public class CharDebugger
    {
        [HarmonyPatch(nameof(Character.SetRampIndices), typeof(EquipmentEntity), typeof(int), typeof(int), typeof(int), typeof(int))]
        [HarmonyPrefix]
        public static bool SetRampIndices_prefix(EquipmentEntity ee, int primaryRampIndex, int secondaryRampIndex, int primarySpecialRampIndex, int secondarySpecialRampIndex,
            Character __instance)
        {
            if (ee.name == "a15b0963b1a047f7b151a1e4a0f38281")
            {
                MC.Log($"Tail primary color count:{ee.m_PrimaryRamps.Count()}");
                MC.Log($"primaryindex{primaryRampIndex}");
                var selri = __instance.m_RampIndices.FirstOrDefault((Character.SelectedRampIndices i) => i.EquipmentEntity == ee);
                MC.Log($"Tail PrimaryIndex: {selri?.PrimaryIndex}");
            }
            return true;
        }
        [HarmonyPatch(nameof(Character.OnRenderObject))]
        public static void OnRender_postfix(Character __instance)
        {

        }
    }

    [HarmonyPatch(typeof(EquipmentEntity))]
    public class EEDebugger
    {
        [HarmonyPatch(nameof(EquipmentEntity.RepaintTextures), typeof(EquipmentEntity.PaintedTextures), typeof(EquipmentEntity.IColorRampIndicesProvider))]
        [HarmonyPrefix]
        public static bool RepaintTextures_prefix(EquipmentEntity.PaintedTextures paintedTextures, EquipmentEntity.IColorRampIndicesProvider i, EquipmentEntity __instance)
        {
            if (__instance.name == "a15b0963b1a047f7b151a1e4a0f38281"||__instance.name=="EE_Tail_TL")
            {
                MC.Log($"needsrepaint {paintedTextures.CheckNeedRepaint(__instance, i.PrimaryIndex, i.SecondaryIndex, i.SpecialPrimaryIndex, i.SpecialSecondaryIndex).ToString()}");
                MC.Log($"Bodypartcount:{__instance.BodyParts.Count()}");
            }
            return true;
        }
        
    }

    [HarmonyPatch]
    public class ResLibPatch
    {

        [HarmonyPatch(nameof(ResourcesLibrary.FreeResourceRequest),typeof(string),typeof(bool))]
        static void FreeResource_postfix(string assetId,bool held)
        {
            MC.Log($"Free {assetId}");
        }

        //static MethodBase TargetMethod()
        //{
        //    return AccessTools.Method(typeof(ResourcesLibrary), "TryGetResource").MakeGenericMethod(typeof(EquipmentEntity));
        //}
        //static void Postfix(MethodBase __originalMethod, string assetId, UnityEngine.Object __result)
        //{
        //    MC.Log($"TryGetResource {assetId}");
        //    if (__result == null)
        //    {
        //        MC.Log("Failed");
        //    }
        //    else
        //    {
        //        MC.Log($"Succeeded: {__result.name}");
        //    }
        //}
        //public static bool TryGetResource_prefix(string assetId, bool ignorePreloadWarning, bool hold)
        //{
        //    MC.Log($"TryGetResource {assetId}");
        //    return true;
        //}
    }
}
