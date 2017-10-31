using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    public class SMODataAttributeCollectionDesign : SmoDictionaryBaseDesign<string, IDataAttribute>, IDataAttributeCollection
    {
        public SMODataAttributeCollectionDesign(IDictionaryBase<string, IDataAttribute> serverControl)
            : base(serverControl)
        { }

        protected override Type GetItemValueSmoObjectType(object obj)
        {
            return typeof(SmoAttributeDesign);
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
            return ((IDataAttributeCollection)serverControl).GetDataTable();
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
