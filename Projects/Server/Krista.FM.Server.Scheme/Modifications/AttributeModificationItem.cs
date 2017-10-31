using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Classes;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Modifications
{
    internal sealed class AttributeModificationItem : MinorObjectModificationItem
    {
        public AttributeModificationItem(string name, object fromObject, object toObject)
            : base(ModificationTypes.Modify, name, fromObject, toObject, null)
        {
        }

        protected override void OnBeforeChildApplay(ModificationContext context, out bool isAppliedPartially)
        {
            base.OnBeforeChildApplay(context, out isAppliedPartially);

            if (Parent.Parent.FromObject is Entity)
            {
                Entity entity = (Entity)Parent.Parent.FromObject;
                Trace.WriteLine(String.Format("Изменение атрибута \"{0} ({1})\"", ((DataAttribute)ToObject).Caption, ((DataAttribute)ToObject).Name));
                entity.UpdateAttribute(context, (EntityDataAttribute)ToObject);
            }
            else if (Parent.Parent.FromObject is Presentation)
            {
                Presentation presentation = (Presentation)Parent.Parent.FromObject;
                Trace.WriteLine(String.Format("Изменение атрибута представления \"{0} ({1})\"", ((DataAttribute)ToObject).Caption, ((DataAttribute)ToObject).Name));
                presentation.UpdateAttribute(context, (EntityDataAttribute)ToObject);
            }
        }

        protected override void OnAfterChildApplay(ModificationContext context)
        {
            Entity entity;
            if (Parent.Parent.FromObject is Presentation)
                entity = (Entity)Parent.Parent.Parent.Parent.FromObject;
            else
                entity = (Entity)Parent.Parent.FromObject;
            entity.SaveConfigurationToDatabase(context);

            base.OnAfterChildApplay(context);
        }
    }
}
