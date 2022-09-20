using Kingmaker;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.CharGen;
using Kingmaker.Blueprints.Classes;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Entities;
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
        public float ExtraSizePercent;
        public bool reset=false;
        public static BlueprintRace HumanRace = BlueprintTools.GetBlueprint<BlueprintRace>("0a5d473ead98b0646b94495af250fdc4");
        public static BlueprintRace TieflingRace = BlueprintTools.GetBlueprint<BlueprintRace>("5c4e42124dc2b4647af6e36cf2590500");
        public static BlueprintRaceVisualPresetReference[] humanpresets=null;
        public static BlueprintRaceVisualPresetReference[] minosPresets= null;
        public ChangeVisualSize()
        {
            
            
           
        }
        public override void OnActivate()
        {
            MC.mc.Logger.Log(Owner.View.GetType().ToString());
            var v = this.Owner.View as UnitEntityView;
            if(v != null)
            {
                MC.mc.Logger.Log("unitview is not null");

                ((UnitEntityData)this.Owner.View.Data).Progression.CurrentScalePercent = ExtraSizePercent;
                v.UpdateScaleImmediately();
            }
            else
            {
                MC.mc.Logger.Log("unitview is null");
            }
        }
        public override void OnPostLoad()
        {
            MC.mc.Logger.Log("OnPostLoad minos scaling");

            OnActivate();
            base.OnPostLoad(); 
        }
        public override void OnViewDidAttach()
        {
            MC.mc.Logger.Log("OnViewDidAttach minos scaling");

            OnActivate();
            base.OnViewDidAttach();
        }

        public override void OnTurnOff()
        {
            MC.mc.Logger.Log("Turning off minos scaling");
        }
    }
}
