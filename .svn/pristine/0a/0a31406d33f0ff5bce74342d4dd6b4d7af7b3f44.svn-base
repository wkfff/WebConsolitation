using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Classes;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Modifications
{
    internal sealed class HierarchyModificationItem : MinorObjectModificationItem
    {
        public HierarchyModificationItem(string name, object fromObject, object toObject)
            : base(ModificationTypes.Modify, name, fromObject, toObject, null)
        {
        }

        protected override void OnBeforeChildApplay(ModificationContext context, out bool isAppliedPartially)
        {
            base.OnBeforeChildApplay(context, out isAppliedPartially);

            DimensionLevelCollection fromObject = FromObject as DimensionLevelCollection;

            Trace.WriteLine(String.Format("Изменение иерархии объекта"));

            fromObject.Update(context, (IModifiable)ToObject);
        }
    }
}
