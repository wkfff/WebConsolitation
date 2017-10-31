using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Classes;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Modifications
{
    /// <summary>
    /// Отвечает за обновление объектов
    /// </summary>
    internal sealed class UpdateMajorObjectModificationItem : MajorObjectModificationItem
    {
        public UpdateMajorObjectModificationItem(ModificationTypes type, string name, object fromObject, object toObject, ModificationItem parent)
            : base(type, name, fromObject, toObject, parent)
        {
        }

        protected override void OnBeforeChildApplay(ModificationContext context, out bool isAppliedPartially)
        {
            base.OnBeforeChildApplay(context, out isAppliedPartially);

            CommonDBObject fromObject = FromObject as CommonDBObject;

            Trace.WriteLine(String.Format("Обновление объекта {0}", fromObject.FullName));

            fromObject.Update(context, (IModifiable)ToObject);
        }

        protected override void OnAfterChildApplay(ModificationContext context)
        {
            CommonDBObject fromObject = FromObject as CommonDBObject;
            fromObject.SaveConfigurationToDatabase(context);

            base.OnAfterChildApplay(context);
        }
    }
}
