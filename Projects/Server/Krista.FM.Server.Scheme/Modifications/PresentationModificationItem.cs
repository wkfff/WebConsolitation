using System;
using System.Collections.Generic;
using System.Text;
using Krista.FM.ServerLibrary;
using Krista.FM.Server.Scheme.Classes;

namespace Krista.FM.Server.Scheme.Modifications
{
    internal sealed class PresentationModificationItem : MinorObjectModificationItem
    {
        public PresentationModificationItem(string name, object fromObject, object toObject)
            : base(ModificationTypes.Modify, name, fromObject, toObject, null)
        {
        }

        protected override void OnBeforeChildApplay(ModificationContext context, out bool isAppliedPartially)
        {
            base.OnBeforeChildApplay(context, out isAppliedPartially);

            Entity entity = (Entity)Parent.Parent.FromObject;

            Trace.WriteLine(String.Format("Изменение представления \"{0}\"", ((Presentation)ToObject).Name));

            entity.UpdatePresentation(context, (Presentation)ToObject);
        }

        protected override void OnAfterChildApplay(ModificationContext context)
        {
            Entity entity = (Entity)Parent.Parent.FromObject;
            entity.SaveConfigurationToDatabase(context);

            base.OnAfterChildApplay(context);
        }
    }
}
