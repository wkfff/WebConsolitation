using System.ComponentModel;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SMO.Design
{
    public class SmoDataKindDesign : SmoServerSideObjectDesign<IDataKind>, IDataKind
    {
        public SmoDataKindDesign(IDataKind supplier)
            : base(supplier)
        {
        }

        #region IDataKind Members

        [Browsable(false)]
        public IDataSupplier Supplier
        {
            get { return serverControl.Supplier; }
        }

        [DisplayName(@"Код (Code)")]
        [Description("Код поступающей информации")]
        public string Code
        {
            get
            {
                return serverControl.Code;
            }
            set
            {
                serverControl.Code = value;
            }
        }

        [DisplayName("Наименование (Name)")]
        [Description("Наименование поступающей информации")]
        public string Name
        {
            get
            {
                return serverControl.Name;
            }
            set
            {
                serverControl.Name = value;
            }
        }

        [DisplayName("Описание (Description)")]
        [Description("Описание поступающей информации")]
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

        [DisplayName("Вид параметров (ParamKind)")]
        [Description("Вид параметров поступающей информации")]
        public ParamKindTypes ParamKind
        {
            get
            {
                return serverControl.ParamKind;
            }
            set
            {
                serverControl.ParamKind = value;
            }
        }

        [DisplayName("Метод получения (TakeMethod)")]
        [Description("Метод получения данных")]
        public TakeMethodTypes TakeMethod
        {
            get
            {
                return serverControl.TakeMethod;
            }
            set
            {
                serverControl.TakeMethod = value;
            }
        }

        #endregion
    }
}
