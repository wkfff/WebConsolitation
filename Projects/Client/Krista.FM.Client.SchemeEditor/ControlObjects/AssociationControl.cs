using System;
using System.Windows.Forms;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class AssociationControl : MajorObjectControl<SmoAssociationDesign>
    {
        public AssociationControl(SmoAssociationDesign controlObject, CustomTreeNodeControl parent)
            : base(controlObject, parent, GetImageIndex(controlObject))
        {
            AddLoadNode();
        }

        private static int GetImageIndex(SmoAssociationDesign controlObject)
        {
            int imageIndex = (int)Images.Associasion;
            switch (controlObject.AssociationClassType)
            {
                case AssociationClassTypes.Bridge:
                case AssociationClassTypes.BridgeBridge:
                    imageIndex = (int)Images.AssociasionBridge;
                    break;
                case AssociationClassTypes.Link: 
                    imageIndex = (int)Images.Associasion;
                    break;
                case AssociationClassTypes.MasterDetail:
                    imageIndex = (int)Images.AssociationMD; 
                    break;
            }
            return imageIndex;
        }

        protected override void ExpandNode()
        {
            base.ExpandNode();

            SmoAttributeDesign attribute;
            SmoEntityDesign roleData;
            SmoEntityDesign roleBridge;

            SmoAssociationDesign association = (SmoAssociationDesign)ControlObject;

            if (association is SmoAssociationReadOnlyDesign)
            {
                attribute = new SmoAttributeReferenceReadOnlyDesign(association.RoleDataAttribute);
                roleData = new SmoEntityReadOnlyDesign((IEntity)association.RoleData);
                roleBridge = new SmoEntityReadOnlyDesign(association.RoleBridge);
            }
            else
            {
                attribute = new SmoAttributeReferenceDesign(association.RoleDataAttribute);
                roleData = new SmoEntityDesign((IEntity)association.RoleData);
                roleBridge = new SmoEntityDesign(association.RoleBridge);
            }

            Nodes.Add(new AttributeControl(attribute, this));
            Nodes.Add(new ReferenceClassControl(roleData, this));
            // Если обе роли не один и тот же объект, то добавляем и вторую.
            if (roleData.Key != roleBridge.Key)
                Nodes.Add(new ReferenceClassControl(roleBridge, this));

            if (association.AssociationClassType == AssociationClassTypes.Bridge ||
                association.AssociationClassType == AssociationClassTypes.BridgeBridge)
            {
                IBridgeAssociation bridgeAssociation = (IBridgeAssociation)association.ServerControl;
                Nodes.Add(new AssociateMappingListControl(bridgeAssociation.Mappings, this));
                Nodes.Add(new AssociateRuleListControl(bridgeAssociation.AssociateRules, this));
            }
        }

        public override string Caption
        {
            get
            {
                IAssociation co = ControlObject as IAssociation;
                return String.Format("{0} ({1})", co.FullCaption, co.FullName);
            }
        }

        public bool CanDelete()
        {
            return !(Parent is ReferenceAssociationListControl || Parent is ReferencedByAssociationListControl) && IsEditable();
        }

        [MenuAction("Удалить", Images.Remove, CheckMenuItemEnabling = "CanDelete")]
        public void Delete()
        {
            AssociationListControl documentList = (AssociationListControl)Parent;
            IAssociation association = ControlObject as IAssociation;

            SmoDictionaryBaseDesign<string, IEntityAssociation> smoCollection = (SmoDictionaryBaseDesign<string, IEntityAssociation>)documentList.ControlObject;
            IModifiableCollection<string, IEntityAssociation> collection = (IModifiableCollection<string, IEntityAssociation>)smoCollection.ServerControl;

            if (MessageBox.Show(String.Format("Вы действительно хотите удалить ассоциацию \r {0} ?", this.Caption), "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {
                collection.Remove(association.ObjectKey);
                association.RoleData.Associations.Remove(association.ObjectKey);
                association.RoleData.Attributes.Remove(association.RoleDataAttribute.ObjectKey);
                //association.RoleBridge.Associated.Remove(association.FullName);

                OnChange(this, new EventArgs());
                this.Remove();
            }
        }


        public override bool Draggable
        {
            get
            {
                return true;
            }
        }


    }
}
