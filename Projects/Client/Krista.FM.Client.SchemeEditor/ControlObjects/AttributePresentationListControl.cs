using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class AttributePresentationListControl : ModifiableListControl<SmoDictionaryBaseDesign<string, IDataAttribute>, IDataAttribute>
    {
        public AttributePresentationListControl(IDataAttributeCollection controlObject, CustomTreeNodeControl parent)
            : base("IDataAttributeCollection", "Атрибуты представления",
            new SmoDictionaryBaseDesign<string, IDataAttribute>(controlObject), parent, (int)Images.Attributes)
        {
        }

        public override CustomTreeNodeControl Create(IDataAttribute item)
        {
            if (item.Class == DataAttributeClassTypes.Reference)
                return new AttributeReferencePresentationControl(item, this);
            return new AttributePresentationControl(item, this);
        }

        public override void AddNew()
        {
        }
    }
}
