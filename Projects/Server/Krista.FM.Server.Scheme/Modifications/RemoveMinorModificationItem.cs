using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Classes;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Modifications
{
    internal sealed class RemoveMinorModificationItem : MinorObjectModificationItem
    {
        public RemoveMinorModificationItem(string name, object fromObject, ModificationItem parent)
            : base(ModificationTypes.Remove, name, fromObject, null, parent)
        {
        }

        protected override void OnBeforeChildApplay(ModificationContext context, out bool isAppliedPartially)
        {
            base.OnBeforeChildApplay(context, out isAppliedPartially);

            if (FromObject is EntityDataAttribute)
            {
                EntityDataAttribute attribute = FromObject as EntityDataAttribute;
                ((DataAttributeCollection)Parent.FromObject).Parent.RemoveAttribute(context, attribute.Name);
            }
            else if (FromObject is AssociateRule)
            {
                AssociateRule rule = FromObject as AssociateRule;
                ((BridgeAssociation)((AssociateRule)FromObject).Owner).AssociateRules.Remove(rule.ObjectKey);
                rule.State = ServerSideObjectStates.Deleted;
            }
            else if (FromObject is AssociateMapping)
            {
                AssociateMapping mapping = FromObject as AssociateMapping;
                if (((ServerSideObject)FromObject).Owner is AssociateRule)
                {
                    ((AssociateRule)((AssociateMapping)FromObject).Owner).Mappings.Remove(mapping);
                }
                else if (((ServerSideObject)FromObject).Owner is Association)
                {
                    ((BridgeAssociation)((AssociateMapping)FromObject).Owner).Mappings.Remove(mapping);
                }
                mapping.State = ServerSideObjectStates.Deleted;
            }
            else if (FromObject is Presentation)
            {
                Presentation presentation = FromObject as Presentation;
                ((PresentationCollection)Parent.FromObject).Parent.RemovePresentation(context, presentation.ObjectKey);
                //TODO: а тут не забыли добавить ? presentation.State = ServerSideObjectStates.Deleted;
            }

            else if (FromObject is UniqueKey)
            {
                UniqueKey uniqueKey = FromObject as UniqueKey;
                UniqueKeyCollection uniqueKeyCollection = (UniqueKeyCollection)uniqueKey.Parent.UniqueKeys;
                uniqueKeyCollection.RemoveUniqueKey(context, uniqueKey.ObjectKey);
                uniqueKey.State = ServerSideObjectStates.Deleted;
            }
        }
    }
}
