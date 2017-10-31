using System;
using System.ComponentModel;

using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SMO.Design
{
    public class SmoSupplierCollectionDesign : SmoDictionaryBaseDesign<string, IDataSupplier>, IDataSupplierCollection
    {
        public SmoSupplierCollectionDesign(IDataSupplierCollection serverObject)
            : base(serverObject)
        {
        }

        #region IDataSupplierCollection Members

        [DisplayName(@"Новый поставщик (New)")]
        [Description("Новый поставщик")]
        public IDataSupplier New()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        [DisplayName(@"Добавьть (Add)")]
        [Description("Добавляет нового поставщика в коллекцию")]
        public void Add(IDataSupplier dataSupplier)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void EndEdit()
        {
            ((IDataSupplierCollection)serverControl).EndEdit();   
        }

        public void CancelEdit()
        {
            ((IDataSupplierCollection)serverControl).CancelEdit();
        }

        #endregion
    }
}
