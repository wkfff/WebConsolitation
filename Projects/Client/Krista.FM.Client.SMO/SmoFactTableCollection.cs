using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    public class SmoFactTableCollection : SmoDictionaryBase<string , IFactTable>, IFactTableCollection
    {
        public SmoFactTableCollection(SMOSerializationInfo cache) 
            : base(cache)
        {
        }


        public SmoFactTableCollection(IDictionaryBase<string, IFactTable> serverObject) 
            : base(serverObject)
        {
        }

        protected override Type GetItemValueSmoObjectType(object obj)
        {
            return typeof(SmoFactTable);
        }

        #region IEntityCollection<IFactTable> Members

        public IFactTable CreateItem(ClassTypes classType, SubClassTypes subClassType)
        {
            throw new NotImplementedException();
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
