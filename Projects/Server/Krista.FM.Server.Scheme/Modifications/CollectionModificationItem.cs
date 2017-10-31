using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Modifications
{
    internal class CollectionModificationItem : ModificationItem
    {
        public CollectionModificationItem(string name, object fromObject, object toObject)
            : base(ModificationTypes.Modify, name, fromObject, toObject, null)
        {
        }

        public override int ImageIndex
        {
            get
            {
                if (ModificationObject is IPackageCollection)
                    return 10;
                else if (ModificationObject is IEntityCollection<IEntity>)
                    return 15;
                else if (ModificationObject is IEntityAssociationCollection)
                    return 23;
                else if (ModificationObject is IDocumentCollection)
                    return 22;
                else if (ModificationObject is IDataAttributeCollection)
                    return 26;
                else if (ModificationObject is IAssociateRuleCollection)
                    return 49;
                else if (ModificationObject is IUniqueKeyCollection)
                    return 64; //Krista.FM.Client.SchemeEditor.Images.UniqueKeys
                else
                    return base.ImageIndex;
            }
        }
    }
}
