using System.ComponentModel;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    class SmoAttributeDocumentDesign : SmoAttributeDesign
    {
        public SmoAttributeDocumentDesign(IDataAttribute serverObject)
            : base(serverObject)
        {
        }

        [DisplayName(@"Атрибут-документ (IsDocument)")]
        [Description("Определяет является ли атрибут документом.")]
        [TypeConverter(typeof(BooleanTypeConvertor))]
        [Browsable(true)]
        new public bool IsDocument
        {
            get { return serverControl is IDocumentAttribute; }
        }
    }
}
