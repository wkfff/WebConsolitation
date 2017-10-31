using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.SMO.Design;
using Krista.FM.ServerLibrary;


namespace Krista.FM.Client.SchemeEditor.ControlObjects
{
    public class AssociateRuleMappingListControl : CustomListControl<IAssociateRule, IAssociateMapping>
    {
        public AssociateRuleMappingListControl(IAssociateRule controlObject, CustomTreeNodeControl parent)
            : base(controlObject.Name, controlObject.Name, controlObject, parent, 0)
        {
        }

        public override string Caption
        {
            get { return ((IAssociateRule)ControlObject).Name; }
        }

        public override CustomTreeNodeControl Create(IAssociateMapping item)
        {
            return new AssociateRuleMappingControl(new SmoAssociateMappingDesign(item), this);
        }

        protected override void ExpandNode()
        {
            try
            {
                StartExpand();

                List<CustomTreeNodeControl> list = new List<CustomTreeNodeControl>();
                IEnumerable levels = (IEnumerable)((IAssociateRule)ControlObject).Mappings;

                foreach (object item in levels)
                    list.Add(Create((IAssociateMapping)item));

                Nodes.AddRange(list.ToArray());
            }
            finally
            {
                EndExpand();
            }
        }


        [MenuAction("Создать соответствие", CheckMenuItemEnabling = "IsEditable")]
        public void AddNew()
        {
            IAssociateRule rule = (IAssociateRule)ControlObject;
            IAssociateMapping newObj = rule.Mappings.CreateItem();
            CustomTreeNodeControl node = Create(newObj);
            Nodes.Add(node);
            OnChange(this, new EventArgs());

            this.SchemeEditor.SelectObject(node.Key, true);
        }

        [MenuAction("Удалить"/*, CheckMenuItemEnabling = "CanDelete"*/, Images.Remove, CheckMenuItemEnabling = "IsEditable")]
        public void Delete()
        {
            AssociateRuleListControl list = (AssociateRuleListControl)Parent;
            IAssociateRule association = ControlObject as IAssociateRule;

            SmoAssociateRulesCollectionDesign smoCollection = (SmoAssociateRulesCollectionDesign)list.ControlObject;
            IModifiableCollection<string, IAssociateRule> collection = (IModifiableCollection<string, IAssociateRule>)smoCollection.ServerControl;

            if (MessageBox.Show(String.Format("Вы действительно хотите удалить правило сопоставления \r {0} ?", this.Caption), "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == System.Windows.Forms.DialogResult.Yes)
            {
                if (association.Name == smoCollection.AssociateRuleDefault)
                {
                    smoCollection.AssociateRuleDefault = "";
                }
                collection.Remove(association.ObjectKey);

                OnChange(this, new EventArgs());
                this.Remove();
            }
        }
    }
}
