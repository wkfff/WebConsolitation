using System.ComponentModel;

using Krista.FM.Common;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    public abstract class SmoCommonObjectDesign : SmoKeyIdentifiedObjectDesign<ICommonObject>, ICommonObject
    {
        protected SmoCommonObjectDesign(ICommonObject serverControl)
            : base(serverControl)
        {
        }

        #region ICommonObject Members

        [Category("CommonObject.Naming")]
        [DisplayName(@"Наименование (Name)")]
        [Description("Имя объекта")]
        [RefreshProperties(RefreshProperties.All)]
        public virtual string Name
        {
            get
            {
                return serverControl.Name;
            }
            set
            {
                if (ReservedWordsClass.CheckName(value))
                {
                    serverControl.Name = value;
                    CallOnChange();
                }
            }
        }

        [Category("CommonObject.Naming")]
        [DisplayName(@"Английское полное наименование (FullName)")]
        [Description("Полное имя объекта")]
        public virtual string FullName
        {
            get { return serverControl.FullName; }
        }

        [Category("CommonObject.Naming")]
        [DisplayName(@"Наименование таблицы (FullDBName)")]
        [Description("Полное имя объекта")]
        public virtual string FullDBName
        {
            get { return serverControl.FullDBName; }
        }

        [Category("CommonObject")]
        [DisplayName(@" (IsValid)")]
        [Description("Определяет состояние корректности объекта")]
        [Browsable(false)]
        public bool IsValid
        {
            get { return serverControl.IsValid; }
        }

        [Category("CommonObject.Naming")]
        [DisplayName(@"Русское наименование (Caption)")]
        [Description("Наименование объекта, выводимое в интерфейсе")]
        [RefreshProperties(RefreshProperties.All)]
        public virtual string Caption
        {
            get
            {
                return serverControl.Caption;
            }
            set
            {
                serverControl.Caption = value;
                CallOnChange();
            }
        }

        [Category("CommonObject")]
        [DisplayName(@"Описание (Description)")]
        [Description("Описание объекта")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string Description
        {
            get
            {
                return serverControl.Description;
            }
            set
            {
                serverControl.Description = value;
                CallOnChange();
            }
        }

        [DisplayName(@"Конфигурация (OuterXml)")]
        [Description("XML конфигурация объекта")]
        [Browsable(false)]
        public string ConfigurationXml
        {
            get { return serverControl.ConfigurationXml; }
        }

        #endregion
    }
}
