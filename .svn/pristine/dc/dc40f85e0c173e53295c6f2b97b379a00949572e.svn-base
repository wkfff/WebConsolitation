using System;
using System.Collections.Generic;

using Krista.FM.Server.Common;
using Krista.FM.Server.Scheme.Modifications;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Server.Scheme.Classes
{
    internal abstract class MinorObjecModifiableCollection<TKey, TValue> : ModifiableCollection<TKey, TValue> where TValue : ICommonDBObject
    {
        public MinorObjecModifiableCollection(ServerSideObject owner, ServerSideObjectStates state)
            : base(owner, state)
        {
        }

        protected override ModificationItem GetCreateModificationItem(KeyValuePair<TKey, TValue> item, CollectionModificationItem root)
        {
            return new CreateModificationItem(item.Value.FullName, Owner, item.Value, root);
        }

        protected override ModificationItem GetRemoveModificationItem(KeyValuePair<TKey, TValue> item, CollectionModificationItem root)
        {
            return new RemoveMajorModificationItem(item.Value.FullName, item.Value, root);
        }
    }
}
