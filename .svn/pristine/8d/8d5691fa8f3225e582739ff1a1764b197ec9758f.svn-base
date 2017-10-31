using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    public class SmoAssociationCollection : SmoEntityAssociationColection, IAssociationCollection
    {
        public SmoAssociationCollection(SMOSerializationInfo cache) 
            : base(cache)
        {
        }


        public SmoAssociationCollection(IDictionaryBase<string, IEntityAssociation> serverObject) 
            : base(serverObject)
        {
        }

        protected override System.Type GetItemValueSmoObjectType(object obj)
        {
            return base.GetItemValueSmoObjectType(obj);
        }
    }

    public class SmoAssociatedCollection : SmoEntityAssociationColection, IAssociationCollection
    {
        public SmoAssociatedCollection(IDictionaryBase<string, IEntityAssociation> serverObject) 
            : base(serverObject)
        {
        }


        public SmoAssociatedCollection(SMOSerializationInfo cache) 
            : base(cache)
        {
        }
    }
}
