using System.ComponentModel;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SMO.Design
{
    public class SmoAttributeDocumentEntityDesign : SmoAttributeDesign, IDocumentEntityAttribute
    {
        public SmoAttributeDocumentEntityDesign(IDocumentEntityAttribute serverObject)
            : base(serverObject)
        {
        }

        #region IDocumentEntityAttribute

        [Category("KeyIdentifiedObject")]
        [DisplayName(@"”никальный ключ исходного объекта (SourceEntityKey)")]
        [Browsable(true)]
        public string SourceEntityKey
        {
            get { return ((IDocumentEntityAttribute)serverControl).SourceEntityKey; }
        }

        [Category("KeyIdentifiedObject")]
        [DisplayName(@"”никальный ключ исходного атрибута (SourceEntityAttributeKey)")]
        [Browsable(true)]
        public string SourceEntityAttributeKey
        {
            get { return ((IDocumentEntityAttribute)serverControl).SourceEntityAttributeKey; }
        }

        public void SetSourceAttribute(IDataAttribute sourceAttribute)
        {
            ((IDocumentEntityAttribute)serverControl).SetSourceAttribute(sourceAttribute);
        }

        #endregion IDocumentEntityAttribute
    }
}
