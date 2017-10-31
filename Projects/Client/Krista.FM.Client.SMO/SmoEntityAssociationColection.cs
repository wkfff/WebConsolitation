using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    public class SmoEntityAssociationColection : SmoDictionaryBase<string, IEntityAssociation>, IEntityAssociationCollection
    {
        public SmoEntityAssociationColection(IDictionaryBase<string, IEntityAssociation> serverObject)
            : base(serverObject)
        {
        }

        public SmoEntityAssociationColection(SMOSerializationInfo cache) 
            : base(cache)
        {
        }

        protected override Type GetItemValueSmoObjectType(object obj)
        {
            return obj is IBridgeAssociation ? typeof (SmoBridgeAssociation) : typeof (SmoAssociation);
        }

        #region IEntityAssociationCollection Members

        public IEntityAssociation CreateItem(IEntity roleA, IEntity roleB)
        {
            throw new NotImplementedException();
        }

        public IEntityAssociation CreateItem(IEntity roleA, IEntity roleB, AssociationClassTypes associationClassType)
        {
            return ((IEntityAssociationCollection) ServerControl).CreateItem(roleA, roleB, associationClassType);
        }

        #endregion

        #region ICollection2DataTable Members

        public DataTable GetDataTable()
        {
            return ((ICollection2DataTable)ServerControl).GetDataTable();
        }

        #endregion
    }
}
