using System;
using System.Data;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO
{
    public class SMODataAttributeCollection : SmoDictionaryBase<string, IDataAttribute>, IDataAttributeCollection
    {
        public SMODataAttributeCollection(SMOSerializationInfo cache) : 
            base(cache)
        {
        }

        public SMODataAttributeCollection(IDictionaryBase<string , IDataAttribute> serverObject)
            : base(serverObject)
        {}

        protected override Type GetItemValueSmoObjectType(object obj)
        {
            return typeof(SmoAttribute);
        }

        #region IDataAttributeCollection Members

        public void Add(IDataAttribute dataAttribute)
        {
            throw new NotImplementedException();
        }

        public IDataAttribute CreateItem(AttributeClass attributeClass)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ICollection2DataTable Members

        public DataTable GetDataTable()
        {
        	return ((IDataAttributeCollection) serverControl).GetDataTable();
        }

        #endregion

        #region IEnumerable

        /// <summary>
        /// Для коллекции атрибутов вседа получаем IEnumerator объекта сервера
        /// </summary>
        /// <returns></returns>
        protected override System.Collections.IEnumerator Enumerator()
        {
            return serverControl.GetEnumerator();
        } // Enumerator

        #endregion IEnumerable
    }
}
