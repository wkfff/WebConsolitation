using System;
using System.Collections.Generic;
using System.Text;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Modifications
{
    internal abstract class MajorObjectModificationItem : ModificationItem
    {
        public MajorObjectModificationItem(ModificationTypes type, string name, object fromObject, object toObject, ModificationItem parent)
            : base(type, name, fromObject, toObject, parent)
        {
        }

        public override int ImageIndex
        {
            get 
            {
                if (ModificationObject is IPackage)
                    return 11;
                else if (ModificationObject is IEntity)
                    return 16;
                else if (ModificationObject is IEntityAssociation)
                    return 32;
                else if (ModificationObject is IDocument)
                    return 21;
                else 
                    return base.ImageIndex;
            }
        }
    }
}
