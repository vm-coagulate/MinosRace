using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.CharGen;
using Kingmaker.Blueprints.Classes;
using Kingmaker.EntitySystem;
using Kingmaker.View;
using MinosRace.Context;
using Owlcat.Runtime.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabletopTweaks.Core.Utilities;

namespace MinosRace
{
    public class ChangeVisualSize: EntityFactComponentDelegate
    {
        public float Multiplier;
        public bool reset=false;
        public static BlueprintRace HumanRace = BlueprintTools.GetBlueprint<BlueprintRace>("0a5d473ead98b0646b94495af250fdc4");
        public static BlueprintRace TieflingRace = BlueprintTools.GetBlueprint<BlueprintRace>("5c4e42124dc2b4647af6e36cf2590500");
        public static BlueprintRaceVisualPresetReference[] humanpresets=null;
        public static BlueprintRaceVisualPresetReference[] minosPresets= null;
        public ChangeVisualSize()
        {
            
            //if(minosPresets==null)
            //{
            //    minosPresets = new BlueprintRaceVisualPresetReference[TieflingRace.m_Presets.Length];
            //    for (int i = 0; i < minosPresets.Length; i++)
            //    {
            //        minosPresets[i] = Helpers.CreateBlueprint<BlueprintRaceVisualPreset>(MC.mc, "MinosVisualPreset" + i, bp =>
            //        {
            //            bp.name = "MinosVisualPreset" +i;
            //            bp.MaleSkeleton = TieflingRace.m_Presets[i].Get().MaleSkeleton;
            //            bp.FemaleSkeleton = TieflingRace.m_Presets[i].Get().FemaleSkeleton;
            //            bp.m_Skin = TieflingRace.m_Presets[i].Get().m_Skin;
                        
            //        }).ToReference<BlueprintRaceVisualPresetReference>();
            //    }
            //}
           
        }
        public override void OnTurnOn()
        {
            //UnitEntityView componentNonAlloc = ObjectExtensions.GetComponentNonAlloc<UnitEntityView>(this);
            MC.mc.Logger.Log(Owner.View.GetType().ToString());
            var v = this.Owner.View as UnitEntityView;
            if(v != null)
            {
                MC.mc.Logger.Log("unitview is not null");
             
                v.m_RacialPetScaleCoeff = Multiplier;
            }
            else
            {
                MC.mc.Logger.Log("unitview is null");
            }
            //if (reset)
            //{
            //    if (humanpresets != null)
            //    {
            //        HumanRace.m_Presets = new BlueprintRaceVisualPresetReference[humanpresets.Count()];
            //        humanpresets.CopyTo(HumanRace.m_Presets, 0);
                    
            //    }
            //}
            //else
            //{
            //    //save presets
            //    humanpresets = new  BlueprintRaceVisualPresetReference[HumanRace.m_Presets.Count()];
            //    HumanRace.m_Presets.CopyTo(humanpresets, 0);    
                
            //    //replace with tiefling
            //    HumanRace.m_Presets = new BlueprintRaceVisualPresetReference[minosPresets.Length];
            //    minosPresets.CopyTo(HumanRace.m_Presets, 0);
                
            //}
        }

        

        public override void OnTurnOff()
        {
            MC.mc.Logger.Log("Turning off minos scaling");
            var v = this.Owner.View as UnitEntityView;
            if (v != null)
            {
                v.m_RacialPetScaleCoeff = 1.0f;
            }
            //HumanRace.m_Presets = new BlueprintRaceVisualPresetReference[humanpresets.Count()];
            //humanpresets.CopyTo(HumanRace.m_Presets, 0);
        }
    }
}
