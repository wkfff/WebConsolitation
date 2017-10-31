using System.Collections.Generic;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design 
{
    public class SmoUniqueKeyCollectionDesign : SmoDictionaryBaseDesign<string, IUniqueKey>, IUniqueKeyCollection
    {
        public SmoUniqueKeyCollectionDesign(IDictionaryBase<string, IUniqueKey> serverObject) 
            : base(serverObject)
        {
        }

        public IModificationItem GetChanges(IModifiable toObject)
        {
            return ((IUniqueKeyCollection)serverControl).GetChanges(toObject);
        }

        public IUniqueKey CreateItem(string caption, List<string> fieldList, bool hashable)
        {
            return ((IUniqueKeyCollection)serverControl).CreateItem(caption, fieldList, hashable);
        }

        
    }
}
