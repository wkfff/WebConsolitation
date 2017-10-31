using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Modifications
{
    // TODO abstract class MinorObjectModificationItem
    internal /*abstract*/ class MinorObjectModificationItem : ModificationItem
    {
        public MinorObjectModificationItem(ModificationTypes type, string name, object fromObject, object toObject, ModificationItem parent)
            : base(type, name, fromObject, toObject, parent)
        {
        }

        public override int ImageIndex
        {
            get
            {
                if (ModificationObject is IDataAttribute)
                    return 27;
                else if (ModificationObject is IDimensionLevelCollection)
                    return 25;
                else if (ModificationObject is IAssociateMappingCollection)
                    return 47;
                else if (ModificationObject is IAssociateRule)
                    return 49;
                else if (ModificationObject is IUniqueKey)
                    return 63; //Krista.FM.Client.SchemeEditor.Images.UniqueKey
                else
                    return base.ImageIndex;
            }
        }
    }
}
