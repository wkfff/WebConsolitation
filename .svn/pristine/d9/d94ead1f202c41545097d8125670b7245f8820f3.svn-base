using System;
using System.ComponentModel;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SMO.Design
{
    public class SmoSupplierDesign : SmoServerSideObjectDesign<IDataSupplier>, IDataSupplier
    {
        public SmoSupplierDesign(IDataSupplier supplier)
            : base(supplier)
        {
        }

        #region IDataSupplier Members

        [DisplayName(@"Имя поставщика (Name)")]
        [Description("Имя поставщика данных")]
        public string Name
        {
            get
            {
                return serverControl.Name;
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        [DisplayName(@"Описание поставщика (Description)")]
        [Description("Описание поставщика данных")]
        public string Description
        {
            get
            {
                return serverControl.Description;
            }
            set
            {
                serverControl.Description = value;
            }
        }

        [DisplayName(@"Коллекция видов (DataKinds)")]
        [Description("Коллекция видов поступающей информации")]
        public IDataKindCollection DataKinds
        {
            get { return serverControl.DataKinds; }
        }

        #endregion
    }
}
