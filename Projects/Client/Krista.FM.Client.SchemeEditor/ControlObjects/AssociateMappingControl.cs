using System;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;

namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class AssociateMappingControl : MinorObjectControl<IAssociateMapping>
    {
        public AssociateMappingControl(IAssociateMapping controlObject, CustomTreeNodeControl parent)
            : base(
                String.Format("{0}.{1}", controlObject.DataValue.Name, controlObject.BridgeValue.Name), 
                "", controlObject, parent, 0)
        {
        }

        public override string Caption
        {
            get
            {
                try
                {
                    IAssociateMapping am = (IAssociateMapping)ControlObject;

                    string nameA = am.DataValue.Attribute != null ? am.DataValue.Attribute.Caption : am.DataValue.Name;
                    string nameB = am.BridgeValue.Attribute != null ? am.BridgeValue.Attribute.Caption : am.BridgeValue.Name;
                    
                    return String.Format("{0} -> {1}", nameA, nameB);
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
        }

        [MenuAction("Удалить", Images.Remove, CheckMenuItemEnabling = "IsEditable")]
        public void Delete()
        {
            AssociateMappingListControl amListControl = (AssociateMappingListControl)Parent;
            IAssociateMappingCollection amCollection = (IAssociateMappingCollection)amListControl.ControlObject;
            bool result = amCollection.Remove(((SmoAssociateMappingDesign)ControlObject).ServerControl);
            if (result)
            {
                OnChange(this, new EventArgs());
                this.Remove();
            }
        }
    }
}
