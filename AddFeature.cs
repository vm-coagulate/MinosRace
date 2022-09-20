using Kingmaker.EntitySystem;
using MinosRace.Context;

namespace MinosRace
{
    public class AddFeature:EntityFactComponentDelegate
    {
        public override void OnActivate()
        {
            MC.mc.Logger.Log("Activate Addfeature");
        }
        public override void OnTurnOn()
        {
            MC.mc.Logger.Log("TurnOn Addfeature");

        }
        public override void ApplyValidation(Owlcat.QA.Validation.ValidationContext context, int parentIndex)
        {
            MC.mc.Logger.Log("ApplyValidation Addfeature");
            base.ApplyValidation(context, parentIndex);
        }
        public override void OnFactAttached()
        {
            MC.mc.Logger.Log("OnFactAttached Addfeature");

            base.OnFactAttached();
        }
        public override void OnApplyPostLoadFixes()
        {
            MC.mc.Logger.Log("OnApplyPostLoadFixes Addfeature");

            base.OnApplyPostLoadFixes();
        }
        public override void OnInitialize()
        {
            MC.mc.Logger.Log("OnInitialize Addfeature");

            base.OnInitialize();
        }
        public override void OnPostLoad()
        {
            MC.mc.Logger.Log("OnPostLoad Addfeature");

            base.OnPostLoad();
        }
        public override void OnRecalculate()
        {
            MC.mc.Logger.Log("OnRecalculate Addfeature");

            base.OnRecalculate();
        }
        public override void OnViewDidAttach()
        {
            MC.mc.Logger.Log("OnViewDidAttach Addfeature");

            base.OnViewDidAttach();
        }
    }
}