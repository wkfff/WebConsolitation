using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Classes;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Modifications
{
    internal sealed class RemoveMajorModificationItem : MajorObjectModificationItem
    {
        public RemoveMajorModificationItem(string name, object fromObject, ModificationItem parent)
            : base(ModificationTypes.Remove, name, fromObject, null, parent)
        {
        }

        protected override void OnBeforeChildApplay(ModificationContext context, out bool isAppliedPartially)
        {
            base.OnBeforeChildApplay(context, out isAppliedPartially);

            // FromObject - удаляемый объект
            if (FromObject is EntityDataAttribute)
            {
                EntityDataAttribute attribute = FromObject as EntityDataAttribute;
                ((Entity)Parent.FromObject).RemoveAttribute(context, attribute.Name);
            }

            if (FromObject is CommonDBObject)
            {
                ((CommonDBObject)FromObject).Drop(context);
            }
        }
    }
}
