using System.ComponentModel;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    public class SmoDocumentDesign : SmoCommonDBObjectDesign, IDocument
    {
        public SmoDocumentDesign(IDocument serverControl)
            : base(serverControl)
        {
        }

        [Browsable(false)]
        public new IDocument ServerControl
        {
            get { return (IDocument)serverControl; }
        }

        [Browsable(false)]
        public override string Semantic
        {
            get
            {
                return base.Semantic;
            }
            set
            {
                base.Semantic = value;
            }
        }

        [Browsable(false)]
        public override string Caption
        {
            get
            {
                return base.Caption;
            }
            set
            {
                base.Caption = value;
            }
        }

        [Browsable(false)]
        public override string FullDBName
        {
            get
            {
                return base.FullDBName;
            }
        }

        [Browsable(false)]
        public override string FullName
        {
            get
            {
                return base.FullName;
            }
        }

        [Browsable(false)]
        public new string Description
        {
            get { return base.Description; }
            set { base.Description = value; }
        }

        #region IDocument Members

        [TypeConverter(typeof(EnumTypeConverter))]
        public DocumentTypes DocumentType
        {
            get { return ServerControl.DocumentType; }
        }

        [DisplayName(@"Конфигурация")]
        [Description("XML конфигурация объекта")]
        public virtual string Configuration
        {
            get { return ServerControl.Configuration; }
            set { ServerControl.Configuration = value; CallOnChange(); }
        }

        #endregion
    }
}
