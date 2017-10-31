using System.Collections.Generic;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    public class SmoUniqueKeyCollection : SmoDictionaryBase<string, IUniqueKey>, IUniqueKeyCollection, IMinorModifiable
    {
        public SmoUniqueKeyCollection(IDictionaryBase<string, IUniqueKey> serverObject) 
            : base(serverObject)
        {
        }

        public SmoUniqueKeyCollection(SMOSerializationInfo cache)
            : base(cache)
        {
        }

        public IModificationItem GetChanges(IModifiable toObject)
        {
            return ((IUniqueKeyCollection) serverControl).GetChanges(toObject);
        }

        public IUniqueKey CreateItem(string caption, List<string> fieldList, bool hashable)
        {
            return ((IUniqueKeyCollection)serverControl).CreateItem(caption, fieldList, hashable);
        }

    }
}
