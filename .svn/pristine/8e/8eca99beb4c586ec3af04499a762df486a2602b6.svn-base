using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.Common;
using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Classes;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Modifications
{
    internal sealed class CreateMinorModificationItem : ModificationItem
    {
        public CreateMinorModificationItem(string name, object ownerObject, object toObject, ModificationItem parent)
            : base(ModificationTypes.Create, name, ownerObject, toObject, parent)
        {
        }

        //protected override void OnBeforeChildApplay(ModificationContext context)
        //{
        //    bool isSuccessful;
        //    OnBeforeChildApplay(context, out isSuccessful);
        //}

        protected override void OnBeforeChildApplay(ModificationContext context, out bool isAppliedPartially)
        {
            base.OnBeforeChildApplay(context, out isAppliedPartially);

            if (ToObject is EntityDataAttribute)
            {
                EntityDataAttribute attribute = ToObject as EntityDataAttribute;
                ((Entity)FromObject).AddAttribute(context, attribute);
                attribute.State = ServerSideObjectStates.Consistent;
            }
            else
                if (ToObject is AssociateRule)
                {
                    AssociateRule rule = ToObject as AssociateRule;
                    ((AssociateRuleCollection)((BridgeAssociation)FromObject).AssociateRules).AddRule(rule);
                    rule.State = ServerSideObjectStates.Consistent;
                }
                else
                    if (ToObject is AssociateMapping)
                    {
                        AssociateMapping mapping = ToObject as AssociateMapping;
                        ((AssociateMappingCollection)FromObject).Add(mapping);
                        mapping.State = ServerSideObjectStates.Consistent;
                    }
                    else
                        if (ToObject is Presentation)
                        {
                            Presentation presentation = ToObject as Presentation;
                            ((Entity)FromObject).AddPresentation(context, presentation);
                        }
                        else
                            if (ToObject is UniqueKey)
                            {
                                UniqueKey uniqueKey = ToObject as UniqueKey;
                                UniqueKeyCollection uniqueKeyCollection = (UniqueKeyCollection) ((Entity) FromObject).UniqueKeys;
                                uniqueKeyCollection.AddUniqueKey(context, uniqueKey, out isAppliedPartially);
                            }
        }
    }
}
