using System;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class AssociateRuleMappingControl : MinorObjectControl<IAssociateMapping>
    {
        public AssociateRuleMappingControl(IAssociateMapping controlObject, CustomTreeNodeControl parent)
            : base(
                String.Format("{0}.{1}", controlObject.DataValue.Name, controlObject.BridgeValue.Name), 
                "", controlObject, parent, 0)
        {
        }

        public override string Caption
        {
            get
            {
                IAssociateMapping am = (IAssociateMapping)ControlObject;

                string nameA = am.DataValue.Attribute != null ? am.DataValue.Attribute.Caption : am.DataValue.Name;
                string nameB = am.BridgeValue.Attribute != null ? am.BridgeValue.Attribute.Caption : am.BridgeValue.Name;

                return String.Format("{0} -> {1}", nameA, nameB);
            }
        }

        [MenuAction("Удалить", Images.Remove, CheckMenuItemEnabling = "IsEditable")]
        public void Delete()
        {
            AssociateRuleMappingListControl amListControl = (AssociateRuleMappingListControl)Parent;
            IAssociateRule associateRule = (IAssociateRule)amListControl.ControlObject;
            IAssociateMappingCollection amCollection = associateRule.Mappings;
            amCollection.Remove(((SmoAssociateMappingDesign)ControlObject).ServerControl);
            OnChange(this, new EventArgs());
            this.Remove();
        }
    }
}
