using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Facts;
using Kingmaker.Designers.Mechanics.Facts;
using Kingmaker.EntitySystem;
using Kingmaker.UnitLogic;
using Kingmaker.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinosRace
{
    public class ModifyBlueprintOnApply: UnitFactComponentDelegate
    {
        public BlueprintFeature m_Feature;
        public BlueprintRace m_RaceToModify;
        public BlueprintUnitFact m_fact; 
        public bool reset=false;
        public override void OnFactAttached()
        {
            if (reset)
            {
                m_RaceToModify.m_Features= m_RaceToModify.m_Features.Where(p => p.Get() != m_Feature).ToArray();
            }
            else
            {
               
                    if (!m_RaceToModify.m_Features.Any(p => p.Get() == m_Feature))
                    {
                        m_RaceToModify.m_Features = m_RaceToModify.m_Features.AddToArray(m_Feature.ToReference<BlueprintFeatureBaseReference>());
                    }
                
            }
            
        }
       
    }
}
