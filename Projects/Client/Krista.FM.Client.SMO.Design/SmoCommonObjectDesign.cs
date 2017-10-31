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
        [DisplayName(@"������������ (Name)")]
        [Description("��� �������")]
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
        [DisplayName(@"���������� ������ ������������ (FullName)")]
        [Description("������ ��� �������")]
        public virtual string FullName
        {
            get { return serverControl.FullName; }
        }

        [Category("CommonObject.Naming")]
        [DisplayName(@"������������ ������� (FullDBName)")]
        [Description("������ ��� �������")]
        public virtual string FullDBName
        {
            get { return serverControl.FullDBName; }
        }

        [Category("CommonObject")]
        [DisplayName(@" (IsValid)")]
        [Description("���������� ��������� ������������ �������")]
        [Browsable(false)]
        public bool IsValid
        {
            get { return serverControl.IsValid; }
        }

        [Category("CommonObject.Naming")]
        [DisplayName(@"������� ������������ (Caption)")]
        [Description("������������ �������, ��������� � ����������")]
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
        [DisplayName(@"�������� (Description)")]
        [Description("�������� �������")]
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

        [DisplayName(@"������������ (OuterXml)")]
        [Description("XML ������������ �������")]
        [Browsable(false)]
        public string ConfigurationXml
        {
            get { return serverControl.ConfigurationXml; }
        }

        #endregion
    }
}
